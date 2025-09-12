using System;
using System.Collections.Concurrent;
using UnityEngine;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static readonly ConcurrentQueue<Action> _executionQueue = new ConcurrentQueue<Action>();
    private static UnityMainThreadDispatcher _instance = null;

    public static UnityMainThreadDispatcher Instance()
    {
        if (_instance == null)
        {
            throw new Exception("UnityMainThreadDispatcher could not find an instance in the scene.");
        }
        return _instance;
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void Update()
    {
        while (_executionQueue.TryDequeue(out var action))
        {
            action.Invoke();
        }
    }

    public void Enqueue(Action action)
    {
        _executionQueue.Enqueue(action);
    }
}