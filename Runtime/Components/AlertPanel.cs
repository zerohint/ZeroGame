using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class AlertPanel : MonoSingleton<AlertPanel>
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private Button button;

    public static void Alert(string message, string title = "Alert", Action OnConfirm = null, string buttonText = "Okay")
    {
        Instance.titleText.text = title;
        Instance.messageText.text = message;
        Instance.buttonText.text = buttonText;
        Instance.button.onClick.RemoveAllListeners();
        Instance.button.onClick.AddListener(() => OnConfirm?.Invoke());
        Instance.button.onClick.AddListener(() => Instance.ClosePanel());
        Instance.OpenPanel();
    }

    public void OpenPanel()
    {
        Instance.gameObject.SetActive(true);
    }
    public void ClosePanel()
    {
        Instance.gameObject.SetActive(false);
    }
}
