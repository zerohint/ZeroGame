using UnityEngine;

/// <summary>
/// Singleton structure with instance caching
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class MonoInstance<T> : MonoBehaviour where T : MonoInstance<T>
{
    private static volatile T _instance = null;

    public static T Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<T>();

            return _instance;
        }
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }
}