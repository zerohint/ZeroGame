using UnityEngine;
using UnityEngine.SceneManagement;

namespace ZeroGame.RHP
{
    public class AppInfoPanel : RHPPanelBase
    {
        [SerializeField] private TMPro.TMP_Text infoText;

        private void Awake()
        {
            infoText.text =
                $"Version: {Application.version}\n" +
                $"Scene: {SceneManager.GetActiveScene().name}";
        }
    }
}