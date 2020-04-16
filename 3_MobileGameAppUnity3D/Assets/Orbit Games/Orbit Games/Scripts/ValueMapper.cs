using System;
using UnityEngine;

[Serializable]
public class ValueMapper
{
    public bool useValueMapping;
    [Height(50f)]
    public AnimationCurve valueMapping;
    [Buttons("FlipCurve", "FlipCurve")]
    public ButtonsContainer flip;
    [Height(50f)]
    public AnimationCurve reverseMapping;

    public float Map(float value)
    {
        return valueMapping.Evaluate(value);
    }

    public float ReverseMap(float value)
    {
        return reverseMapping.Evaluate(value);
    }

    public void FlipCurve()
    {
        reverseMapping = valueMapping.FlipXY();
    }
}