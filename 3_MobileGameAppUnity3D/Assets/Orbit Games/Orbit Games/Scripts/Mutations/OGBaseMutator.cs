using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OGBaseMutator<TValue> : MonoBehaviour {

    public TValue startValue;
    public TValue mutateAmount;
    [Buttons("● To start", "Read", "► Show start", "Write")]
    public ButtonsContainer helpers;

    public void Read()
    {
        startValue = ReadValue();
    }

    public void Write()
    {
        ApplyValue(startValue);
    }

    protected abstract TValue ReadValue();
    protected abstract void ApplyValue(TValue value);
    protected abstract TValue Mutate();

    public void OnEnable()
    {
        ApplyValue(Mutate());
    }
}
