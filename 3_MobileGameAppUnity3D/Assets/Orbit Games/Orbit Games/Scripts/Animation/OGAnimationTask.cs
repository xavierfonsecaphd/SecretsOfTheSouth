using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class OGAnimationTask
{
    private class Trigger
    {
        public float percentRunning;
        public Action onTrigger;
    }

    public delegate void OnAnimationAtTimeListener(float time, bool isAnimationOverruled);
    public delegate void OnAnimationEndedListener(OGAnimator.RunningResult.EndState reason);

    public OGAnimationTargets target;
    public OGAnimationState origin;
    public IOGStateAnimation animation;
    public OGAnimationMask mask;

    public OnAnimationAtTimeListener onAnimationAtTime;
    public OnAnimationEndedListener onAnimationEnded;

    public float delay;
    public float speed;
    public float fadeIn;
    public bool randomStartTime;
    public bool unscaledTime;
    public OGLoopType loop;

    public OGAnimator.RunningResult runningResult;
    private List<Trigger> triggers = new List<Trigger>();

    public void AddTrigger(float percentTime, Action triggerAction)
    {
        triggers.Add(new Trigger()
        {
            percentRunning = percentTime,
            onTrigger = triggerAction
        });
    }

    public void OnTimeUpdated()
    {
        if (onAnimationAtTime != null)
        {
            onAnimationAtTime.Invoke(runningResult.time, runningResult.animationOverruled);
        }
        CheckTriggers();
    }

    public void OnAnimationEnded(OGAnimator.RunningResult.EndState endState)
    {
        runningResult.endState = endState;
        CheckTriggers();

        if (onAnimationAtTime != null)
        {
            onAnimationAtTime.Invoke(runningResult.time, runningResult.animationOverruled);
        }

        // call finished listener
        if (onAnimationEnded != null)
        {
            onAnimationEnded.Invoke(runningResult.endState);
        }
    }

    private void CheckTriggers()
    {
        float percentNow = runningResult.time / runningResult.duration;
        if ((int)runningResult.endState <= 1)
        {
            for (int i = 0; i < triggers.Count; i++)
            {
                if (triggers[i].percentRunning <= percentNow + 0.00001f) // a tiny extra trigger range for precision errors
                {
                    if (triggers[i].onTrigger != null)
                    {
                        triggers[i].onTrigger.Invoke();
                    }
                    triggers.RemoveAt(i);
                    i--;
                }
            }
        }

    }
}
