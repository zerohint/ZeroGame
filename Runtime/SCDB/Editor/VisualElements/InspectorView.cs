using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ZeroGame
{
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }

        private Editor editor;

        public InspectorView()
        {
            
        }

        internal void SetTarget(ScriptableObject sc)
        {
            Clear();

            if (editor) Object.DestroyImmediate(editor);
            editor = Editor.CreateEditor(sc);
            IMGUIContainer container = new (() => editor.OnInspectorGUI());
            Add(container);
        }
    }
}
