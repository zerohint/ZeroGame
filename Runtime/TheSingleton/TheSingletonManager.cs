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
        private const string ASSET_PATH = "Assets/Runtime/TheSingleton/TheSingleton.prefab";
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
            var asset = AssetDatabase.LoadAssetAtPath<TheSingleton>(ASSET_PATH);
            if(asset == null)
            {
                Debug.LogError("HEREE!");
            }
            return asset;
        }
    }

}