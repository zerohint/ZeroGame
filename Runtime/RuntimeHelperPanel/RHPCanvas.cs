using UnityEngine;
using UnityEngine.InputSystem;
namespace ZeroGame.RHP
{
    public class RHPCanvas : MonoBehaviour
    {
        // DEBUG: use gizmo etc?
        [SerializeField] private InputAction toggleInput;
        [Space]
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private Transform listTransform;

        /// <summary>
        /// Called by serialized events
        /// </summary>
        public void TogglePanel()
        {
            mainPanel.SetActive(!mainPanel.activeSelf);
        }

        private void Start()
        {
            // foreach (var panel in Kit.IllumateConfigSC.Instance.rdpSettings.panels)
            //     Instantiate(panel, listTransform);
            mainPanel.SetActive(false);
        }


        private void OnEnable()
        {
            toggleInput.Enable();
            toggleInput.performed += TogglePerformed;
        }
        private void OnDisable()
        {
            toggleInput.Disable();
            toggleInput.performed -= TogglePerformed;
        }

        private void TogglePerformed(InputAction.CallbackContext obj) => TogglePanel();
    }
}
