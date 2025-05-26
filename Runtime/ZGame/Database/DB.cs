using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Linq;
using ZeroGame.DB;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace ZeroGame
{
    public class DBResponse : ResponseBase
    {
        public DBResponse(UnityWebRequest request) : base(request) { }
        public DocumentSnapshot AsSnapshot => JsonConvert.DeserializeObject<DocumentSnapshot>(RawBody);

        public DocumentSnapshot[] AsQuerySnapshot
        {
            get
            {
                var jsObj = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(RawBody);

                DocumentSnapshot[] ret = new DocumentSnapshot[jsObj.Count];
                for(int i=0; i<jsObj.Count; i++)
                {
                    var jObj = jsObj[i].Values.First() as JObject;
                    if (jObj == null) continue;
                    ret[i] = jObj.ToObject<DocumentSnapshot>();
                }
                if (ret.Length == 1 && ret[0] == null)
                    return new DocumentSnapshot[0];
                return ret;
            }
        }
    }

    public enum Method
    {
        GET,
        POST,
        PUT,
        DELETE,
        PATCH,
    }
}

namespace ZeroGame.DB
{
    public class DB : SystemBase
    {
        public string IdToken {  get; internal set; }

        public Collection Collection(string id) => new (this, id);

        internal void SendRequest(string path, Method method, Dictionary<string, object> jsonData, Action<DBResponse> OnComplete)
        {
            TheSingleton.Instance.StartCoroutine(SendRequestCR(path, method, jsonData, OnComplete));

            IEnumerator SendRequestCR(string path, Method method, Dictionary<string, object> jsonData, Action<DBResponse> OnComplete)
            {
                string url = $"{ZGameManager.BASE_URL}projects/{ZGameManager.Instance.ProjectId}/databases/(default)/documents/{path}?key={ZGameManager.Instance.ApiKey}";
                Debug("Url: " + url);
                using UnityWebRequest request = method switch
                {
                    Method.GET => UnityWebRequest.Get(url),
                    Method.POST => new UnityWebRequest(url, "POST"),
                    Method.PUT => new UnityWebRequest(url, "PUT"),
                    Method.DELETE => new UnityWebRequest(url, "DELETE"),
                    Method.PATCH => new UnityWebRequest(url, "PATCH"),
                    _ => throw new ArgumentException("Unsupported method", nameof(method))
                };

                if (!string.IsNullOrEmpty(IdToken))
                    request.SetRequestHeader("Authorization", $"Bearer {IdToken}");

                request.downloadHandler = new DownloadHandlerBuffer();
                if (jsonData != null)
                {
                    UnityEngine.Debug.Assert(method == Method.POST || method == Method.PUT || method == Method.PATCH);
                    string formattedJson = FirestoreHelper.ConvertToFirestoreJson(jsonData);
                    Debug("Json: " + formattedJson);
                    request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(formattedJson));
                    request.SetRequestHeader("Content-Type", "application/json");
                }

                yield return request.SendWebRequest();

                var response = new DBResponse(request);
                Debug("Response: " + response.IsSuccess + ", " + response.RawBody);
                OnComplete?.Invoke(response);

                static void Debug(string message) { /*UnityEngine.Debug.Log(message);*/ }
            }
        }
    }
}
