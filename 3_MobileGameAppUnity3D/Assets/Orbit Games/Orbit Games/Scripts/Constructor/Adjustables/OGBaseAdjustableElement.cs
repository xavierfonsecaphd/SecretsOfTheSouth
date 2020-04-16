using GameToolkit.Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AdjustableType
{
    Continuous, Enum, Bool
}

public abstract class OGBaseAdjustableElement : MonoBehaviour
{
    [Header("Adjustable")]
    public OGAdjustableDefinition definition;

    public abstract string ToDisplayValue(float value);
    public abstract float GetT();
    public abstract float[] GetTs();
    public abstract void SetT(float value);
    public abstract void ResetToDefault();
    public abstract AdjustableType GetAdjustableType();

    public string ToStoredValue()
    {
        return GetT().ToString("G9", System.Globalization.CultureInfo.InvariantCulture);
    }

    public void FromStoredValue(string stringFloat)
    {
        if (stringFloat.IsNullOrEmpty())
        {
            ResetToDefault();
            return;
        }
        SetT(float.Parse(stringFloat, System.Globalization.CultureInfo.InvariantCulture));
    }
}
