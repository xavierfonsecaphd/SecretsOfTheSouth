using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OGAnimationExtensions
{

    public static void SetActiveWithTransition(this GameObject gameObject, bool active, string tag = "", float speed = 1f)
    {
        var transitionable = gameObject.GetComponent<OGTransitionable>();
        if (transitionable == null)
        {
            Debug.LogWarning("GameObject itself does not have a OGTransitionable component, therefore we just set its active state directly");
            gameObject.SetActive(active);
        }
        else if (!transitionable.animatedProperties.active)
        {
            Debug.LogWarning("GameObject itself does not have its active state animated in its Transitionable object");
        }
        else
        {
            if (active)
            {
                OGTransition.In(gameObject, tag, speed);
            }
            else
            {
                OGTransition.Out(gameObject, tag, speed);
            }
        }
    }

    public static void SmoothCurve(this AnimationCurve animationCurve)
    {
        for (int k = 0; k < animationCurve.keys.Length; k++)
        {
            animationCurve.SmoothTangents(k, 0);
        }
    }

    /// <summary>
    /// Creates a curve that approximates the XY flip of a curve. Unfortunately
    /// </summary>
    /// <param name="animationCurve"></param>
    /// <returns></returns>
    public static AnimationCurve FlipXY(this AnimationCurve animationCurve)
    {
        if (animationCurve.keys.Length < 2)
        {
            return new AnimationCurve(animationCurve.keys);
        }

        var approximatingFrames = new List<Keyframe>();
        var previousValue = animationCurve.keys[0].value;
        for (int t = 0; t < 51; t++)
        {
            var time = t / 50f;
            var value = animationCurve.Evaluate(time);
            var key = new Keyframe(value, time);
            approximatingFrames.Add(key);

            if (value < previousValue)
            {
                throw new System.Exception("This method does not support curves with decreasing values");
            }

            previousValue = value;
        }

        AnimationCurve backupCurve = new AnimationCurve(approximatingFrames.ToArray());
        AnimationCurve newCurve = new AnimationCurve(approximatingFrames.ToArray());
        backupCurve.SmoothCurve();
        newCurve.SmoothCurve();

        // approximate the curve by removing and checking how strongly the curve has changed
        bool removedFrame = false;
        int maxIterations = 500;
        do
        {
            removedFrame = false;
            for (int existingFrameIndex = 1; existingFrameIndex < newCurve.keys.Length - 1; existingFrameIndex++)
            {
                var testingFrame = newCurve.keys[existingFrameIndex];
                newCurve.RemoveKey(existingFrameIndex);

                var variance = newCurve.CalculateMaxVariance(backupCurve);

                if (variance >= 0f && variance <= 0.05f)
                {
                    // removal was ok!
                    removedFrame = true;
                    existingFrameIndex--;
                }
                else
                {
                    // removal was NOT ok..
                    newCurve.AddKey(testingFrame);
                }
            }
        } while (removedFrame && maxIterations-- > 0);

        if (maxIterations <= 0)
        {
            Debug.LogError("Curve cleaning took way too long, canceled further execution");
        }

        //AnimationCurve newCurve = new AnimationCurve(newKeyframes.ToArray());
        //for (int k = 0; k < newCurve.keys.Length; k++)
        //{
        //    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(newCurve, k, UnityEditor.AnimationUtility.TangentMode.Linear);
        //    UnityEditor.AnimationUtility.SetKeyRightTangentMode(newCurve, k, UnityEditor.AnimationUtility.TangentMode.Linear);
        //}
        return newCurve;
    }

    public static float CalculateMaxVariance(this AnimationCurve fromCurve, AnimationCurve trueCurve)
    {
        List<float> times = new List<float>();
        float percentBetweenKeys = 1f / 0.25f;
        for (int k = 0; k < trueCurve.keys.Length - 1; k++)
        {
            var key1 = trueCurve.keys[k];
            var key2 = trueCurve.keys[k + 1];
            var timeDifference = key2.time - key1.time;
            for (float t = key1.time; t < key2.time; t += timeDifference * percentBetweenKeys)
            {
                times.Add(t);
            }
        }
        times.Add(trueCurve.keys[trueCurve.keys.Length - 1].time);

        var maxVariance = 0f;
        foreach (var time in times)
        {
            var eval1 = fromCurve.Evaluate(time) * 100f;
            var eval2 = trueCurve.Evaluate(time) * 100f;
            maxVariance = Mathf.Max(maxVariance, (eval1 - eval2) * (eval1 - eval2));
        }
        return maxVariance;
    }
}
