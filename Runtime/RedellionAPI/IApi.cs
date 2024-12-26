using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace Redellion.API
{
    /// <summary>
    /// Api call class
    /// </summary>
    public sealed class IApi : MonoBehaviour
    {
        #region Singleton
        private static IApi _instance;
        private static IApi Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GameObject(nameof(IApi)).AddComponent<IApi>();
                return _instance;
            }
        }
        #endregion

        public static void SendRequest(RequestBase request, Action<RequestResult> OnResultCallback)
        {
            Instance.StartCoroutine(Instance.SendRequestCR(request, OnResultCallback));
        }

        private IEnumerator SendRequestCR(RequestBase request, Action<RequestResult> OnResultCallback)
        {
            throw new NotImplementedException();
            string apiUrl = "#";//DevKit.RedellionConfigSC.Instance.apiSettings.apiUrl;
            string appId = "#"; //Kit.RedellionConfigSC.Instance.apiSettings.appId;
            var postData = JsonConvert.SerializeObject(request.PostData);
            var url = $"{apiUrl}/{request.ApiName}/{request.RequestName}.php";

            using UnityWebRequest www = UnityWebRequest.Post(url, postData, "application/json");
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                string response = www.downloadHandler.text.Trim();
                if (response.StartsWith("\ufeff")) response = response[1..]; // Remove BOM
                RequestResult result;
                try
                {
                    result = JsonUtility.FromJson<RequestResult>(response);
                }
                catch
                {
                    result = new RequestResult { result = false, message = $"Invalid response from server: {response}" };
                }
                OnResultCallback?.Invoke(result);
            }
            else if(www.responseCode == 404)
            {
                OnResultCallback?.Invoke(new RequestResult { result = false, message = $"Api not found. Url: {url}" });
            }
            else
            {
                OnResultCallback?.Invoke(new RequestResult { result = false, message = $"Request error: {www.error}" });
            }
        }
    }

    public class RequestResult
    {
        public bool result;
        public string message;
    }
}
