using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class OGTransitionable : OGBaseAnimatableBehaviour
{
    public enum OnEnableAction
    {
        Nothing, PlayInTransitionWithTag, ReadyInTransitionWithTag, PlayIdleWithTag
    }

    public enum PhasePlayBehaviour
    {
        OnlyPlayInPhase, PlayAnyRequestedAnimation
    }

    public enum Phase
    {
        WaitingForEntrance, TransitioningIn, WaitingForAction, Idling, TransitioningOut, WaitingForRestart
    }
    
    public override void PlayDemo()
    {
        StopActiveAnimations();
        var thisDemoID = ++demoID;
        TransitionIn(activeTag, 1f, true);
        OGRun.Delayed(5f, () =>
        {
            if (thisDemoID == demoID)
                TransitionOut(null);
        });
    }

    protected override List<EditorButton> GetPlayTestButtons()
    {
        List<EditorButton> buttons = new List<EditorButton>();
        buttons.Add(new EditorButton("► In", "TestTransitionIn"));
        buttons.Add(new EditorButton("► Idle", "TestIdleAnimation"));
        buttons.Add(new EditorButton("► Out", "TestTransitionOut"));
        return buttons;
    }

    public void TestTransitionIn() { TransitionIn(activeTag, 1f, true, true); }
    public void TestIdleAnimation() { PlayIdleAnimation(activeTag, 1f, true, true); }
    public void TestTransitionOut() { TransitionOut(activeTag, 1f, true, true); }

    [Header("Phase")]
    [ReadOnly]
    public Phase activePhase = Phase.WaitingForEntrance;
    [EnumButtons(1)]
    public PhasePlayBehaviour phaseBehaviour;

    [Header("On Enable")]
    [ConditionalField(true, "onEnableDo", false, OnEnableAction.Nothing)]
    public string onEnableTag;
    [EnumButtons(1)]
    public OnEnableAction onEnableDo = OnEnableAction.PlayInTransitionWithTag;

    [Header("Animations")]
    public AnimationsPresetOrCustom Animations;
    [System.Serializable]
    public class AnimationsPresetOrCustom : OGPresetOrCustom<OGTransitionableAnimationsPreset, OGTransitionableAnimations> { }

    [Header("Events")]
    public Events events;
    public EditorHelpBox todoBox;

    [System.Serializable]
    public class Events
    {
        public StringEvent onTransitionInStart;
        public StringEvent onTransitionInEnd;
        public StringEvent onIdleAnimationStart;
        public StringEvent onIdleAnimationEnd;
        public StringEvent onTransitionOutStart;
        public StringEvent onTransitionOutEnd;
    }

    private void Awake()
    {
        activePhase = Phase.WaitingForEntrance;
    }

    private void OnEnable()
    {
        OGRun.LateUpdate(() =>
        {
            switch (onEnableDo)
            {
                case OnEnableAction.Nothing:
                    break;
                case OnEnableAction.PlayInTransitionWithTag:
                    TransitionIn(onEnableTag, 1f, true);
                    break;
                case OnEnableAction.ReadyInTransitionWithTag:
                    ForceRestart(onEnableTag);
                    break;
                case OnEnableAction.PlayIdleWithTag:
                    PlayIdleAnimation(onEnableTag, 1f, true);
                    break;
            }
        });
    }

    public void ForceRestart(string tag = "")
    {
        if (tag == null)
            tag = activeTag;

        activePhase = Phase.WaitingForEntrance;
        OGAnimator.StopAllAnimations(gameObject);
        var transition = Animations.Definition.GetInTransitionWithTag(tag);
        if (transition != null) transition.GetAnimationStartState(origin).ApplyTo(animationTargets, animatedProperties);
        if (startDisabled && animationTargets.gameObject) animationTargets.gameObject.SetActive(false);
    }

    public void ForceEnd(string tag = "")
    {
        if (tag == null)
            tag = activeTag;

        activePhase = Phase.WaitingForRestart;
        OGAnimator.StopAllAnimations(gameObject);
        var transition = Animations.Definition.GetInTransitionWithTag(tag);
        if (transition != null) transition.GetAnimationEndState(origin).ApplyTo(animationTargets, animatedProperties);
        if (endDisabled && animationTargets.gameObject) animationTargets.gameObject.SetActive(false);
    }

    public void TransitionIn(string tag = "", float speed = 1f, bool ignorePhaseBehaviour = false, bool forceRepeat = false)
    {
        // check if our phase allows for a transitioning in
        if (!ignorePhaseBehaviour
            && phaseBehaviour == PhasePlayBehaviour.OnlyPlayInPhase
            && activePhase != Phase.WaitingForEntrance
            && activePhase != Phase.TransitioningOut
            && activePhase != Phase.WaitingForRestart)
            return;

        if (tag == null)
            tag = activeTag;

        activeTag = tag;
        activePhase = Phase.TransitioningIn;

        var animation = Animations.Definition.GetInTransitionWithTag(tag);
        if (animation == null) return;

        // not sure if necessary, because gameobject will automatically be activated due to animation (unless 'active' is not animated)
        if (!animationTargets.gameObject.activeSelf)
            animationTargets.gameObject.SetActive(true);

        events.onTransitionInStart.Invoke(tag);
        var task = ProcessAndPlayAnimationSetup(animation, speed, forceRepeat ? OGLoopType.Repeat : OGLoopType.None, false, 0f,
            (OGAnimator.RunningResult.EndState reason) =>
            {
                if (reason == OGAnimator.RunningResult.EndState.Completed)
                {
                    events.onTransitionInEnd.Invoke(tag);
                    activePhase = Phase.WaitingForAction;
                }
            }
        );

        if (!forceRepeat && animation.playIdleBehaviour != OGTransitionInAnimation.PlayIdleBehaviour.DontPlayIdle)
        {
            task.AddTrigger(animation.fadeInIdleAtPercent, () =>
            {
                if (animation.playIdleBehaviour == OGTransitionInAnimation.PlayIdleBehaviour.PlayIdleWithActiveTag)
                {
                    PlayIdleAnimation(activeTag);
                }
                else if (animation.playIdleBehaviour == OGTransitionInAnimation.PlayIdleBehaviour.PlayIdleWithNewTag)
                {
                    PlayIdleAnimation(animation.playIdleTag);
                }
            });
        }
    }

    public void PlayIdleAnimation(string tag = "", float speed = 1f, bool ignorePhaseBehaviour = false, bool forceRepeat = false)
    {
        // check if our phase allows for idling
        if (!ignorePhaseBehaviour
            && phaseBehaviour == PhasePlayBehaviour.OnlyPlayInPhase
            && activePhase != Phase.WaitingForEntrance
            && activePhase != Phase.TransitioningIn
            && activePhase != Phase.WaitingForAction)
            return;

        if (tag == null)
            tag = activeTag;

        activeTag = tag;
        activePhase = Phase.Idling;

        var animation = Animations.Definition.GetIdleAnimationWithTag(tag);
        if (animation == null) return;

        events.onIdleAnimationStart.Invoke(tag);
        ProcessAndPlayAnimationSetup(animation, speed, forceRepeat ? OGLoopType.Repeat : animation.loopType, animation.randomStartTime, animation.fadeInTime,
            (OGAnimator.RunningResult.EndState reason) =>
            {
                if (reason == OGAnimator.RunningResult.EndState.Completed)
                {
                    events.onIdleAnimationEnd.Invoke(tag);
                    activePhase = Phase.WaitingForAction;
                }
            }
        );
    }

    public void TransitionOut(string tag = "", float speed = 1f, bool ignorePhaseBehaviour = false, bool forceRepeat = false)
    {
        // check if our phase allows for a transitioning out
        if (!ignorePhaseBehaviour
            && phaseBehaviour == PhasePlayBehaviour.OnlyPlayInPhase
            && activePhase != Phase.TransitioningIn
            && activePhase != Phase.WaitingForAction
            && activePhase != Phase.Idling)
            return;
        
        if (tag == null)
            tag = activeTag;

        activeTag = tag;
        activePhase = Phase.TransitioningOut;

        var animation = Animations.Definition.GetOutTransitionWithTag(tag);
        if (animation == null) return;

        events.onTransitionOutStart.Invoke(tag);
        ProcessAndPlayAnimationSetup(animation, speed, forceRepeat ? OGLoopType.Repeat : OGLoopType.None, false, 0f,
            (OGAnimator.RunningResult.EndState reason) =>
            {
                activePhase = Phase.WaitingForRestart;
                if (reason == OGAnimator.RunningResult.EndState.Completed)
                {
                    ForceRestart();
                    events.onTransitionOutEnd.Invoke(tag);

                    if (endDisabled && animationTargets.gameObject)
                        animationTargets.gameObject.SetActive(false);

                    if (Application.isPlaying)
                    {
                        if (endInPool && animationTargets.gameObject)
                            OGPool.removeCopy(animationTargets.gameObject);
                    }
                }
            }
        );
    }

    public void Reset()
    {
        if (Animations == null) Animations = new AnimationsPresetOrCustom();
        Animations.presetDefinition = OGTransitionableDefaults.GetTransitionableAnimationSetPreset();
    }

    protected override void OnEditorUpdate()
    {
        var todo = "####### TODO ####### \n";
        todo += "- Add prefix tag\n";
        todo += "- Let animations run stackwise\n";
        todo += "- Transition calling from script\n";
        todo += "- Allow playing audio as well\n";
        todoBox = EditorHelpBox.Info(todo);

        if (Animations == null || Animations.Definition == null)
        {
            errorMessage = EditorHelpBox.Error("No animations were found attached to this object");
            return;
        }

        Animations.Definition.Initialize();

        if (Animations.Definition.inTransitions.Length == 0 && Animations.Definition.idleAnimations.Length == 0 && Animations.Definition.outTransitions.Length == 0)
        {
            errorMessage = EditorHelpBox.Error("There seem to be no animations defined.");
            return;
        }

        var noError = Animations.Definition.OnGizmoUpdate(this);

        if (!noError)
        {
            errorMessage = EditorHelpBox.Error("There seems to be a problem with one of the included animations.");
            return;
        }
    }
}
