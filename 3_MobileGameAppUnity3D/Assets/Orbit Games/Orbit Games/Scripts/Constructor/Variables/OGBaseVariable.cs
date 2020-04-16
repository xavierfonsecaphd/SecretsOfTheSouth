using GameToolkit.Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OGBaseVariable : MonoBehaviour
{
    [Header("Variable")]
    public OGBaseVariableDefinition definition;
    
    public abstract string ToDisplayValue();
    public abstract string ToStoredValue();
    public abstract void FromStoredValue(string stringValue);
    public abstract void ResetToDefault();
    protected abstract void OnValueUpdate();
}

public abstract class OGBaseVariable<TValue>: OGBaseVariable
{
    public TValue value;

    private void Awake()
    {
        OnValueUpdate();
    }

    public void SetValue(TValue value)
    {
        this.value = value;
        OnValueUpdate();
    }

    public override string ToStoredValue()
    {
        return JsonUtility.ToJson(value);
    }

    public static string ToStoredValue(TValue value)
    {
        if (typeof(TValue) == typeof(string))
        {
            return value as string;
        }
        else if (typeof(TValue) == typeof(bool))
        {
            return ((bool)(object)value) ? bool.TrueString : bool.FalseString;
        }
        return JsonUtility.ToJson(value);
    }

    public override void FromStoredValue(string stringValue)
    {
        if (typeof(TValue) == typeof(string))
        {
            value = (TValue)(object)stringValue;
        }
        else if(typeof(TValue) == typeof(bool))
        {
            value = (TValue)(object)bool.Parse(stringValue);
        }
        else
        {
            value = JsonUtility.FromJson<TValue>(stringValue);
        }
        OnValueUpdate();
    }
}
