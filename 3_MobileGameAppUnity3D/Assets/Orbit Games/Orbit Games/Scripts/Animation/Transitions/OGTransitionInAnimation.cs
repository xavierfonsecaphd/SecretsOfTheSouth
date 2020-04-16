using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OGTransitionInAnimation : OGStateAnimation
{
    public enum PlayIdleBehaviour
    {
        DontPlayIdle, PlayIdleWithActiveTag, PlayIdleWithNewTag
    }

    [Header("Additional Settings")]
    [Range(0f, 1f)]
    public float fadeInIdleAtPercent = 0.8f;
    [ConditionalField(true, propertyToCheck: "playIdleBehaviour", compareValue: PlayIdleBehaviour.PlayIdleWithNewTag)]
    public string playIdleTag;
    public PlayIdleBehaviour playIdleBehaviour;

    public override AnimationDirection AnimationDirectionSetting { get { return AnimationDirection.IntoOrigin; } }
    public override bool IsLoopingAnimation { get { return false; } }
    public override void DrawAnimationGizmos(Vector3 start, Vector3 end)
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(start, end);
        Gizmos.DrawSphere(start, 2f);
    }
    public override OGAnimationState PostProcessStateDifference(OGAnimationState stateDifference)
    {
        return stateDifference;
    }
    public override void InitializeAdditionally()
    {
        fadeInIdleAtPercent = 0.8f;
    }
}
