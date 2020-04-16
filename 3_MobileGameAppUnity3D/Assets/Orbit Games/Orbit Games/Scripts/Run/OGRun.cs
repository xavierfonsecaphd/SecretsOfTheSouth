using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OGRun : OGSingletonBehaviour<OGRun> {

    protected override void OnSingletonInitialize() { }

    private LinkedList<IOGBaseTask> tasks = new LinkedList<IOGBaseTask>();
    private Queue<Action> lateUpdateTodo = new Queue<Action>();

    public static IOGBaseTask AddTask(IOGBaseTask task)
    {
        GetInstance().tasks.AddLast(task);
        return task;
    }

    public static OGTask<T> AddTask<T>(string description, Action<OGTask<T>, int> stepper, Dictionary<string, string> requestContext, Action<OGTaskResult> onCompleted = null)
    {
        OGTask<T> task = new OGTask<T>(description, (OGTask<T> thisTask, int step) =>
        {
            stepper(thisTask, step);
        }, requestContext, onCompleted);
        GetInstance().tasks.AddLast(task);
        return task;
    }

    private void Update()
    {
        if (tasks.Count > 0)
        {
            LinkedListNode<IOGBaseTask> currentNode = tasks.First;
            while (currentNode != null)
            {
                IOGBaseTask task = currentNode.Value;
                if (task.IsCompleted())
                {
                    var temp = currentNode;
                    tasks.Remove(temp);
                } else
                {
                    task.Update();
                }
                currentNode = currentNode.Next;
            }
        }
    }

    private static void RunCoroutine(IEnumerator coroutine)
    {
        if (!OGEditorCoroutines.IsRegularCorouteAvailable)
        {
            OGEditorCoroutines.Run(coroutine);
        }
        else
        {
            I.StartCoroutine(coroutine);
        }
    }

    public static void LateUpdate(Action method)
    {
        I.lateUpdateTodo.Enqueue(method);
    }

    public static void Coroutine(IEnumerator routine)
    {
        RunCoroutine(routine);
    }

    public static void NextFrame(Action method)
    {
        RunCoroutine(GetInstance().NextFrameCoroutine(method));
    }

    private IEnumerator NextFrameCoroutine(Action method)
    {
        yield return null;
        method();
    }

    public static void Delayed(float seconds, Action method)
    {
        RunCoroutine(GetInstance().DelayedCoroutine(seconds, method)); 
    }

    private IEnumerator DelayedCoroutine(float seconds, Action method)
    {
        if (OGEditorCoroutines.IsRegularCorouteAvailable)
        {
            yield return new WaitForSeconds(seconds);
        }
        else
        {
            yield return new WaitForEditorSeconds(seconds);
        }

        method();
    }

    private void LateUpdate()
    {
        while (lateUpdateTodo.Count > 0)
        {
            var dequeued = lateUpdateTodo.Dequeue();
            if (dequeued != null) dequeued.Invoke();
        }
    }
}
