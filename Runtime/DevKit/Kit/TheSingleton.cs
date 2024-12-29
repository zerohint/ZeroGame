using UnityEngine;

namespace ZeroGame
{
    /// <summary>
    /// Singleton scene instance for the DevKit
    /// </summary>
    public class TheSingleton : MonoBehaviour
    {
        public static TheSingleton Instance { get; private set; }

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
