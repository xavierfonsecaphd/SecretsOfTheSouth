using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OGBaseAdjustableValue<Value> : OGBaseSlidableElement
{
    [Header("Settings")]
    public Value min;
    public Value max;
    public Value defaultValue;
    public ValueMapper mapper;

    [Buttons("● Min", "WriteMIN", "● Default", "WriteDEFAULT", "● Max", "WriteMAX")]
    public ButtonsContainer save;

    [Buttons("► Min", "ReadMIN", "► Default", "ReadDEFAULT", "► Max", "ReadMAX")]
    public ButtonsContainer apply;

    [Header("State")]
    [ReadOnly]
    public float currentT;
    [ReadOnly]
    public Value currentValue;

    protected abstract void SetValue(Value value);
    protected abstract Value ReadValue();
    protected abstract Value FromTToValue(float t);
    protected abstract float FromValueToT(Value value);

    public void ReadMIN() { SetValue(min); }
    public void ReadDEFAULT() { SetValue(defaultValue); }
    public void ReadMAX() { SetValue(max); }
    public void WriteMIN() { min = ReadValue(); }
    public void WriteDEFAULT() { defaultValue = ReadValue(); }
    public void WriteMAX() { max = ReadValue(); }

    private static float[] ts;

    private void Initialize()
    {
        if (ts == null || ts.Length == 0)
        {
            ts = new float[51];
            for (int i = 0; i < 51; i++)
            {
                ts[i] = (float)i / 50;
            }
        }
    }

    public override void ResetToDefault()
    {
        SetT(FromValueToT(defaultValue));
    }

    public override string GetMaxDisplayValue()
    {
        return ToDisplayValue(FromValueToT(min));
    }

    public override string GetMinDisplayValue()
    {
        return ToDisplayValue(FromValueToT(max));
    }

    public override float GetT()
    {
        return currentT;
    }

    public override void SetT(float t)
    {
        currentT = t;
        currentValue = FromTToValue(t);
        SetValue(currentValue);
    }

    public override float[] GetTs()
    {
        Initialize();
        return ts;
    }

    public override AdjustableType GetAdjustableType()
    {
        return AdjustableType.Continuous;
    }
}
