using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Curve", menuName = "Animation/Curve preset", order = 1)]
public class OGAnimationCurvePreset : ScriptableObject {
    [Height(100f)]
    public AnimationCurve curve;

    [Header("Generation Tools")]
    [Buttons("Flip XY", "FlipXY", "Generate", "Generate", "Normalize Time", "NormalizeTime")]
    public ButtonsContainer helpers;
    public int timestamps = 15;
    public float minY = -1f;
    public float maxY = 1f;
    public float minValueDifference = 1f;
    public float timeJiggle = 0.5f;
    public bool alternateDirection = true;

    public void FlipXY()
    {
        curve = curve.FlipXY();
    }

    public void NormalizeTime()
    {
        var keys = new Keyframe[curve.keys.Length];
        curve.keys.CopyTo(keys, 0);

        float maxTime = 0f;
        for (int i = 0; i < keys.Length; i++)
        {
            maxTime = Mathf.Max(maxTime, keys[i].time);
        }

        if (maxTime == 0f) return;
        if (curve.postWrapMode == WrapMode.PingPong)
            maxTime *= 2f;

        for (int i = 0; i < keys.Length; i++)
        {
            keys[i].time /= maxTime;
        }
        
        var newCurve = new AnimationCurve(keys);
        newCurve.postWrapMode = curve.postWrapMode;
        newCurve.preWrapMode = curve.preWrapMode;
        curve = newCurve;
    }

    public void Generate()
    {
        curve = new AnimationCurve();

        var goUp = true;
        var lastValue = -1f;
        var time = 0f;

        for (int i = 0; i < timestamps; i++)
        {
            var newValue = 0f;

            if (!alternateDirection)
            {
                newValue = Random.Range(minY, maxY);
                while (lastValue - minValueDifference < newValue && newValue < lastValue + minValueDifference)
                {
                    newValue = Random.Range(minY, maxY);
                }
            }
            else
            {
                if (goUp)
                {
                    newValue = Random.Range(lastValue + minValueDifference, maxY);
                }
                else
                {
                    newValue = Random.Range(minY, lastValue - minValueDifference);
                }
            }

            curve.AddKey(new Keyframe(time, newValue, 0, 0));

            var jiggle = Random.Range(-timeJiggle * 0.5f, timeJiggle * 0.5f);
            time = time + 1f + jiggle;

            goUp = newValue < lastValue;
            lastValue = newValue;
        }
        
        curve.postWrapMode = WrapMode.PingPong;
        curve.preWrapMode = WrapMode.PingPong;
    }
}
