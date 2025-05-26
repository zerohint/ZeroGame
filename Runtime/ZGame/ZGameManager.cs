using System;
using System.Collections;
using UnityEngine;

namespace ZeroGame
{
    [CreateAssetMenu(fileName = "ZGame Manager", menuName = "ZeroGame/ZGame Manager")]
    public class ZGameManager : SingletonSC<ZGameManager>
    {
        [Header("Firebase")]
        [field:SerializeField] public string ApiKey { get; private set; }
        [field: SerializeField] public string ProjectId { get; private set; }
        [field: SerializeField] public string AppId { get; private set; }
        public string AuthDomain => $"{ProjectId}.firebaseapp.com";
        public const string BASE_URL = "https://firestore.googleapis.com/v1/";

        public override void Initialize()
        {
            base.Initialize();
            if (ApiKey.IsNullOrEmpty())
            {
                Debug.LogWarning("ApiKey is null. Please fill it in from manager.", this);
            }
        }
    }
}
