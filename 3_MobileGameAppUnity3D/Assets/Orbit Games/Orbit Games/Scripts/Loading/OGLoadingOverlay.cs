using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OGLoadingOverlay : OGSingletonBehaviour<OGLoadingOverlay>
{
    protected override void OnSingletonInitialize()
    {
        loader.onTasksCompleted = OnLoadingCompleted;
        OGTransition.Disable(fullcoverBackground);
        OGTransition.Disable(loadingContainer);
    }

    [Header("References")]
    public GameObject fullcoverBackground;
    public GameObject loadingContainer;
    public OGLoading loader;

    private List<OGTaskResult> loadResults = new List<OGTaskResult>();
    private Action<List<OGTaskResult>> onLoadingCompleted;
    private static Dictionary<int, OGTask> simpleLoadingTasks = new Dictionary<int, OGTask>();
    private static List<int> activeSimpleLoaderIDs = new List<int>();
    private static int nextSimpleLoadingTaskID = 0;

    public static int ShowLoading(OGFormattedLocalizedText loadingText, bool stopAnyActiveLoaders = false, bool fullcoverLoading = false)
    {
        if (stopAnyActiveLoaders) StopAllLoaders();

        var simpleLoader = new OGTask(loadingText);
        simpleLoadingTasks.Add(++nextSimpleLoadingTaskID, simpleLoader);
        activeSimpleLoaderIDs.Add(nextSimpleLoadingTaskID);
        GetInstance()._addTask(simpleLoader, null, fullcoverLoading);
        return nextSimpleLoadingTaskID;
    }

    public static int ShowFullcoverLoading(OGFormattedLocalizedText loadingText, bool stopAnyActiveLoaders = false)
    {
        return ShowLoading(loadingText, stopAnyActiveLoaders, true);
    }

    public static void StopLoading(int loaderID)
    {
        if (simpleLoadingTasks.ContainsKey(loaderID))
        {
            simpleLoadingTasks[loaderID].SetSucceeded();
            simpleLoadingTasks.Remove(loaderID);
            activeSimpleLoaderIDs.Remove(loaderID);
        }
    }

    public static void StopOldestActiveLoading()
    {
        if (activeSimpleLoaderIDs.Count == 0) return;
        var id = activeSimpleLoaderIDs[0];
        simpleLoadingTasks[id].SetSucceeded();
        simpleLoadingTasks.Remove(id);
        activeSimpleLoaderIDs.RemoveAt(0);
    }

    public static void StopAllLoaders()
    {
        foreach (var key in simpleLoadingTasks.Keys)
        {
            simpleLoadingTasks[key].SetSucceeded();
        }
        simpleLoadingTasks.Clear();
    }

    public static void ShowLoading(IOGBaseTask task, Action<OGTaskResult> onLoaded = null)
    {
        GetInstance()._addTask(task, onLoaded, false);
    }

    public static void ShowLoading(IEnumerable<IOGBaseTask> tasks, Action<OGTaskResult> onLoaded = null)
    {
        foreach (var task in tasks)
        {
            GetInstance()._addTask(task, onLoaded, false);
        }
    }

    public static void ShowFullcoverLoading(IOGBaseTask task, Action<OGTaskResult> onLoaded = null)
    {
        GetInstance()._addTask(task, onLoaded, true);
    }

    public static void ShowFullcoverLoading(IEnumerable<IOGBaseTask> tasks, Action<OGTaskResult> onLoaded = null)
    {
        foreach (var task in tasks)
        {
            GetInstance()._addTask(task, onLoaded, true);
        }
    }

    public static void AddOnLoadingCompletedListener(Action<List<OGTaskResult>> onLoadingCompleted)
    {
        I.onLoadingCompleted += onLoadingCompleted;
    }

    private void _addTask(IOGBaseTask task, Action<OGTaskResult> onLoaded, bool fullcoverLoading = false)
    {
        OGTransition.In(loadingContainer);

        loader.AddTask(task, (result) => {
            loadResults.Add(result);
            if (onLoaded!= null)
                onLoaded.Invoke(result);
        });

        if (fullcoverLoading)
        {
            fullcoverBackground.SetActive(true);
            //OGTransition.In(fullcoverBackground);
        }
    }

    private void OnLoadingCompleted()
    {
        if (onLoadingCompleted != null)
            onLoadingCompleted.Invoke(loadResults);

        onLoadingCompleted = null;
        loadResults.Clear();

        //OGTransition.Out(fullcoverBackground);
        OGTransition.Out(loadingContainer);
    }
}
