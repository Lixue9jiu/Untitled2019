using UnityEngine;
using System;
using System.Threading;
using System.Collections.Generic;

public class TaskManager : MonoBehaviour
{
    public class WorkHandle
    {
        public Action callback;
    }

    class Work
    {
        public WorkHandle handle;
        public object obj;
        public Action<object> action;
    }

    Queue<WorkHandle> m_messageQueue = new Queue<WorkHandle>();

    object m_lock = new object();

    public void CreateWork(Action<object> work, object obj, Action callback)
    {
        ThreadPool.QueueUserWorkItem(HandleWaitCallback, new Work { action = work, obj = obj, handle = new WorkHandle { callback = callback } });
    }

    private void Update()
    {
        lock (m_lock)
            while (m_messageQueue.Count > 0)
                m_messageQueue.Dequeue().callback?.Invoke();
    }

    void HandleWaitCallback(object state)
    {
        var work = state as Work;
        work.action.Invoke(work.obj);
        lock (m_lock)
            m_messageQueue.Enqueue(work.handle);
    }
}
