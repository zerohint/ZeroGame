using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine;
using System;

namespace ZeroGame.RHP
{
    public class SendReportPanel : RHPPanelBase
    {
        public TMP_InputField inputField;
        public Button sendButton;
        public Button copyButton;
        public TMP_Text sendButtonText;

        private string log;

        private void Start()
        {
            sendButton.onClick.AddListener(SendReport);
            copyButton.onClick.AddListener(CopyReport);
            
        }

        private void CopyReport()
        {
            throw new NotImplementedException();
            log = Reporter.GenerateReport();
            // Clipboard.CopyToClipboard(log);
            StartCoroutine(CopyButtonTextCR());
        }

        private void SendReport()
        {
            throw new NotImplementedException();
            // string reportText = inputField.text == "" ? "No report text provided." : "- Report Text: " + inputField.text;
            // Reporter.Log(reportText);
            // inputField.interactable = false;
            // sendButton.interactable = false;
            // sendButtonText.text = "Sending...";
            // log = Reporter.GenerateReport();
            // string sender = $"{SystemInfo.deviceName} ({SystemInfo.deviceModel})";
            // IApi.SendRequest(new SendReport(sender, log), OnReportResult);
        }


        // private void OnReportResult(RequestResult result)
        // {
        //     if (result.result)
        //     {
        //         sendButtonText.text = "Sent!";
        //     }
        //     else
        //     {
        //         sendButtonText.text = "Sending failed!";
        //         Clipboard.CopyToClipboard(log);
        //         Modals.Info("Failed to send report! Logs has been copied to your clipboard. You can send the logs to the developers manually.");
        //         Reporter.LogError("SendReportPanel.OnReportResult | Failed to send report: " + result.message);
        //     }
        // }

        private IEnumerator CopyButtonTextCR()
        {
            var txt = copyButton.GetComponentInChildren<TMP_Text>();
            txt.text = "Done!";
            yield return new WaitForSeconds(1);
            txt.text = "Copy";
        }
    }
}