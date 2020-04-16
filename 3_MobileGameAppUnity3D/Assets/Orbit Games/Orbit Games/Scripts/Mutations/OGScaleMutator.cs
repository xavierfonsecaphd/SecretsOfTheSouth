using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OGScaleMutator : OGBaseMutator<Vector3> {

    protected override void ApplyValue(Vector3 value)
    {
        transform.localScale = value;
    }

    protected override Vector3 Mutate()
    {
        return startValue + Random.Range(-1f, 1f) * mutateAmount;
    }

    protected override Vector3 ReadValue()
    {
        return transform.localScale;
    }
}
