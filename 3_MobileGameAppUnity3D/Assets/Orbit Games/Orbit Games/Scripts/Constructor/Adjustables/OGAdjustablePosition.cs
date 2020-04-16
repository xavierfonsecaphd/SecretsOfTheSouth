using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OGAdjustablePosition : OGBaseAdjustableDimension
{
    public override string ToDisplayValue(float value)
    {
        return value.toNumberString(2) + " pt";
    }

    protected override float ReadValue()
    {
        switch (dimension)
        {
            case Dimension.X:
                return transform.localPosition.x;
            case Dimension.Y:
                return transform.localPosition.y;
            case Dimension.Z:
                return transform.localPosition.z;
            default:
                return 0f;
        }
    }

    protected override void SetValue(float value)
    {
        var p = transform.localPosition;
        switch (dimension)
        {
            case Dimension.X:
                transform.localPosition = new Vector3(value, p.y, p.z);
                break;
            case Dimension.Y:
                transform.localPosition = new Vector3(p.x, value, p.z);
                break;
            case Dimension.Z:
                transform.localPosition = new Vector3(p.x, p.y, value);
                break;
            default:
                break;
        }
    }
}
