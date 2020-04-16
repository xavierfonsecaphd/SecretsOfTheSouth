using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OGBaseAdjustableDimension : OGBaseAdjustableValue<float>
{
    [Header("Dimension Settings")]
    public Dimension dimension;

    protected override float FromTToValue(float t)
    {
        if (!mapper.useValueMapping)
        {
            return min + (max - min) * t;
        }
        else
        {
            return min + (max - min) * mapper.Map(t);
        }
    }

    protected override float FromValueToT(float value)
    {
        if (max == min) return 0f;
        if (!mapper.useValueMapping)
        {
            return (value - min) / (max - min);
        }
        else
        {
            return mapper.ReverseMap((value - min) / (max - min));
        }
    }
}
