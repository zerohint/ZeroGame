using UnityEngine;

namespace ZeroGame.RHP
{
    [System.Serializable]
    internal class RHPPanelsSettings
    {
        public RHPPanelBase[] panels;

        [TextArea(minLines: 2, maxLines: 5)]
        public string infoText = "Illumate Studios";
    }
}
