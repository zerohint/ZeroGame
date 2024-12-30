using UnityEngine;

[CreateAssetMenu(fileName = "Main Thread Dispatcher", menuName = "Game/Managers/Main Thread Dispatcher")]
public class MainThreadDispatcher : SingletonSC<MainThreadDispatcher>
{
    private static UnityMainThreadDispatcher _instance;

    public override void Initialize()
    {
        base.Initialize();
        // Create an instance at main thread.
        UnityMainThreadDispatcher.Instance();
    }

    public static void Enqueue(System.Action action) => UnityMainThreadDispatcher.Instance().Enqueue(action);
}
