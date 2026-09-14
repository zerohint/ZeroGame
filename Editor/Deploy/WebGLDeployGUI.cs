using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace ZeroGame.Editor
{
    /// <summary>
    /// The WebGL FTP deploy block, drawn in two places from one implementation:
    ///
    ///  - Project Settings > ZeroGame          (see ZeroGameProjectSettingsEditor)
    ///  - Project Settings > Player > WebGL Deploy, a child page of Player registered below.
    ///
    /// Unity's real "Publishing Settings" foldout is drawn by the platform module's internal
    /// settings extension, which has no public hook, so a sibling page under Player is as
    /// close as this can get without reflecting into PlayerSettingsEditor.
    /// </summary>
    internal static class WebGLDeployGUI
    {
        internal const string SETTINGS_PATH = "Project/Player/WebGL Deploy";

        [SettingsProvider]
        internal static SettingsProvider CreateProvider()
        {
            return new SettingsProvider(SETTINGS_PATH, SettingsScope.Project)
            {
                label = "WebGL Deploy",
                guiHandler = _ =>
                {
                    EditorGUILayout.Space();
                    using (new EditorGUI.IndentLevelScope())
                    {
                        Draw();
                    }
                },
                keywords = new HashSet<string>(new[]
                {
                    "ZeroGame", "WebGL", "FTP", "Deploy", "Publish", "Upload", "Build"
                })
            };
        }

        /// <summary>Opens the standalone page. Used from the ZeroGame settings block.</summary>
        internal static void OpenSettingsPage()
        {
            SettingsService.OpenProjectSettings(SETTINGS_PATH);
        }

        internal static void Draw()
        {
            var settings = WebGLDeploySettings.Instance;
            var isWebGL = EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebGL;

            EditorGUILayout.LabelField("WebGL FTP Deploy", EditorStyles.boldLabel);

            if (!isWebGL)
            {
                EditorGUILayout.HelpBox(
                    $"Active build target is {EditorUserBuildSettings.activeBuildTarget}. " +
                    "Build and Deploy only builds WebGL.",
                    MessageType.Info);

                if (GUILayout.Button("Switch Active Build Target to WebGL"))
                {
                    if (!EditorUserBuildSettings.SwitchActiveBuildTarget(NamedBuildTarget.WebGL, BuildTarget.WebGL))
                        Debug.LogError("[ZeroGame] Could not switch the active build target to WebGL. Is the WebGL build support module installed?");
                }
            }

            EditorGUI.BeginChangeCheck();

            settings.Host = EditorGUILayout.TextField(
                new GUIContent("FTP Host", "Host name or IP, without the ftp:// prefix."),
                settings.Host);

            settings.Port = EditorGUILayout.IntField(
                new GUIContent("FTP Port", "21 for plain FTP / explicit FTPS."),
                settings.Port);

            settings.Username = EditorGUILayout.TextField("FTP Username", settings.Username);

            var password = EditorGUILayout.PasswordField(
                new GUIContent("FTP Password", "Stored in EditorPrefs on this machine only, never in the project."),
                settings.Password);

            settings.RemoteDirectory = EditorGUILayout.TextField(
                new GUIContent("Remote Directory", "Upload target on the server, e.g. /public_html/mygame. Use / for the FTP root."),
                settings.RemoteDirectory);

            settings.UseFtps = EditorGUILayout.Toggle(
                new GUIContent("Use FTPS", "Explicit FTP over TLS. Turn off for plain FTP."),
                settings.UseFtps);

            settings.PassiveMode = EditorGUILayout.Toggle(
                new GUIContent("Passive Mode", "Leave on unless the server requires active FTP."),
                settings.PassiveMode);

            settings.PruneRemoteBuildFolder = EditorGUILayout.Toggle(
                new GUIContent(
                    "Clean Remote Build",
                    "After a successful upload, delete player files of older deploys from the remote " +
                    "Build/ folder. File names are content hashes, so old builds are never overwritten."),
                settings.PruneRemoteBuildFolder);

            EditorGUILayout.Space();

            DrawBuildPathField(settings);

            var changed = EditorGUI.EndChangeCheck();

            if (password != settings.Password)
                settings.Password = password;

            if (changed)
                settings.Save();

            EditorGUILayout.Space();

            EditorGUILayout.HelpBox(
                "Host, user, port and directory are saved in ProjectSettings/ZeroGameWebGLDeploy.asset. " +
                "The password is kept in EditorPrefs on this machine, so it is never committed.",
                MessageType.None);

            EditorGUILayout.Space();

            var isConfigured = settings.Validate(out var problems);
            if (!isConfigured)
                EditorGUILayout.HelpBox("Missing settings:" + problems, MessageType.Warning);

            using (new EditorGUI.DisabledScope(!isConfigured))
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Test Connection", GUILayout.Height(24)))
                    WebGLDeployer.TestConnection();

                using (new EditorGUI.DisabledScope(!Directory.Exists(settings.AbsoluteBuildPath)))
                {
                    if (GUILayout.Button("Deploy Last Build", GUILayout.Height(24)))
                        WebGLDeployer.Deploy();
                }
            }

            using (new EditorGUI.DisabledScope(!isConfigured || !isWebGL))
            {
                if (GUILayout.Button("Build and Deploy", GUILayout.Height(32)))
                {
                    var target = $"{settings.Host}:{settings.Port}{settings.RemoteDirectory}";

                    if (EditorUtility.DisplayDialog(
                            "Build and Deploy WebGL",
                            $"Build the WebGL player and upload it to\n\n{target}\n\nExisting files with the same name are overwritten.",
                            "Build and Deploy",
                            "Cancel"))
                    {
                        // Delayed so the settings window is not mid-layout when the build blocks the Editor.
                        EditorApplication.delayCall += WebGLDeployer.BuildAndDeploy;
                    }
                }
            }
        }

        private static void DrawBuildPathField(WebGLDeploySettings settings)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                settings.BuildPath = EditorGUILayout.TextField(
                    new GUIContent("Build Path", "WebGL output folder. Relative paths resolve against the project root."),
                    settings.BuildPath);

                if (GUILayout.Button("...", GUILayout.Width(28)))
                {
                    var picked = EditorUtility.SaveFolderPanel("WebGL Build Folder", settings.AbsoluteBuildPath, "");
                    if (!string.IsNullOrEmpty(picked))
                    {
                        settings.BuildPath = ToProjectRelative(picked);
                        settings.Save();
                        GUI.FocusControl(null);
                    }
                }
            }
        }

        private static string ToProjectRelative(string absolutePath)
        {
            var root = Directory.GetParent(Application.dataPath).FullName + Path.DirectorySeparatorChar;

            return absolutePath.StartsWith(root)
                ? absolutePath.Substring(root.Length).Replace('\\', '/')
                : absolutePath;
        }
    }
}
