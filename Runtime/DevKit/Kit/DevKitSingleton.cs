using UnityEngine;

namespace Redellion.DevKit
{
    /// <summary>
    /// Singleton scene instance for the DevKit
    /// </summary>
    public class DevKitSingleton : MonoBehaviour
    {
        public static DevKitSingleton Instance { get; private set; }

        // HERE
        //[Header("References")]
        //public Modals modals;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
