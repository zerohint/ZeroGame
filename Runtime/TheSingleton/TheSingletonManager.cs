using UnityEditor;
using UnityEngine;

namespace ZeroGame
{
    /// <summary>
    /// Ensures TheSingleton is exists
    /// </summary>
    [InitializeOnLoad]
    internal static class TheSingletonManager
    {
        private const string ASSET_PATH_IN_PROJECT = "Assets/ZeroGame/TheSingleton.prefab";
        private const string ASSET_PATH_IN_PACKAGE = "Packages/ZeroGame/Runtime/TheSingleton/TheSingleton.prefab";
        private const string ASSET_PATH_PACKAGE_MODE = "Assets/Runtime/TheSingleton/TheSingleton.prefab";

        private static TheSingleton theSingletonInstance;


        static TheSingletonManager()
        {
            //BuildPlayerWindow.RegisterBuildPlayerHandler(OnBuild);
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                theSingletonInstance = GameObject.Instantiate(GetAsset());
                theSingletonInstance.name = "TheSingleton";
            }
            else if (state == PlayModeStateChange.ExitingPlayMode)
            {
                Object.DestroyImmediate(theSingletonInstance.gameObject);
            }
        }

        /// <summary>
        /// Get or create TheSingleton prefab
        /// </summary>
        /// <returns></returns>
        internal static TheSingleton GetAsset()
        {
            // Try loading from the project folder first
            var asset = AssetDatabase.LoadAssetAtPath<TheSingleton>(ASSET_PATH_IN_PROJECT);

            if (asset == null)
            {
                // If asset isn't found, we are likely in package mode, try loading from the package folder
                asset = AssetDatabase.LoadAssetAtPath<TheSingleton>(ASSET_PATH_PACKAGE_MODE);

                if (asset == null)
                {
                    // If asset still not found, copy it from the package to the Assets folder
                    bool result = AssetDatabase.CopyAsset(ASSET_PATH_IN_PACKAGE, ASSET_PATH_IN_PROJECT);

                    if (result)
                    {
                        // Refresh the asset database and load the asset from the project folder
                        AssetDatabase.Refresh();
                        asset = AssetDatabase.LoadAssetAtPath<TheSingleton>(ASSET_PATH_IN_PROJECT);
                    }
                    else
                    {
                        Debug.LogError("TheSingleton prefab not found in the package. Please ensure the prefab is located in the package at 'Packages' + ASSET_PATH.");
                    }
                }
            }
            return asset;
        }
    }

}