using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OGRegularAnimation : OGStateAnimation
{
    [Header("Additional Settings")]
    public float fadeInTime = 5f;

    public override bool IsLoopingAnimation { get { return true; } }
    public override AnimationDirection AnimationDirectionSetting { get { return AnimationDirection.AwayFromOrigin; } }

    public override void DrawAnimationGizmos(Vector3 start, Vector3 end)
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(start, end);
        Gizmos.DrawSphere(end, 2f);

        var diff = end - start;

        var topRightFront = diff;
        var topLeftFront = diff;
        topLeftFront.x *= -1f;
        var bottomRightFront = topRightFront;
        bottomRightFront.y *= -1f;
        var bottomLeftFront = topLeftFront;
        bottomLeftFront.y *= -1f;
        var topRightBack = topRightFront;
        topRightBack.z *= -1f;
        var topLeftBack = topLeftFront;
        topLeftBack.z *= -1f;
        var bottomRightBack = bottomRightFront;
        bottomRightBack.z *= -1f;
        var bottomLeftBack = bottomLeftFront;
        bottomLeftBack.z *= -1f;

        topLeftFront += start;
        topRightFront += start;
        bottomLeftFront += start;
        bottomRightFront += start;
        topLeftBack += start;
        topRightBack += start;
        bottomLeftBack += start;
        bottomRightBack += start;

        Gizmos.DrawLine(topLeftFront, topRightFront);
        Gizmos.DrawLine(topRightFront, topRightBack);
        Gizmos.DrawLine(topRightBack, topLeftBack);
        Gizmos.DrawLine(topLeftBack, topLeftFront);

        Gizmos.DrawLine(bottomLeftFront, bottomRightFront);
        Gizmos.DrawLine(bottomRightFront, bottomRightBack);
        Gizmos.DrawLine(bottomRightBack, bottomLeftBack);
        Gizmos.DrawLine(bottomLeftBack, bottomLeftFront);

        Gizmos.DrawLine(topLeftFront, bottomLeftFront);
        Gizmos.DrawLine(topRightFront, bottomRightFront);
        Gizmos.DrawLine(topRightBack, bottomRightBack);
        Gizmos.DrawLine(topLeftBack, bottomLeftBack);
    }

    public override void InitializeAdditionally()
    {
        fadeInTime = 0f;
    }

    public override OGAnimationState PostProcessStateDifference(OGAnimationState stateDifference)
    {
        return stateDifference;
    }
}
