//using UnityEditor;
//using UnityEngine;

//[CustomPropertyDrawer(typeof(SerializableDictionary<,>), true)]
//public class SerializableDictionaryDrawer : PropertyDrawer
//{
//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    {
//        EditorGUI.BeginProperty(position, label, property);

//        var keysProp = property.FindPropertyRelative("keys");
//        var valuesProp = property.FindPropertyRelative("values");

//        position.height = EditorGUIUtility.singleLineHeight;
//        property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);

//        if (property.isExpanded)
//        {
//            EditorGUI.indentLevel++;
//            for (int i = 0; i < keysProp.arraySize; i++)
//            {
//                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

//                Rect keyRect = new Rect(position.x, position.y, position.width / 2 - 5, EditorGUIUtility.singleLineHeight);
//                Rect valueRect = new Rect(position.x + position.width / 2 + 5, position.y, position.width / 2 - 5, EditorGUIUtility.singleLineHeight);

//                EditorGUI.PropertyField(keyRect, keysProp.GetArrayElementAtIndex(i), GUIContent.none);
//                EditorGUI.PropertyField(valueRect, valuesProp.GetArrayElementAtIndex(i), GUIContent.none);
//            }

//            Rect addButtonRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);

//            if (GUI.Button(addButtonRect, "Add Element"))
//            {
//                keysProp.arraySize++;
//                valuesProp.arraySize++;
//            }

//            EditorGUI.indentLevel--;
//        }

//        EditorGUI.EndProperty();
//    }

//    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//    {
//        if (!property.isExpanded)
//        {
//            return EditorGUIUtility.singleLineHeight;
//        }

//        var keysProp = property.FindPropertyRelative("keys");
//        return (keysProp.arraySize + 2) * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
//    }
//}
