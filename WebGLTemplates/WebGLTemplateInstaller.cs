using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace ZeroGame.Editor
{
    public sealed class WebGLTemplateInstaller : AssetPostprocessor
    {
        private static readonly string[] TemplateNames =
        {
            "Zero-FullScreen",
            "Zero-Modern"
        };

        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths,
            bool didDomainReload)
        {
            if (didDomainReload)
                Install(false);
        }

        [MenuItem("Tools/ZeroHint/Reinstall WebGL Templates")]
        private static void Reinstall()
        {
            Install(true);
        }

        private static void Install(bool overwrite)
        {
            var package = UnityEditor.PackageManager.PackageInfo.FindForAssembly(
                typeof(WebGLTemplateInstaller).Assembly
            );

            if (package == null)
            {
                Debug.LogError("ZeroHint package path bulunamadı.");
                return;
            }

            const string destinationRoot = "Assets/WebGLTemplates";

            if (!AssetDatabase.IsValidFolder(destinationRoot))
                AssetDatabase.CreateFolder("Assets", "WebGLTemplates");

            foreach (var templateName in TemplateNames)
            {
                var source =
                    $"{package.assetPath}/WebGLTemplates/{templateName}";

                var destination =
                    $"{destinationRoot}/{templateName}";

                if (overwrite && AssetDatabase.IsValidFolder(destination))
                    AssetDatabase.DeleteAsset(destination);

                if (!AssetDatabase.IsValidFolder(destination) &&
                    !AssetDatabase.CopyAsset(source, destination))
                {
                    Debug.LogError($"Template kopyalanamadı: {source}");
                    continue;
                }

                // Runs for an existing folder too, so an older install still picks up
                // .htaccess changes shipped with the package.
                CopyHiddenFiles(package, templateName, destination);
            }

            EnableNameFilesAsHashes();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Copies the template's dot-files (".htaccess") with plain file IO.
        ///
        /// AssetDatabase treats a leading dot as "hidden" and skips those files, so a template
        /// copied through CopyAsset alone arrives without its .htaccess - and the build then has
        /// no Content-Encoding rules, which makes every compressed .br file fail to load in the
        /// browser. Unity's own template-to-build copy does include them, so only this step is missing.
        /// </summary>
        private static void CopyHiddenFiles(
            UnityEditor.PackageManager.PackageInfo package,
            string templateName,
            string destinationAssetPath)
        {
            var sourceDirectory = Path.Combine(package.resolvedPath, "WebGLTemplates", templateName);
            var destinationDirectory = Path.GetFullPath(destinationAssetPath);

            if (!Directory.Exists(sourceDirectory))
            {
                Debug.LogError($"[ZeroGame] Template source folder is missing: {sourceDirectory}");
                return;
            }

            foreach (var file in Directory.GetFiles(sourceDirectory, ".*", SearchOption.TopDirectoryOnly))
            {
                var name = Path.GetFileName(file);
                if (name == ".DS_Store") continue;

                File.Copy(file, Path.Combine(destinationDirectory, name), true);
            }
        }

        /// <summary>
        /// Publishing Settings > Name Files As Hashes. The templates' .htaccess caches
        /// Build/* forever, which is only safe when every build file is content-hashed.
        ///
        /// PlayerSettings.WebGL.nameFilesAsHashes is NOT usable here: its setter leaves the
        /// serialized field untouched, so the getter reports true while the Publishing
        /// Settings checkbox stays off and ProjectSettings.asset keeps webGLNameFilesAsHashes: 0.
        /// The SerializedObject is the only thing both the inspector and the build read.
        /// </summary>
        private static void EnableNameFilesAsHashes()
        {
            const string settingsPath = "ProjectSettings/ProjectSettings.asset";
            const string fieldName = "webGLNameFilesAsHashes";

            var playerSettings = AssetDatabase
                .LoadAllAssetsAtPath(settingsPath)
                .FirstOrDefault(asset => asset is PlayerSettings);

            if (playerSettings == null)
            {
                Debug.LogError($"[ZeroGame] PlayerSettings couldn't be found: {settingsPath}.");
                return;
            }

            var serializedSettings = new SerializedObject(playerSettings);
            var nameFilesAsHashes = serializedSettings.FindProperty(fieldName);

            if (nameFilesAsHashes == null)
            {
                Debug.LogError($"[ZeroGame] PlayerSettings has no field: '{fieldName}' " + playerSettings);
                return;
            }

            if (nameFilesAsHashes.boolValue)
                return;

            nameFilesAsHashes.boolValue = true;
            serializedSettings.ApplyModifiedProperties();
            EditorUtility.SetDirty(playerSettings);

            // ProjectSettings.asset is not an AssetDatabase asset; only a project save flushes it.
            EditorApplication.ExecuteMenuItem("File/Save Project");

            Debug.Log("[ZeroGame] WebGL Publishing Settings > Name Files As Hashes set True ");
        }
    }
}