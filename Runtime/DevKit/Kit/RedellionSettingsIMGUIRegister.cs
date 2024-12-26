//using Redellion.RuntimeDebugPanel;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;

//internal static class RedellionSettingsIMGUIRegister
//{
//    [SettingsProvider]
//    public static SettingsProvider CreateMyCustomSettingsProvider()
//    {
//        var provider = new SettingsProvider("Redellion/MyCustomIMGUISettings", SettingsScope.Project)
//        {
//            label = nameof(RDPPanelsSettings),
//            guiHandler = (searchContext) =>
//            {
//                var settings = RDPPanelsSettings.GetSerializedSettings();
//                var serializedSettings = new SerializedObject(settings);

//                EditorGUILayout.PropertyField(serializedSettings.FindProperty("infoText"), new GUIContent("My infoText"));

//                // Apply changes if any were made
//                if (serializedSettings.hasModifiedProperties)
//                {
//                    serializedSettings.ApplyModifiedProperties();
//                }
//            },

//            // Populate the search keywords to enable smart search filtering and label highlighting:
//            keywords = new HashSet<string>(new[] { "Number", "Some String" })
//        };

//        return provider;
//    }
//}