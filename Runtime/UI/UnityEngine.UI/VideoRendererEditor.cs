#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CustomEditor(typeof(VideoRenderer))]
[CanEditMultipleObjects]
public class VideoRendererEditor : Editor
{
    SerializedProperty videoClipProp;
    SerializedProperty placeholderProp;
    SerializedProperty playonawakeProp;
    SerializedProperty loopProp;

    SerializedProperty raycastTargetProp;
    SerializedProperty maskableProp;
    SerializedProperty raycastPaddingProp;

    void OnEnable()
    {
        videoClipProp = serializedObject.FindProperty("videoClip");
        placeholderProp = serializedObject.FindProperty("placeholder");
        playonawakeProp = serializedObject.FindProperty("playOnAwake");
        loopProp = serializedObject.FindProperty("loop");

        // RawImage base class 
        raycastTargetProp = serializedObject.FindProperty("m_RaycastTarget");
        maskableProp = serializedObject.FindProperty("m_Maskable");
        raycastPaddingProp = serializedObject.FindProperty("m_RaycastPadding");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(videoClipProp);
        EditorGUILayout.PropertyField(placeholderProp);
        EditorGUILayout.PropertyField(playonawakeProp);
        EditorGUILayout.PropertyField(loopProp);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("RawImage Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(raycastTargetProp, new GUIContent("Raycast Target"));
        EditorGUILayout.PropertyField(raycastPaddingProp, new GUIContent("Raycast Padding"));
        EditorGUILayout.PropertyField(maskableProp, new GUIContent("Maskable"));

        serializedObject.ApplyModifiedProperties();
    }
}

#endif