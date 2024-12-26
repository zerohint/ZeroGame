namespace UnityEngine
{
    /// <summary>
    /// Singleton structure
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static volatile T _instance = null;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<T>();

                return _instance;
            }
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }
    }

    /// <summary>
    /// Singleton but creates one if not exist
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MonoExistSignleton<T> : MonoBehaviour where T : MonoExistSignleton<T>
    {
        private static volatile T _instance = null;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<T>();

                if (_instance == null)
                {
                    GameObject go = new(typeof(T).Name + " Singleton");
                    _instance = go.AddComponent<T>();
                }

                return _instance;
            }
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }
    }
}