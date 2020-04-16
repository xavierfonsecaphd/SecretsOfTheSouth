using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OGAnimator : OGSingletonBehaviour<OGAnimator>
{
    protected override void OnSingletonInitialize() { }

    private static bool IsEditorPreviewMode()
    {
        return Application.isEditor && !Application.isPlaying;
    }

    public static void Animate(GameObject go, float speed)
    {
        Animate(go, "", speed);
    }

    public static void Animate(GameObject go, string tag = "", float speed = 1f)
    {
        var results = go.GetComponentsInChildren<OGAnimatable>();
        for (int i = 0; i < results.Length; i++)
        {
            results[i].PlayAnimation(tag, speed);
        }
    }

    public static int Animate(OGAnimationTask animationTask)
    {
        return I.animate(animationTask);
    }

    public static int GetNextAnimationID(GameObject gameObject)
    {
        if (IsEditorPreviewMode())
        {
            return OGEditorCoroutines.GetNextCoroutineID();
        }
        else
        {
            return I.animationIDs.GetOrDefault(gameObject, 0) + 1;
        }
    }

    public static void StopAllAnimations()
    {
        var objects = new GameObject[I.animationIDs.Keys.Count];
        I.animationIDs.Keys.CopyTo(objects, 0);

        for (int i = 0; i < objects.Length; i++)
        {
            StopAllAnimations(objects[i]);
        }
    }

    public static void StopAllAnimations(GameObject gameObject)
    {
        if (I.activeAnimations.ContainsKey(gameObject))
        {
            var activeAnimations = I.activeAnimations[gameObject];
            var activeAnimationTasks = I.activeAnimationTasks[gameObject];
            var animationIDs = new int[activeAnimations.Keys.Count];
            activeAnimations.Keys.CopyTo(animationIDs, 0);

            for (int i = 0; i < animationIDs.Length; i++)
            {
                var animationID = animationIDs[i];
                StopCoroutine(gameObject, animationID);

                // handle removal and listener
                I.HandleAnimationEnd(animationID, activeAnimationTasks.GetOrDefault(animationID), RunningResult.EndState.Terminated);
            }
        }
    }

    public static void StopCoroutine(GameObject gameObject, int animationID)
    {
        if (IsEditorPreviewMode())
        {
            OGEditorCoroutines.Stop(animationID);
        }
        else
        {
            if (I.activeAnimations.ContainsKey(gameObject))
            {
                var routine = I.activeAnimations[gameObject].GetOrDefault(animationID, null);
                if (routine != null)
                {
                    I.StopCoroutine(routine);
                }
            }
        }

        // handle removal and listener
        if (I.activeAnimationTasks.ContainsKey(gameObject))
        {
            I.HandleAnimationEnd(animationID,
                I.activeAnimationTasks[gameObject].GetOrDefault(animationID), RunningResult.EndState.Terminated);
        }
    }

    public static bool IsAnimationPlaying(GameObject gameObject, int animationID)
    {
        if (gameObject == null) return false;
        return I.activeAnimations[gameObject].ContainsKey(animationID);
    }

    [Header("Settings")]
    public float animationFadeOutSpeed = 2f;
    private Dictionary<GameObject, int> animationIDs = new Dictionary<GameObject, int>();
    private Dictionary<GameObject, Dictionary<int, Coroutine>> activeAnimations = new Dictionary<GameObject, Dictionary<int, Coroutine>>();
    private Dictionary<GameObject, Dictionary<int, OGAnimationTask>> activeAnimationTasks = new Dictionary<GameObject, Dictionary<int, OGAnimationTask>>();

    public void OnDisable()
    {
        StopAllAnimations();
        StopAllCoroutines();
        activeAnimations.Clear();
        activeAnimationTasks.Clear();
        animationIDs.Clear();
    }

    private int animate(OGAnimationTask animationTask)
    {
        var go = animationTask.target.gameObject;
        if (!animationIDs.ContainsKey(go)) { animationIDs.Add(go, 0); }
        if (!activeAnimations.ContainsKey(go)) { activeAnimations.Add(go, new Dictionary<int, Coroutine>()); }
        if (!activeAnimationTasks.ContainsKey(go)) { activeAnimationTasks.Add(go, new Dictionary<int, OGAnimationTask>()); }

        if (IsEditorPreviewMode())
        {
            animationIDs[go] = OGEditorCoroutines.GetNextCoroutineID();
            var animationID = animationIDs[go];
            activeAnimationTasks[go].Add(animationID, animationTask);
            OGEditorCoroutines.Run(animateCoroutine(animationID, animationTask));
            activeAnimations[go].Add(animationID, null);
            return animationID;
        }
        else
        {
            var animationID = ++animationIDs[go];
            activeAnimationTasks[go].Add(animationID, animationTask);
            var coroutine = StartCoroutine(animateCoroutine(animationID, animationTask));
            activeAnimations[go].Add(animationID, coroutine);
            return animationID;
        }
    }

    [System.Serializable]
    public class RunningResult
    {
        public enum EndState
        {
            StillRunning = 0, Completed = 1, FadedOut = 2, Terminated = 3
        }

        public float duration;
        public float time;
        public EndState endState;

        public float lastEditorTime;
        public bool delayProcessed;
        public bool animationOverruled;

        public bool fadeOutAnimation;
        public float fadeOutEffect;

        public bool fadeInAnimation;
        public float fadeInEffect;
        public float fadeInSpeed;
    }

    // TODO let animator run over all currently active animations but when multiple are active on one object, let the effects STACK
    // so that an In animation under a fading in Idle will always produce smooth results, and an Out running over an Idle as well

    private IEnumerator animateCoroutine(int animationID, OGAnimationTask task)
    {
        var run = new RunningResult();
        task.runningResult = run;

        run.duration = task.animation.Duration;
        run.time = task.randomStartTime ? UnityEngine.Random.Range(0, run.duration) : 0f;
        run.endState = RunningResult.EndState.StillRunning;

        run.delayProcessed = task.delay == 0f;
        run.lastEditorTime = OGEditorCoroutines.Time;
        run.animationOverruled = false;

        run.fadeOutAnimation = false;
        run.fadeOutEffect = 1f;
        run.fadeInAnimation = task.fadeIn > 0f || activeAnimations[task.target.gameObject].Keys.Count > 0;
        run.fadeInEffect = run.fadeInAnimation ? 0f : 1f;
        run.fadeInSpeed = Mathf.Min(animationFadeOutSpeed, 1f / (task.fadeIn + 0.000001f));

        // only continue if gameobject is not destroyed
        if (task.target.gameObject == null) { yield break; }

        // begin animation
        task.animation.GetAnimationStartState(task.origin).ApplyTo(task.target, task.mask);

        while (true)
        {
            yield return null;
            // only continue if gameobject is not destroyed
            if (task.target.gameObject == null) { yield break; }

            run.duration = task.animation.Duration;
            float deltaTime;

            // handle timing depending on editor mode or play
            if (IsEditorPreviewMode())
            {
                var nowEditorTime = OGEditorCoroutines.Time;
                deltaTime = nowEditorTime - run.lastEditorTime;
                run.lastEditorTime = nowEditorTime;
            }
            else
            {
                deltaTime = task.unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            }

            // handle fade in
            if (run.fadeInEffect < 1f)
            {
                run.fadeInEffect += deltaTime * run.fadeInSpeed;
                run.fadeInEffect = Mathf.Min(run.fadeInEffect, 1f);
            }

            // handle fade out
            if (run.fadeOutAnimation)
            {
                run.fadeOutEffect -= deltaTime * animationFadeOutSpeed;
                if (run.fadeOutEffect <= 0f)
                {
                    // --  end animation due to fade out
                    HandleAnimationEnd(animationID, task, RunningResult.EndState.FadedOut);
                    yield break;
                }
            }
            else
            {
                run.fadeOutAnimation = animationID != animationIDs[task.target.gameObject];
                run.animationOverruled = animationID != animationIDs[task.target.gameObject];
            }

            // determine new time value
            run.time += deltaTime;

            // process delay
            if (!run.delayProcessed)
            {
                // keep setting the orinal state
                task.animation.GetAnimationStartState(task.origin).ApplyTo(task.target, task.mask);

                if (run.time < task.delay)
                {
                    continue;
                }
                else
                {
                    // delay was finished, so let's subtract it from time and start animating
                    run.delayProcessed = true;
                    run.time -= task.delay;
                }
            }

            // check if we are finished
            if (run.time >= run.duration)
            {
                if (task.loop == OGLoopType.None) break;
                else if (task.loop == OGLoopType.Repeat) run.time = 0f;
            }

            // animation step
            var totalFade = run.fadeInEffect; // Mathf.Min(run.fadeInEffect, run.fadeOutEffect);
            var goalState = task.animation.EvaluateAnimationAtTime(run.time, task.origin);
            if (totalFade < 1f - Mathf.Epsilon)
            {
                var lerpedState = OGAnimationState.Lerp(task.origin, goalState, totalFade);
                lerpedState.ApplyTo(task.target, task.mask, totalFade, new OGAnimationState(task.target));
            }
            else
            {
                goalState.ApplyTo(task.target, task.mask);
            }

            task.OnTimeUpdated();
        }

        // --  end animation
        task.animation.GetAnimationEndState(task.origin).ApplyTo(
            task.target, task.mask, run.fadeInEffect);// Mathf.Min(run.fadeInEffect, run.fadeOutEffect));

        HandleAnimationEnd(animationID, task,
            run.fadeOutAnimation ? RunningResult.EndState.FadedOut : RunningResult.EndState.Completed);
    }

    private void HandleAnimationEnd(int animationID, OGAnimationTask task, RunningResult.EndState reason)
    {
        if (task == null) return;
        if (task.target != null)
        {
            activeAnimations[task.target.gameObject].RemoveIfContainsKey(animationID);
            activeAnimationTasks[task.target.gameObject].RemoveIfContainsKey(animationID);
        }

        task.OnAnimationEnded(reason);
    }
}
