using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ZeroGame
{
    public class SCDBViewEditor : EditorWindow
    {
        private const string PATH = "Assets/1_Components/Utilities/SCDB/Editor/";

        private DBView dbView;
        private InspectorView inspectorView;

        [MenuItem("Window/Asset Management/" + SCDB.TITLE)]
        public static void ShowExample()
        {
            SCDBViewEditor wnd = GetWindow<SCDBViewEditor>();
            wnd.titleContent = new GUIContent(SCDB.TITLE);
        }



        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Instantiate UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(PATH + "SCDBViewEditor.uxml");
            VisualElement ui = visualTree.CloneTree();
            root.Add(ui);

            // Load and apply style
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(PATH + "SCDBViewEditor.uss");
            root.styleSheets.Add(styleSheet);

            dbView = root.Q<DBView>();
            dbView.OnElementSelected = OnListElementSelected;
            inspectorView = root.Q<InspectorView>();
            OnSelectionChange();
        }

        private void OnSelectionChange()
        {
            if (Selection.activeObject is SCDBSC db && AssetDatabase.CanOpenAssetInEditor(db.GetInstanceID()))
            {
                dbView.PopulateView(db);
            }
        }

        private void OnListElementSelected(ScriptableObject listElement)
        {
            inspectorView.SetTarget(listElement);
        }
    }
}
