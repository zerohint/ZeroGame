using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ZeroGame.Editor
{
    /// <summary>
    /// FTP target for WebGL "Build and Deploy".
    ///
    /// Lives in ProjectSettings/ (not in Assets/) so the credentials never end up inside a
    /// player build. Host / user / directory are shared with the team through the asset;
    /// the password is deliberately NOT serialized here - it is kept in EditorPrefs, which
    /// is per-machine and outside the repository. See <see cref="Password"/>.
    /// </summary>
    internal sealed class WebGLDeploySettings : ScriptableObject
    {
        private const string ASSET_PATH = "ProjectSettings/ZeroGameWebGLDeploy.asset";
        private const string PASSWORD_PREF_PREFIX = "ZeroGame.WebGLDeploy.Password.";

        [SerializeField] private string host = "";
        [SerializeField] private int port = 21;
        [SerializeField] private string username = "";
        [SerializeField] private string remoteDirectory = "/public_html";
        [SerializeField] private string buildPath = "Build/WebGL";
        [SerializeField] private bool useFtps;
        [SerializeField] private bool passiveMode = true;
        [SerializeField] private bool pruneRemoteBuildFolder = true;

        private static WebGLDeploySettings instance;

        internal string Host { get => host; set => host = value; }
        internal int Port { get => port; set => port = value; }
        internal string Username { get => username; set => username = value; }
        internal string RemoteDirectory { get => remoteDirectory; set => remoteDirectory = value; }
        internal bool UseFtps { get => useFtps; set => useFtps = value; }
        internal bool PassiveMode { get => passiveMode; set => passiveMode = value; }

        /// <summary>
        /// After a successful upload, delete the player files of previous deploys from the
        /// remote Build/ folder. Hashed file names are never overwritten, so without this the
        /// server keeps every build ever deployed.
        /// </summary>
        internal bool PruneRemoteBuildFolder
        {
            get => pruneRemoteBuildFolder;
            set => pruneRemoteBuildFolder = value;
        }

        /// <summary>Where BuildPipeline writes the player. Relative paths resolve against the project root.</summary>
        internal string BuildPath { get => buildPath; set => buildPath = value; }

        internal string AbsoluteBuildPath =>
            string.IsNullOrWhiteSpace(buildPath)
                ? ""
                : Path.IsPathRooted(buildPath)
                    ? buildPath
                    : Path.GetFullPath(Path.Combine(ProjectRoot, buildPath));

        private static string ProjectRoot => Directory.GetParent(Application.dataPath).FullName;

        /// <summary>
        /// Stored per machine in EditorPrefs, keyed by project path, so it is never committed.
        /// </summary>
        internal string Password
        {
            get => EditorPrefs.GetString(PasswordKey, "");
            set => EditorPrefs.SetString(PasswordKey, value ?? "");
        }

        private static string PasswordKey => PASSWORD_PREF_PREFIX + ProjectRoot;

        internal static WebGLDeploySettings Instance
        {
            get
            {
                if (instance != null) return instance;

                var loaded = InternalEditorUtility.LoadSerializedFileAndForget(ASSET_PATH);
                if (loaded != null && loaded.Length > 0)
                    instance = loaded[0] as WebGLDeploySettings;

                if (instance == null)
                {
                    instance = CreateInstance<WebGLDeploySettings>();
                    instance.Save();
                }

                return instance;
            }
        }

        internal void Save()
        {
            InternalEditorUtility.SaveToSerializedFileAndForget(new Object[] { this }, ASSET_PATH, true);
        }

        /// <summary>
        /// Reports every missing field at once instead of failing on the first one.
        /// </summary>
        internal bool Validate(out string problems)
        {
            var report = "";

            if (string.IsNullOrWhiteSpace(host)) report += "\n - FTP Host is empty.";
            if (port <= 0 || port > 65535) report += $"\n - FTP Port '{port}' is not a valid port.";
            if (string.IsNullOrWhiteSpace(username)) report += "\n - FTP Username is empty.";
            if (string.IsNullOrEmpty(Password)) report += "\n - FTP Password is empty (set it in Project Settings > ZeroGame).";
            if (string.IsNullOrWhiteSpace(remoteDirectory)) report += "\n - Remote Directory is empty (use '/' for the FTP root).";
            if (string.IsNullOrWhiteSpace(buildPath)) report += "\n - Build Path is empty.";

            problems = report;
            return report.Length == 0;
        }
    }
}
