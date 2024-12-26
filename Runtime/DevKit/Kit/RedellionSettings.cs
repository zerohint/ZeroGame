//using UnityEditor;
//using UnityEngine;

//internal abstract class RedellionSettings<T> : ScriptableObject where T : RedellionSettings<T>
//{
//    /// <summary>
//    /// Get or create the settings asset
//    /// </summary>
//    /// <returns></returns>
//    internal static T GetAsset()
//    {
//        // TODO: Redellion folder check
//        string path = $"Assets/Redellion/{typeof(T)}.asset";
//        var settings = AssetDatabase.LoadAssetAtPath<T>(path);
//        if (settings == null)
//        {
//            settings = ScriptableObject.CreateInstance<T>();
//            // TODO: editor check
//            AssetDatabase.CreateAsset(settings, path);
//            AssetDatabase.SaveAssets();
//        }
//        return settings;
//    }

//    internal static SerializedObject GetSerializedSettings()
//    {
//        return new SerializedObject(GetAsset());
//    }
//}
