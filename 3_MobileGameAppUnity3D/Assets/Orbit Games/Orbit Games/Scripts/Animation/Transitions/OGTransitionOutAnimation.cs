using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OGTransitionOutAnimation : OGStateAnimation
{
    public override AnimationDirection AnimationDirectionSetting { get { return AnimationDirection.AwayFromOrigin; } }
    public override bool IsLoopingAnimation { get { return false; } }
    public override void DrawAnimationGizmos(Vector3 start, Vector3 end)
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(start, end);
        Gizmos.DrawSphere(end, 2f);
    }
    public override OGAnimationState PostProcessStateDifference(OGAnimationState stateDifference)
    {
        return stateDifference;
    }
    public override void InitializeAdditionally()
    {

    }
}