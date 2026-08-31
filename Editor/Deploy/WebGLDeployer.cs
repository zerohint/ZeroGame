using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace ZeroGame.Editor
{
    /// <summary>
    /// Builds the WebGL player and uploads it to the FTP target configured in
    /// Project Settings > ZeroGame (and Project Settings > Player > WebGL Deploy).
    /// </summary>
    internal static class WebGLDeployer
    {
        [MenuItem("Tools/ZeroHint/WebGL/Build and Deploy")]
        internal static void BuildAndDeploy()
        {
            if (!Build()) return;
            Deploy();
        }

        [MenuItem("Tools/ZeroHint/WebGL/Deploy Last Build")]
        internal static void DeployLastBuild()
        {
            Deploy();
        }

        /// <summary>Builds the WebGL player into the configured build path. False = do not deploy.</summary>
        internal static bool Build()
        {
            var settings = WebGLDeploySettings.Instance;

            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.WebGL)
            {
                Debug.LogError(
                    "[ZeroGame] Build and Deploy needs WebGL as the active build target. " +
                    $"Current target is {EditorUserBuildSettings.activeBuildTarget}. " +
                    "Switch it in Project Settings > ZeroGame or File > Build Profiles.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(settings.BuildPath))
            {
                Debug.LogError("[ZeroGame] Build Path is empty, set it in Project Settings > ZeroGame.");
                return false;
            }

            var scenes = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();

            if (scenes.Length == 0)
            {
                Debug.LogError("[ZeroGame] No enabled scenes in Build Settings, nothing to build.");
                return false;
            }

            var outputPath = settings.AbsoluteBuildPath;
            Directory.CreateDirectory(outputPath);

            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = outputPath,
                target = BuildTarget.WebGL,
                options = BuildOptions.None
            });

            if (report == null)
            {
                Debug.LogError("[ZeroGame] WebGL build returned no report, build did not run.");
                return false;
            }

            if (report.summary.result != BuildResult.Succeeded)
            {
                Debug.LogError(
                    $"[ZeroGame] WebGL build {report.summary.result} " +
                    $"({report.summary.totalErrors} errors). Deploy cancelled.");
                return false;
            }

            Debug.Log($"[ZeroGame] WebGL build succeeded in {report.summary.totalTime:mm\\:ss} -> {outputPath}");
            return true;
        }

        /// <summary>Uploads whatever is currently in the configured build path.</summary>
        internal static void Deploy()
        {
            var settings = WebGLDeploySettings.Instance;

            if (!settings.Validate(out var problems))
            {
                Debug.LogError($"[ZeroGame] FTP deploy settings are incomplete:{problems}");
                return;
            }

            var buildPath = settings.AbsoluteBuildPath;
            if (!Directory.Exists(buildPath))
            {
                Debug.LogError($"[ZeroGame] Build folder not found: '{buildPath}'. Build first.");
                return;
            }

            var files = WebGLFtpClient.CollectFiles(buildPath);
            if (files.Count == 0) return;

            var client = new WebGLFtpClient(settings);
            var uploaded = 0;

            try
            {
                for (var i = 0; i < files.Count; i++)
                {
                    var (local, remote) = files[i];

                    var cancelled = EditorUtility.DisplayCancelableProgressBar(
                        "ZeroGame - Deploying WebGL build",
                        $"{i + 1}/{files.Count}  {remote}",
                        (float)i / files.Count);

                    if (cancelled)
                    {
                        Debug.LogWarning(
                            $"[ZeroGame] Deploy cancelled after {uploaded}/{files.Count} files. " +
                            "The remote build is now a mix of old and new files, re-deploy to fix it.");
                        return;
                    }

                    client.UploadFile(local, remote);
                    uploaded++;
                }
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    $"[ZeroGame] FTP upload failed after {uploaded}/{files.Count} files " +
                    $"({settings.Host}:{settings.Port}{settings.RemoteDirectory}): {exception.Message}");
                return;
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            Debug.Log(
                $"[ZeroGame] Deployed {uploaded} files to " +
                $"{settings.Host}:{settings.Port}{settings.RemoteDirectory}");
        }

        /// <summary>Test Connection button: lists the remote directory and reports the result.</summary>
        internal static void TestConnection()
        {
            var settings = WebGLDeploySettings.Instance;

            if (!settings.Validate(out var problems))
            {
                Debug.LogError($"[ZeroGame] FTP deploy settings are incomplete:{problems}");
                return;
            }

            try
            {
                new WebGLFtpClient(settings).TestConnection();
                Debug.Log(
                    $"[ZeroGame] FTP connection OK: " +
                    $"{settings.Host}:{settings.Port}{settings.RemoteDirectory}");
            }
            catch (Exception exception)
            {
                Debug.LogError($"[ZeroGame] FTP connection failed: {exception.Message}");
            }
        }
    }
}
