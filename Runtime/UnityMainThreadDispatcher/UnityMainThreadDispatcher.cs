using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

internal class UnityMainThreadDispatcher : MonoBehaviour
{
    private static readonly Queue<Action> _executionQueue = new();
    private static UnityMainThreadDispatcher _instance;
    private static Thread _mainThread;

    public static UnityMainThreadDispatcher Instance()
    {
        if (_instance == null)
        {
            _instance = new GameObject("UnityMainThreadDispatcher").AddComponent<UnityMainThreadDispatcher>();
            _instance.transform.parent = TheSingleton.Instance.transform;
        }
        return _instance;
    }

    public bool IsMainThread => Thread.CurrentThread == _mainThread;

    /// <summary>
    /// Adds an action to main thread queue.
    /// </summary>
    /// <param name="action"></param>
    public void Enqueue(Action action)
    {
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(action);
        }
    }


    private void Awake()
    {
        _mainThread = Thread.CurrentThread;
    }

    /// <summary>
    /// Update runs actions in the queue in main thread.
    /// </summary>
    private void Update()
    {
        lock (_executionQueue)
        {
            while (_executionQueue.Count > 0)
            {
                _executionQueue.Dequeue().Invoke();
            }
        }
    }
}
