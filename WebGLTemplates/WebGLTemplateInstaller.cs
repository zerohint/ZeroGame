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

                if (AssetDatabase.IsValidFolder(destination))
                    continue;

                if (!AssetDatabase.CopyAsset(source, destination))
                    Debug.LogError($"Template kopyalanamadı: {source}");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}