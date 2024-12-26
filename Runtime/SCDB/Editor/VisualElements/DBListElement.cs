using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace Redellion
{
    public class DBListElement : VisualElement
    {
        public ScriptableObject SC => sc;

        private ScriptableObject sc;
        private Label[] labels;

        internal void PopulateView(ScriptableObject sc, string[] labelTexts)
        {
            Clear();
            // DEBUG: needed?
            //foreach (var label in this.labels)
            //    Object.DestroyImmediate(label);

            this.sc = sc;
            List<Label> labels = new();

            foreach (var labelText in labelTexts)
            {
                var label = new Label(labelText);
                label.AddToClassList("dbListElement-label");
                labels.Add(label);
                Add(label);
            }
            this.labels = labels.ToArray();
        }


    }
}
