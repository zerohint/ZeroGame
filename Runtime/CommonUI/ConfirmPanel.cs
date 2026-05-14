using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class ConfirmPanel : MonoSingleton<ConfirmPanel>
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private TMP_Text okButtonText;
    [SerializeField] private TMP_Text cancelButtonText;
    [SerializeField] private Button okButton;
    [SerializeField] private Button cancelButton;

    public static void Confirm(Context context)
    {
        Instance.titleText.text = context.Title ?? "Onayla";
        Instance.messageText.text = context.Message ?? "Onaylıyor musunuz?";
        Instance.okButtonText.text = context.OkButtonText ?? "Tamam";
        Instance.cancelButtonText.text = context.CancelButtonText ?? "Vazgeç";

        Instance.okButton.onClick.RemoveAllListeners();
        Instance.cancelButton.onClick.RemoveAllListeners();
        Instance.okButton.onClick.AddListener(() => context.OnConfirm?.Invoke());
        Instance.okButton.onClick.AddListener(() => Instance.ClosePanel());
        // Instance.okButton.onClick.AddListener(() => AudioManager.Instance.PlayUiSfx(UISoundType.Success));
        Instance.cancelButton.onClick.AddListener(() => context.OnCancel?.Invoke());
        // Instance.cancelButton.onClick.AddListener(() => AudioManager.Instance.PlayUiSfx(UISoundType.ClosePanel));
        Instance.cancelButton.onClick.AddListener(() => Instance.ClosePanel());
        Instance.OpenPanel();
    }

    public void OpenPanel()
    {
        // AudioManager.Instance.PlayUiSfx(UISoundType.OpenPopup);
        Instance.gameObject.SetActive(true);
    }
    public void ClosePanel()
    {
        Instance.gameObject.SetActive(false);
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
