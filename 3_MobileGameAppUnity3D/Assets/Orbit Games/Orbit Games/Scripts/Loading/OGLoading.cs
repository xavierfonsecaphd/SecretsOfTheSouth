using GameToolkit.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OGLoading : MonoBehaviour {

    [Header("Settings")]
    public float minLoadTime;
    public bool managesSelfActivation = false;

    [Header("References")]
    public TMP_Text taskDescription;

    [Header("Dynamic State")]
    [ReadOnly]
    public bool loading;
    [ReadOnly]
    public float loadTime;
    [ReadOnly]
    public int todo;
    [ReadOnly]
    public int completed;

    public Action onTasksCompleted;

    private List<LoadingTask> loadingTasks = new List<LoadingTask>();

    private class LoadingTask
    {
        public Action<OGTaskResult> onLoaded;
        public IOGBaseTask task;
        public bool completed;

        public LoadingTask(IOGBaseTask task, Action<OGTaskResult> onLoaded)
        {
            this.onLoaded = onLoaded;
            this.task = task;
            completed = false;
        }
    }

    private void Awake()
    {
        ResetLoading();
    }

    public void AddTask(IOGBaseTask task, Action<OGTaskResult> onLoaded)
    {
        todo++;

        loading = true;
        loadTime = 0f;
        if (managesSelfActivation) gameObject.SetActive(true);
        loadingTasks.Add(new LoadingTask(task, onLoaded));
    }

    public void Update()
    {
        if (!loading || todo == 0)
        {
            if (managesSelfActivation) gameObject.SetActive(false);
            return;
        }

        loadTime += Time.deltaTime;

        if (completed == todo)
        {
            if (loadTime > minLoadTime)
            {
                // make a copy of the loaded tasks so we can reset, and then do there callbacks
                var completedTasks = loadingTasks.ToArray();

                // reset
                ResetLoading();
                
                // it is possible that from calling these callbacks new tasks
                // are added to be loaded, so we need to check this afterwards
                foreach (var completed in completedTasks)
                {
                    if (completed.onLoaded != null)
                        completed.onLoaded.Invoke(completed.task.Result);
                }

                // are we done now? or have tasks been added?
                if (completed == todo)
                {
                    // we are done
                    if (onTasksCompleted != null)
                        onTasksCompleted.Invoke();
                }
                else
                {
                    // we are not done, let's do another update to update this object immediately
                    Update();
                }
            }

            return;
        }

        OGFormattedLocalizedText someTaskDescription = null; // loadingTasks[0].task.GetDescription();
        foreach (var loading in loadingTasks)
        {
            if (someTaskDescription == null && !loading.completed)
                someTaskDescription = loading.task.GetDescription();

            if (!loading.completed && loading.task.IsCompleted())
            {
                loading.completed = true;
                completed++;
            }
        }

        SetDescription(someTaskDescription);
    }

    private void SetDescription(OGFormattedLocalizedText text)
    {
        if (taskDescription == null) return;
        if (text == null) return;
        taskDescription.text = text.ToString();
    }

    private void ResetLoading()
    {
        if (managesSelfActivation) gameObject.SetActive(false);

        todo = 0;
        completed = 0;
        loadingTasks.Clear();

        loadTime = 0f;
        loading = false;

        //if (taskDescription != null) 
        //    taskDescription.text = "";
    }

}
