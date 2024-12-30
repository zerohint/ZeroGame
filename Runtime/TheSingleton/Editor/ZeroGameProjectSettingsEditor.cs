using UnityEditor;
using UnityEngine;
using ZeroGame;

public class ZeroGameProjectSettingsEditor : EditorWindow
{
    private static TheSingleton theSingleton;

    [SettingsProvider]
    public static SettingsProvider CreateSettingsProvider()
    {
        var provider = new SettingsProvider("Project/ZeroGame", SettingsScope.Project)
        {
            label = "ZeroGame Settings",
            guiHandler = (searchContext) =>
            {
                if(theSingleton == null)
                {
                    theSingleton = TheSingletonManager.GetAsset();
                }

                // GameObject array'i duzenlenebilir panel
                SerializedObject serializedObject = new(theSingleton);

                EditorGUILayout.LabelField("Singletons", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("managers"), new GUIContent("Scriptable Managers"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("singletonUI"), new GUIContent("Singleton UI elements"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("gameObjects"), new GUIContent("Gameobject Managers"), true);

                // Save
                if (serializedObject.ApplyModifiedProperties())
                {
                    EditorUtility.SetDirty(theSingleton);
                }
            },
            keywords = new System.Collections.Generic.HashSet<string>(new[] { "ZeroGame", "ZeroHint", "Settings" })
        };

        return provider;
    }
}
