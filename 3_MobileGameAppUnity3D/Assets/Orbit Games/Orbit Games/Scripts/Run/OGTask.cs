using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct OGTaskResult
{
    public OGTaskResult(object result, Dictionary<string, string> context, OGFormattedLocalizedText errorMessage = null)
    {
        Result = result;
        Failed = errorMessage != null;
        ErrorMessage = errorMessage;
        RequestContext = context;
    }

    public bool Failed { get; private set; }
    public object Result { get; private set; }
    public OGFormattedLocalizedText ErrorMessage { get; private set; }
    public Dictionary<string, string> RequestContext { get; private set; }

    public override string ToString()
    {
        if (Failed)
        {
            return ErrorMessage.ToString();
        }
        else
        {
            return Result.ToString();
        }
    }
}

public class OGTask : IOGBaseTask
{
    public enum State
    {
        Waiting, Running, Succeeded, Failed
    }

    private Action<OGTaskResult> onCompletedListeners;

    public OGFormattedLocalizedText Description { get; private set; }
    public State CurrentState { get; private set; }
    public OGTaskResult Result { get; private set; }

    public OGTask(OGFormattedLocalizedText description, Action<OGTaskResult> onCompleted = null)
    {
        Description = description;
        CurrentState = State.Waiting;
        if (onCompleted != null)
        {
            AddOnCompletedListener(onCompleted);
        }
    }

    public OGFormattedLocalizedText GetDescription()
    {
        return Description;
    }

    public void Update()
    {
        if (IsCompleted()) return;
        CurrentState = State.Running;
    }

    public OGTask AddOnCompletedListener(Action<OGTaskResult> onCompleted)
    {
        this.onCompletedListeners += onCompleted;
        if (IsCompleted())
        {
            onCompleted(Result);
        }
        return this;
    }

    public void SetFailed(OGFormattedLocalizedText message, string extraDetails = "")
    {
        CurrentState = State.Failed;
        Result = new OGTaskResult(extraDetails, null, message);
        if (onCompletedListeners != null) onCompletedListeners.Invoke(Result);
    }

    public void SetSucceeded()
    {
        CurrentState = State.Succeeded;
        Result = new OGTaskResult(null, null);
        if (onCompletedListeners != null) onCompletedListeners.Invoke(Result);
    }

    public bool IsCompleted()
    {
        return CurrentState == State.Failed || CurrentState == State.Succeeded;
    }

    public bool IsFailed()
    {
        return CurrentState == State.Failed;
    }

    public bool IsSucceeded()
    {
        return CurrentState == State.Succeeded;
    }
}

public class OGTask<T> : IOGBaseTask
{
    public enum State
    {
        Waiting, Running, Succeeded, Failed
    }
    
    public delegate void DoTaskStep(OGTask<T> thisTask, int step);

    private Action<OGTaskResult> onCompletedListeners;
    private DoTaskStep stepper;

    public OGFormattedLocalizedText Description { get; private set; }
    public State CurrentState { get; private set; }
    public Dictionary<string, string> Context { get; private set; }
    public OGTaskResult Result { get; private set; }
    public int Step { get; private set; }

    public OGTask(OGFormattedLocalizedText description, Action<OGTaskResult> onCompleted = null)
        : this(description, null, null, onCompleted) { }

    public OGTask(OGFormattedLocalizedText description, DoTaskStep stepper, Action<OGTaskResult> onCompleted = null)
        : this(description, stepper, null, onCompleted) { }

    public OGTask(OGFormattedLocalizedText description, DoTaskStep stepper, Dictionary<string, string> context, Action<OGTaskResult> onCompleted = null)
    {
        Description = description;
        CurrentState = State.Waiting;
        Context = context;
        this.stepper = stepper;
        if (onCompleted != null)
        {
            AddOnCompletedListener(onCompleted);
        }
    }

    public OGFormattedLocalizedText GetDescription()
    {
        return Description;
    }

    public void Update()
    {
        if (IsCompleted()) return;
        CurrentState = State.Running;
        stepper.Invoke(this, Step++);
    }

    public OGTask<T> AddOnCompletedListener(Action<OGTaskResult> onCompleted)
    {
        this.onCompletedListeners += onCompleted;
        if (IsCompleted())
        {
            onCompleted(Result);
        }
        return this;
    }

    public void SetFailed(OGFormattedLocalizedText message, string extraDetails = "")
    {
        CurrentState = State.Failed;
        Result = new OGTaskResult(extraDetails, Context, message);
        if (onCompletedListeners != null) onCompletedListeners.Invoke(Result);
    }

    public void SetSucceeded(T resultingData)
    {
        CurrentState = State.Succeeded;
        Result = new OGTaskResult(resultingData, Context);
        if (onCompletedListeners != null) onCompletedListeners.Invoke(Result);
    }

    public bool IsCompleted()
    {
        return CurrentState == State.Failed || CurrentState == State.Succeeded;
    }

    public bool IsFailed()
    {
        return CurrentState == State.Failed;
    }

    public bool IsSucceeded()
    {
        return CurrentState == State.Succeeded;
    }
}