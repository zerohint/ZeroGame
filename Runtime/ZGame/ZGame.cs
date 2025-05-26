using UnityEngine;
using UnityEngine.Networking;

namespace ZeroGame
{
    [CreateAssetMenu(fileName = "ZGameManager", menuName = "Game/Managers/ZGameManager")]
    public class ZGame : SingletonSC<ZGame>
    {
        public static AuthManager Auth => Instance._auth;
        public static DB.DB DB => Instance._db;        



        private AuthManager _auth;
        private DB.DB _db;

        public override void Initialize()
        {
            base.Initialize();
            _auth = new();
            _db = new ();
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