using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOGStateAnimation
{
    OGAnimationState EvaluateAnimationAtTime(float time, OGAnimationState origin);
    OGAnimationState GetAnimationStartState(OGAnimationState origin);
    OGAnimationState GetAnimationEndState(OGAnimationState origin);
    float Duration { get; }
}
