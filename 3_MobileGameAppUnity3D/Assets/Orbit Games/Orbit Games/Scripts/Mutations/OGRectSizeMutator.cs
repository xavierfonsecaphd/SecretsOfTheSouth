using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OGRectSizeMutator : OGBaseMutator<Vector2> {

    protected override void ApplyValue(Vector2 value)
    {
        ((RectTransform)transform).sizeDelta = value;
    }

    protected override Vector2 Mutate()
    {
        return startValue + Random.Range(-1f, 1f) * mutateAmount;
    }

    protected override Vector2 ReadValue()
    {
        return ((RectTransform)transform).sizeDelta;
    }
}
