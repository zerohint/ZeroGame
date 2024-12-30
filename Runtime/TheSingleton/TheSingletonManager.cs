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
        private const string ASSET_PATH_IN_PACKAGE = "Packages/com.zerogame/Runtime/TheSingleton/TheSingleton.prefab";
        private const string ASSET_PATH_PACKAGE_MODE = "Assets/Runtime/TheSingleton/TheSingleton.prefab";

        /// <summary>
        /// Is script running as a package?
        /// </summary>
        private static bool IS_PACKAGE_MODE => AssetDatabase.IsValidFolder("Packages/com.zerogame");
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
            TheSingleton ret = null;
            if (IS_PACKAGE_MODE)
            {
                ret = AssetDatabase.LoadAssetAtPath<TheSingleton>(ASSET_PATH_IN_PROJECT);
                if (ret == null)
                {
                    if (!AssetDatabase.IsValidFolder("Assets/ZeroGame")) AssetDatabase.CreateFolder("Assets", "ZeroGame");
                    AssetDatabase.CopyAsset(ASSET_PATH_IN_PACKAGE, ASSET_PATH_IN_PROJECT);
                    AssetDatabase.Refresh();
                    ret = AssetDatabase.LoadAssetAtPath<TheSingleton>(ASSET_PATH_IN_PROJECT);
                }
            }
            else
            {
                ret = AssetDatabase.LoadAssetAtPath<TheSingleton>(ASSET_PATH_PACKAGE_MODE);
            }
            return ret;
        }
    }

}