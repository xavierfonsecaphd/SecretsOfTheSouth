using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OGAdjustableRotation : OGBaseAdjustableDimension
{
    public override string ToDisplayValue(float value)
    {
        return value + "°";
    }

    protected override float ReadValue()
    {
        switch (dimension)
        {
            case Dimension.X:
                return transform.localEulerAngles.x;
            case Dimension.Y:
                return transform.localEulerAngles.y;
            case Dimension.Z:
                return transform.localEulerAngles.z;
            default:
                return 0f;
        }
    }

    protected override void SetValue(float value)
    {
        var p = transform.localEulerAngles;
        switch (dimension)
        {
            case Dimension.X:
                transform.localEulerAngles = new Vector3(value, p.y, p.z);
                break;
            case Dimension.Y:
                transform.localEulerAngles = new Vector3(p.x, value, p.z);
                break;
            case Dimension.Z:
                transform.localEulerAngles = new Vector3(p.x, p.y, value);
                break;
            default:
                break;
        }
    }
}