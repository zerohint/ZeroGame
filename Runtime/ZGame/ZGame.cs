using UnityEngine;
using UnityEngine.Networking;

namespace ZeroGame
{
    [CreateAssetMenu(fileName = "ZGame Manager", menuName = "ZeroGame/ZGame Manager")]
    public class ZGame : SingletonSC<ZGame>
    {

        public static AuthManager Auth => Instance._auth;
        public static DB.DB DB => Instance._db;

        [Tooltip("Add your Firebase configuration here. You can find it by creating web app.")]
        [field:SerializeField] public Config config { get; private set; }



        private AuthManager _auth;
        private DB.DB _db;

        public override void Initialize()
        {
            base.Initialize();
            _auth = new();
            _db = new ();
        }

        private void OnValidate()
        {
            config.OnValidate();
        }



        [System.Serializable]
        public class Config
        {
            [field: SerializeField] public string ApiKey { get; private set; }
            [field: SerializeField] public string ProjectId { get; private set; }
            [field: SerializeField] public string AppId { get; private set; }
            public string AuthDomain => $"{ProjectId}.firebaseapp.com";
            public const string BASE_URL = "https://firestore.googleapis.com/v1/";

            public void OnValidate()
            {
                ApiKey = ApiKey?.Trim();
                ProjectId = ProjectId?.Trim();
                AppId = AppId?.Trim();
            }
        }
    }

    public abstract class SystemBase
    {
        
    }

    public abstract class ResponseBase
    {
        public readonly bool IsSuccess;
        public readonly long StatusCode;
        public readonly string RawBody;
        public string Error;

        public ResponseBase(string error)
        {
            IsSuccess = false;
            StatusCode = 0;
            RawBody = "";
            Error = error;
        }
        public ResponseBase(UnityWebRequest request)
        {
            IsSuccess = request.result == UnityWebRequest.Result.Success;
            StatusCode = request.responseCode;
            RawBody = request.downloadHandler?.text;
            Error = request.error;
        }

        public override string ToString()
        {
            return $"{this.GetType()} (IsSuccess:{IsSuccess}): {RawBody}";
        }
    }
}