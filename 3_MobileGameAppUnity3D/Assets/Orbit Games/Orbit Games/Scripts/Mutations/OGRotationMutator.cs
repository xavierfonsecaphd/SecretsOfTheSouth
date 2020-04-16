using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OGRotationMutator : OGBaseMutator<Vector3> {

    protected override void ApplyValue(Vector3 value)
    {
        transform.localEulerAngles = value;
    }

    protected override Vector3 Mutate()
    {
        return startValue + Random.Range(-1f, 1f) * mutateAmount;
    }

    protected override Vector3 ReadValue()
    {
        return transform.localEulerAngles;
    }
}
