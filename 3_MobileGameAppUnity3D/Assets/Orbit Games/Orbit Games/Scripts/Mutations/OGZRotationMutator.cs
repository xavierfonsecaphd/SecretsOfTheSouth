using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OGZRotationMutator : OGBaseMutator<float> {

    protected override void ApplyValue(float value)
    {
        var temp = transform.localEulerAngles;
        temp.z = value;
        transform.localEulerAngles = temp;
    }

    protected override float Mutate()
    {
        return startValue + Random.Range(-1f, 1f) * mutateAmount;
    }

    protected override float ReadValue()
    {
        return transform.localEulerAngles.z;
    }
}
