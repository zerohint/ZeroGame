using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;

namespace ZeroGame
{
    /// <summary>
    /// Ensures TheSingleton is exists
    /// </summary>
    [InitializeOnLoad]
    internal static class TheSingletonChecker
    {
        const string assetPath = "Packages/ZeroGame/TheSingleton.prefab";


        static TheSingletonChecker()
        {
            BuildPlayerWindow.RegisterBuildPlayerHandler(OnBuild);
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        /// <summary>
        /// Add prefab to scenes on build
        /// </summary>
        /// <param name="buildPlayerOptions"></param>
        private static void OnBuild(BuildPlayerOptions buildPlayerOptions)
        {
            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                {
                    EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Single);
                    EnsureSingletonExists(EditorSceneManager.GetActiveScene());
                }
            }
            BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(buildPlayerOptions);
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                var activeScene = EditorSceneManager.GetActiveScene();

                if (activeScene.isDirty)
                {
                    Debug.LogWarning("save before play?");
                    return;
                }

                EnsureSingletonExists(activeScene);
            }
        }

        private static void EnsureSingletonExists(Scene scene)
        {
            var sceneManager = TheSingleton.Instance;
            if (sceneManager == null)
            {
                PrefabUtility.InstantiatePrefab(GetAsset());
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }
        }

        /// <summary>
        /// Get TheSingleton prefab
        /// </summary>
        /// <returns></returns>
        private static TheSingleton GetAsset() => AssetDatabase.LoadAssetAtPath<TheSingleton>(assetPath);
    }

}