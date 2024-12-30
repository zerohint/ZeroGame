using UnityEngine;

public sealed class TheSingleton : MonoBehaviour
{
    public static TheSingleton Instance { get; private set; }

    [SerializeField] private SingletonSCNonGeneric[] managers;
    [SerializeField] private GameObject[] singletonUI;
    [SerializeField] private GameObject[] gameObjects;

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

        foreach (var manager in managers) manager.Initialize();
        foreach (var ui in singletonUI) Instantiate(ui);
        foreach (var gameObject in gameObjects) Instantiate(gameObject);
    }


    /// <summary>
    /// Get singleton manager by type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetManager<T>() where T : SingletonSC<T>
    {
        Debug.Assert(Instance != null, "TheSingleton instance not found. Did you reached at awake?");

        foreach (var manager in Instance.managers)
            if (manager is T t)
                return t;
        return null;
    }
}
