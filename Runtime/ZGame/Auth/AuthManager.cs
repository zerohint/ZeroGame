using UnityEngine;
using System;
using ZeroGame;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;

public class AuthManager : SystemBase
{
    public Action OnLoginSucces;


    public const string SALT = "ezhelderdo";

    public Status status = new();
    
    /// <summary>
    /// JWT Auth token saved in player prefs.
    /// TODO: it expires, handle
    /// </summary>
    internal string AuthToken
    {
        get => PlayerPrefs.GetString("auth_token");
        private set => PlayerPrefs.SetString("auth_token", value);
    }

    /// <summary>
    /// Try login with Auth Token if not expired.
    /// </summary>
    /// <param name="callback">Is login succeed</param>
    public void TryAutoLogin(Action<bool> callback)
    {
        if (AuthToken.IsNullOrEmpty())
        {
            callback?.Invoke(false);
            // todo status class
            OnLoginSucces?.Invoke();
            return;
        }

        // TODO: try login with authtoken if not expired: signInWithCustomToken
        callback?.Invoke(false);
        //TheSingleton.Instance.StartCoroutine(LoginWithIDCoroutine(customId, onComplete));

        //static IEnumerator LoginWithIDCoroutine(string customId, Action<AuthResponse> onComplete)
        //{
        //    var requestData = new
        //    {
        //        customToken = customId,
        //        returnSecureToken = true
        //    };

        //    string jsonPayload = JsonConvert.SerializeObject(requestData);
        //    byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);

        //    string url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithCustomToken?key={Config.apiKey}";

        //    using UnityWebRequest request = new(url, "POST");
        //    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        //    request.downloadHandler = new DownloadHandlerBuffer();
        //    request.SetRequestHeader("Content-Type", "application/json");

        //    yield return request.SendWebRequest();

        //    string errorMessage = null;
        //    string idToken = null;

        //    if (request.result == UnityWebRequest.Result.Success)
        //    {
        //        var responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(request.downloadHandler.text);
        //        idToken = responseData["idToken"];
        //    }
        //    else
        //    {
        //        errorMessage = "Custom authentication failed";
        //        if (!string.IsNullOrEmpty(request.downloadHandler.text))
        //        {
        //            try
        //            {
        //                var errorData = JsonConvert.DeserializeObject<Dictionary<string, object>>(request.downloadHandler.text);
        //                if (errorData.TryGetValue("error", out object errorObj) && errorObj is Dictionary<string, object> errorDetails)
        //                {
        //                    errorMessage = errorDetails["message"]?.ToString();
        //                }
        //            }
        //            catch
        //            {
        //                errorMessage = request.downloadHandler.text;
        //            }
        //        }
        //        else
        //        {
        //            errorMessage = request.error;
        //        }
        //    }

        //    var authResponse = new AuthResponse(request)
        //    {
        //        IdToken = idToken,
        //        Error = errorMessage
        //    };

        //    onComplete?.Invoke(authResponse);
        //}
    }


    public void SignupWithMail(string email, string password, Action<AuthResponse> onComplete)
    {
        TheSingleton.Instance.StartCoroutine(SignupCoroutine(email, password, (r) =>
        {
            this.status.isLogin = true;
            this.status.loginType = Status.LoginType.Email;
            this.status.email = email;
            onComplete?.Invoke(r);
            if (r.IsSuccess) OnLoginSucces?.Invoke();
        }));

        
    }


    public void SigninWithMail(string email, string password, Action<AuthResponse> onComplete)
    {
        TheSingleton.Instance.StartCoroutine(SignInCoroutine(email, password, (r) =>
        {
            this.status.isLogin = true;
            this.status.loginType = Status.LoginType.Email;
            this.status.email = email;
            onComplete?.Invoke(r);
            if (r.IsSuccess) OnLoginSucces?.Invoke();
        }));
    }


    /// <summary>
    /// Login with custom string
    /// </summary>
    /// <param name="customId"></param>
    /// <param name="apiResponse"></param>
    public void LoginWithID(string customId, Action<AuthResponse> onComplete)
    {
        // TODO: cloudscript etc.

        string email = $"{customId}@loginwithid.com";
        string password = customId;

        TheSingleton.Instance.StartCoroutine(SignInCoroutine(email, password, authResponse =>
        {
            if (authResponse.IsSuccess)
            {
                OnLogin(authResponse);
            }
            else
            {
                var errorCode = GetErrorCode(authResponse.RawBody);
                switch (errorCode)
                {
                    case "INVALID_LOGIN_CREDENTIALS":
                        Debug.Log("Account doesn't exist, creating...");
                        TheSingleton.Instance.StartCoroutine(SignupCoroutine(email, password, OnLogin));
                        break;

                    default:
                        onComplete?.Invoke(authResponse);
                        break;
                }
            }
        }));

        void OnLogin(AuthResponse r)
        {
            if (r.IsSuccess)
            {
                this.status.isLogin = true;
                this.status.loginType = Status.LoginType.CustomId;
                this.status.customId = customId;
            }
            onComplete?.Invoke(r);
            if (r.IsSuccess) OnLoginSucces?.Invoke();
        }
        static string GetErrorCode(string errorMessage)
        {
            try
            {
                // Firebase error format: "message": "EMAIL_NOT_FOUND"
                var match = System.Text.RegularExpressions.Regex.Match(errorMessage, @"message""\s*:\s*""([^""]+)""");
                return match.Success ? match.Groups[1].Value : null;
            }
            catch
            {
                return null;
            }
        }
    }


    /// <summary>
    /// Login as guest
    /// </summary>
    /// <param name="apiResponse"></param>
    public void LoginAsGuest(Action<AuthResponse> onComplete)
    {
        string uniqueID = PlayerPrefs.GetString("GuestID", string.Empty);

        // If not, create a new one
        if (string.IsNullOrEmpty(uniqueID))
        {
            uniqueID = Guid.NewGuid().ToString();
            PlayerPrefs.SetString("GuestID", uniqueID);
            PlayerPrefs.Save();
        }

        // Use the unique ID for login
        LoginWithID(uniqueID, onComplete);
    }


    public void Logout()
    {
        status.isLogin = false;
        AuthToken = "";
    }


    IEnumerator SignInCoroutine(string email, string password, Action<AuthResponse> onComplete)
    {
        var requestData = new
        {
            email = email,
            password = password,
            returnSecureToken = true
        };

        string jsonPayload = JsonConvert.SerializeObject(requestData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);

        string signInUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={ZGameManager.Instance.ApiKey}";
        using UnityWebRequest request = new(signInUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        string errorMessage = null;
        if (request.result == UnityWebRequest.Result.Success)
        {
            var responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(request.downloadHandler.text);
            this.AuthToken = responseData["idToken"];
        }
        else
        {
            errorMessage = "Authentication failed";
            if (!string.IsNullOrEmpty(request.downloadHandler.text))
            {
                try
                {
                    var errorData = JsonConvert.DeserializeObject<Dictionary<string, string>>(request.downloadHandler.text);
                    errorMessage = errorData["error"];
                }
                catch
                {
                    errorMessage = request.downloadHandler.text;
                }
            }
            else
            {
                errorMessage = request.error;
            }
        }
        var authResponse = new AuthResponse(request);
        if (errorMessage != null) authResponse.Error = errorMessage;
        onComplete?.Invoke(authResponse);
    }

    IEnumerator SignupCoroutine(string email, string password, Action<AuthResponse> onComplete)
    {
        var requestData = new
        {
            email = email,
            password = password,
            returnSecureToken = true
        };

        string jsonPayload = JsonConvert.SerializeObject(requestData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);

        string signUpUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={ZGameManager.Instance.ApiKey}";
        using UnityWebRequest request = new(signUpUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        string errorMessage = null;
        if (request.result == UnityWebRequest.Result.Success)
        {
            var responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(request.downloadHandler.text);
            this.AuthToken = responseData["idToken"];
        }
        else
        {
            errorMessage = "Authentication failed";
            if (!string.IsNullOrEmpty(request.downloadHandler.text))
            {
                try
                {
                    var errorData = JsonConvert.DeserializeObject<Dictionary<string, string>>(request.downloadHandler.text);
                    errorMessage = errorData["error"];
                }
                catch
                {
                    errorMessage = request.downloadHandler.text;
                }
            }
            else
            {
                errorMessage = request.error;
            }
        }
        var authResponse = new AuthResponse(request);
        if (errorMessage != null) authResponse.Error = errorMessage;
        onComplete(authResponse);
    }


    public class AuthResponse : ResponseBase
    {
        public AuthResponse(UnityWebRequest request) : base(request) { }
        public AuthResponse(string error) : base(error) { }
    }


    public class Status
    {
        public bool isLogin = false;
        public LoginType loginType;
        public string email;

        /// <summary>
        /// CustomId is same with document id for now
        /// </summary>
        public string customId;

        public enum LoginType
        {
            NotLoggedIn,
            Email,
            CustomId
        }
    }
}