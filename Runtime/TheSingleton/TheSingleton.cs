using UnityEngine;

[DefaultExecutionOrder(-1000)]
public sealed class TheSingleton : MonoBehaviour
{
    public static TheSingleton Instance { get; private set; }

    [SerializeField] private RectTransform uiParent;
    [Space]
    [SerializeField] private SingletonSCNonGeneric[] managers;
    [SerializeField] private MonoSingletonSerializable[] singletonUI;
    [SerializeField] private MonoSingletonSerializable[] gameObjects;

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
            return;
        }

        foreach (var manager in managers) manager.Initialize(); // TODO: createinstance
        for (int i = 0; i < singletonUI.Length; i++) singletonUI[i] = Instantiate(singletonUI[i], uiParent);
        for (int i = 0; i < gameObjects.Length; i++) gameObjects[i] = Instantiate(gameObjects[i], uiParent);
    }


    /// <summary>
    /// Get singleton manager by type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetManager<T>() where T : SingletonSC<T>
    {
        Debug.Assert(Instance != null, $"TheSingleton instance not found. Did you reached at awake? (Tried to reach: {typeof(T)})");

        foreach (var manager in Instance.managers)
            if (manager is T t)
                return t;
        return null;
    }


    /// <summary>
    /// Get singleton by type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetSingleton<T>() where T : MonoSingleton<T>
    {
        Debug.Assert(Instance != null, $"TheSingleton instance not found. Did you reached at awake? (Tried to reach: {typeof(T)})");

        foreach (var s in Instance.singletonUI)
            if (s is T t)
                return t;
        foreach (var s in Instance.gameObjects)
            if (s is T t)
                return t;
        return null;
    }
}
