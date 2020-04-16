using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OGEditorCoroutines {

#if UNITY_EDITOR
    private static int createdEditorCoroutines = 0;
    private static Dictionary<int, UnityEditor.EditorApplication.CallbackFunction> runningEditorCoroutines = new Dictionary<int, UnityEditor.EditorApplication.CallbackFunction>();
    private static Dictionary<int, Stack<IEnumerator>> editorCoroutineStack = new Dictionary<int, Stack<IEnumerator>>();

    public static bool IsRegularCorouteAvailable
    {
        get
        {
            return !(Application.isEditor && !Application.isPlaying);
        }
    }

    public static int GetNextCoroutineID()
    {
        return createdEditorCoroutines;
    }

    public static int Run(IEnumerator coroutine)
    {
        var id = createdEditorCoroutines++;
        UnityEditor.EditorApplication.CallbackFunction coroutineUpdater = () =>
        {
            // if stack of ienumerables to handle is empty, the entire coroutine can be stopped
            if (editorCoroutineStack[id].Count == 0)
            {
                Stop(id);
                return;
            }
            else
            {
                // let's find the current active Ienumerable for this coroutine and see what happens when we run a step
                var currentIEnumerable = editorCoroutineStack[id].Peek();
                if (!currentIEnumerable.MoveNext())
                {
                    // stepping was stopped, so let's remove the coroutine
                    editorCoroutineStack[id].Pop();
                }
                // if a new ienumerator is yield returned, add to the stack to process
                else if (currentIEnumerable.Current is IEnumerator)
                {
                    editorCoroutineStack[id].Push(currentIEnumerable.Current as IEnumerator);
                }
            }
        };

        runningEditorCoroutines.Add(id, coroutineUpdater);
        editorCoroutineStack.Add(id, new Stack<IEnumerator>());
        editorCoroutineStack[id].Push(coroutine);

        UnityEditor.EditorApplication.update += coroutineUpdater;
        coroutineUpdater.Invoke();

        return id;
    }

    public static void StopAll()
    {
        var keys = new int[runningEditorCoroutines.Keys.Count];
        runningEditorCoroutines.Keys.CopyTo(keys, 0);

        for (int i = 0; i < keys.Length; i++)
        {
            Stop(keys[i]);
        }
    }

    public static void Stop(int id)
    {
        if (!runningEditorCoroutines.ContainsKey(id)) return;
        UnityEditor.EditorApplication.update -= runningEditorCoroutines[id];
        runningEditorCoroutines.Remove(id);
    }

    public static float Time
    {
        get
        {
            return (float)UnityEditor.EditorApplication.timeSinceStartup;
        }
    }

#else
    public static bool IsRegularCorouteAvailable
    {
        get
        {
            return true;
        }
    }
    public static int GetNextCoroutineID()
    {
        return -1;
    }

    public static int Run(IEnumerator coroutine)
    {
        return -1;
    }

    public static void StopAll() { }

    public static void Stop(int id) { }

    public static float Time { get { return 0; } }
#endif
}
