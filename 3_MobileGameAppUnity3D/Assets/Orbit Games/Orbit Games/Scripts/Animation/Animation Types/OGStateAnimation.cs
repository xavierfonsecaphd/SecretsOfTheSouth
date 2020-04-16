using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class OGStateAnimation : IOGStateAnimation, IOGInitializable
{
    [QuickButtons(false)]
    public ButtonsContainer testButtons;
    [Buttons("● Set diff", "RememberChange", "◀ Return", "ShowChange", "▶ Play", "PlayAnimation", "■ Stop", "StopActiveAnimation")]
    public ButtonsContainer recorder;
    public EditorHelpBox errorMessage = EditorHelpBox.None();
    public EditorHelpBox changeMessage = EditorHelpBox.None();

    public abstract bool IsLoopingAnimation { get; }
    public abstract AnimationDirection AnimationDirectionSetting { get; }
    public abstract OGAnimationState PostProcessStateDifference(OGAnimationState stateDifference);
    public abstract void DrawAnimationGizmos(Vector3 start, Vector3 end);
    public abstract void InitializeAdditionally();

    [HideInInspector]
    public bool initialized = false;

    public OGStateAnimation() { }

    public enum AnimationDirection
    {
        IntoOrigin, AwayFromOrigin
    }

    [Header("Reference")]
    [Unit("@")]
    public string tag;

    [Header("Timing")]
    [Unit("seconds")]
    public float baseDelay;
    [Unit("seconds")]
    public float childIndexDelay;
    [Unit("seconds")]
    public float duration;
    [ConditionalField("IsLoopingAnimation", compareValue: true)]
    public OGLoopType loopType = OGLoopType.Endless;
    [ConditionalField("IsLoopingAnimation", compareValue: true)]
    public bool randomStartTime = true;

    [Header("Animation")]
    public bool autoOriginOnAnimTrigger = false;
    public OGAnimationState valueDifference;
    public OGAnimationCurvesPreset curves;

    [HideInInspector]
    public OGBaseAnimatableBehaviour animatable;

    public void EditorHelpBoxUpdate() { }

    public void Initialize()
    {
        if (initialized) return;
        initialized = true;
        duration = 1f;
        InitializeAdditionally();
    }

    public bool OnTransitionableGizmoUpdate(OGBaseAnimatableBehaviour animatable)
    {

        // manage error boxes
        changeMessage = EditorHelpBox.None();
        errorMessage = EditorHelpBox.None();

        this.animatable = animatable;

        if (valueDifference.IsEmpty() || valueDifference.IsNoChange())
        {
            errorMessage = EditorHelpBox.Error("Can't animate when no changes in value have been defined. Use the Save diff button to remember changes in value");
            return false;
        }

        if (curves == null)
        {
            errorMessage = EditorHelpBox.Error("Can't animate without having animation curves");
            return false;
        }

        if (duration <= 0f)
        {
            errorMessage = EditorHelpBox.Error("Animation has invalid duration, needs to be larger than 0");
            return false;
        }

        // draw gizmos
        // find endpoint positions
        var rememberState = new OGAnimationState(animatable.animationTargets);
        var startState = GetAnimationStartState(animatable.origin);
        startState.ApplyTo(animatable.animationTargets, animatable.animatedProperties);
        var startPosition = animatable.transform.localPosition;
        var endState = GetAnimationEndState(animatable.origin);
        endState.ApplyTo(animatable.animationTargets, animatable.animatedProperties);
        var endPosition = animatable.transform.localPosition;
        rememberState.ApplyTo(animatable.animationTargets, animatable.animatedProperties);

        DrawAnimationGizmos(startPosition, endPosition);

        // manage info boxes
        if ((AnimationDirectionSetting == AnimationDirection.AwayFromOrigin && rememberState != endState)
            || (AnimationDirectionSetting == AnimationDirection.IntoOrigin && rememberState != startState))
        {
            changeMessage = EditorHelpBox.Info("Object is currently not at this animation's diff point. You can set a new one at the object's current state, or return back to old set point");
            return true;
        }

        if (animatable == null)
        {
            errorMessage = EditorHelpBox.Warning("This animation can't be previewed, because it is not attached to a transitionable object");
            return true;
        }
        
        return true;
    }

    public void StopActiveAnimation(OGBaseAnimatableBehaviour animatable)
    {
        if (animatable == null)
        {
            Debug.LogError("Animation is not attached to an object, can't proceed to stop animation");
            return;
        }

        animatable.StopActiveAnimations();
    }

    public void PlayAnimation(OGBaseAnimatableBehaviour animatable)
    {
        if (animatable == null)
        {
            Debug.LogError("Animation is not attached to an object, can't proceed to play animation");
            return;
        }

        this.animatable = animatable;
        animatable.ProcessAndPlayAnimationSetup(this, 1f, IsLoopingAnimation ? loopType : OGLoopType.Repeat, randomStartTime, 0f);
    }

    public void RememberChange(OGBaseAnimatableBehaviour animatable)
    {
        if (animatable == null)
        {
            Debug.LogError("Animation is not attached to an object, can't proceed to save diff");
            return;
        }

        this.animatable = animatable;
        valueDifference = GetChange(animatable);
        valueDifference = PostProcessStateDifference(valueDifference);
    }

    private OGAnimationState GetChange(OGBaseAnimatableBehaviour animatable)
    {
        var sourceAbsolute = new OGAnimationState(animatable.animationTargets);
        return GetDifference(animatable.origin, sourceAbsolute);
    }

    private OGAnimationState GetDifference(OGAnimationState origin, OGAnimationState target)
    {
        return animatable.origin - target;
    }

    public void ShowChange(OGTransitionable transitionable)
    {
        if (transitionable == null)
        {
            Debug.LogError("Animation is not attached to an object, can't proceed to show diff");
            return;
        }

        this.animatable = transitionable;
        if (AnimationDirectionSetting == AnimationDirection.IntoOrigin)
        {
            GetAnimationStartState(transitionable.origin).ApplyTo(transitionable.animationTargets, transitionable.animatedProperties);
        }
        else
        {
            GetAnimationEndState(transitionable.origin).ApplyTo(transitionable.animationTargets, transitionable.animatedProperties);
        }
    }

    public OGAnimationState EvaluateAnimationAtTime(float time, OGAnimationState origin)
    {
        return OGAnimationState.Lerp(GetAnimationStartState(origin), GetAnimationEndState(origin), curves, time / duration);
    }

    public OGAnimationState GetAnimationStartState(OGAnimationState origin)
    {
        if (AnimationDirectionSetting == AnimationDirection.IntoOrigin)
        {
            return OGAnimationState.Subtract(origin, valueDifference);
        }
        else
        {
            return origin;
        }
    }

    public OGAnimationState GetAnimationEndState(OGAnimationState origin)
    {
        if (AnimationDirectionSetting == AnimationDirection.AwayFromOrigin)
        {
            return OGAnimationState.Subtract(origin, valueDifference);
        }
        else
        {
            return origin;
        }
    }

    public float Duration
    {
        get
        {
            return curves.GetMaxWarpedDuration(duration);
        }
    }
}
