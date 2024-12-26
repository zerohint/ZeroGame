using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;

namespace Redellion
{
    public class DBView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<DBView, UxmlTraits> { }
        
        public Action<ScriptableObject> OnElementSelected;

        private ListView listView;
        private Label label;

        private SCDBSC db;

        internal void PopulateView(SCDBSC db)
        {
            listView = this.Q<ListView>();
            label = this.Q<Label>("db-title");

            this.db = db;
            label.text = db.name;

            listView.makeItem = () => new DBListElement();
            listView.bindItem = (element, i) => (element as DBListElement).PopulateView(db.scriptables[i], new[] { db.scriptables[i].name, db.scriptables[i].GetType().ToString() });
            listView.itemsSource = db.scriptables;
            listView.selectionType = SelectionType.Single;
            listView.itemsChosen += OnElementSelected;
            listView.selectionChanged += OnElementSelected;

            void OnElementSelected(IEnumerable<object> objs) => this.OnElementSelected(objs.First() as ScriptableObject);
        }
    }
}
