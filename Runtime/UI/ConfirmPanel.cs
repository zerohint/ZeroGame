using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;

public class ConfirmPanel : Panel
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private TMP_Text okButtonText;
    [SerializeField] private TMP_Text cancelButtonText;
    [SerializeField] private Button okButton;
    [SerializeField] private Button cancelButton;


    /// <summary>
    /// Open confirm panel 
    /// </summary>
    /// <param name="context"></param>
    public static async Task Confirm(Context context)
    {
        var panel = await PanelManager.Instance.OpenPanel<ConfirmPanel>();

        panel.titleText.text = context.Title ?? "Onayla";
        panel.messageText.text = context.Message ?? "Onaylıyor musunuz?";
        panel.okButtonText.text = context.OkButtonText ?? "Tamam";
        panel.cancelButtonText.text = context.CancelButtonText ?? "Vazgeç";

        panel.okButton.onClick.RemoveAllListeners();
        panel.cancelButton.onClick.RemoveAllListeners();
        panel.okButton.onClick.AddListener(() => context.OnConfirm?.Invoke());
        panel.okButton.onClick.AddListener(() => panel.ClosePanel());
        // Instance.okButton.onClick.AddListener(() => AudioManager.Instance.PlayUiSfx(UISoundType.Success));
        panel.cancelButton.onClick.AddListener(() => context.OnCancel?.Invoke());
        // Instance.cancelButton.onClick.AddListener(() => AudioManager.Instance.PlayUiSfx(UISoundType.ClosePanel));
        panel.cancelButton.onClick.AddListener(() => panel.ClosePanel());
        panel.OpenPanel();
    }


    public void OpenPanel()
    {
        // AudioManager.Instance.PlayUiSfx(UISoundType.OpenPopup);
        gameObject.SetActive(true);
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }


    public struct Context
    {
        public string Title;
        public string Message;
        public string OkButtonText;
        public string CancelButtonText;
        public Action OnConfirm;
        public Action OnCancel;
    }
}
