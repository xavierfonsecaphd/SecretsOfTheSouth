using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OGAdjustableScale : OGBaseAdjustableDimension
{
    public override string ToDisplayValue(float value)
    {
        return value.toNumberString(2) + " x";
    }

    protected override float ReadValue()
    {
        switch (dimension)
        {
            case Dimension.X:
                return transform.localScale.x;
            case Dimension.Y:
                return transform.localScale.y;
            case Dimension.Z:
                return transform.localScale.z;
            default:
                return 0f;
        }
    }

    protected override void SetValue(float value)
    {
        var p = transform.localScale;
        switch (dimension)
        {
            case Dimension.X:
                transform.localScale = new Vector3(value, p.y, p.z);
                break;
            case Dimension.Y:
                transform.localScale = new Vector3(p.x, value, p.z);
                break;
            case Dimension.Z:
                transform.localScale = new Vector3(p.x, p.y, value);
                break;
            default:
                break;
        }
    }
}
