using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OGAnimatable : OGBaseAnimatableBehaviour
{
    public enum OnEnableAction
    {
        Nothing, PlayActiveTagAnimation, PlayAnimationWithTag, ReadyActiveTagAnimation, ReadyAnimationWithTag
    }

    public enum Phase
    {
        WaitingForEntrance, TransitioningIn, WaitingForAction, Idling, TransitioningOut, WaitingForRestart
    }

    public override void PlayDemo()
    {
        StopActiveAnimations();
        TestAnimationNormal();
    }

    protected override List<EditorButton> GetPlayTestButtons()
    {
        List<EditorButton> buttons = new List<EditorButton>();
        buttons.Add(new EditorButton("► Loop Endless", "TestAnimationLoop"));
        buttons.Add(new EditorButton("► Play", "TestAnimationNormal"));
        return buttons;
    }

    public void TestAnimationLoop() { PlayAnimation(activeTag, 1f, true, true); }
    public void TestAnimationNormal() { PlayAnimation(activeTag, 1f, true); }

    [Header("Behaviour")]
    public bool blockStartWhenPlaying;
    [ConditionalField(true, "onEnableDo", false, OnEnableAction.Nothing)]
    public string onEnableTag;
    [EnumButtons(1)]
    public OnEnableAction onEnableDo = OnEnableAction.PlayAnimationWithTag;

    [Header("Animations")]
    public AnimationsPresetOrCustom Animations;
    [System.Serializable]
    public class AnimationsPresetOrCustom : OGPresetOrCustom<OGRegularAnimationsPreset, OGRegularAnimations> { }
    
    [Header("Events")]
    public Events events;
    public EditorHelpBox todoBox;

    [System.Serializable]
    public class Events
    {
        public StringEvent onAnimationStart;
        public StringEvent onAnimationEnd;
    }

    private void OnEnable()
    {
        OGRun.LateUpdate(() =>
        {
            switch (onEnableDo)
            {
                case OnEnableAction.Nothing:
                    break;
                case OnEnableAction.PlayAnimationWithTag:
                    PlayAnimation(onEnableTag, 1f, true);
                    break;
                case OnEnableAction.ReadyAnimationWithTag:
                    ForceRestart(onEnableTag);
                    break;
                case OnEnableAction.PlayActiveTagAnimation:
                    PlayAnimation(activeTag, 1f, true);
                    break;
                case OnEnableAction.ReadyActiveTagAnimation:
                    ForceRestart(activeTag);
                    break;
            }
        });
    }

    public void ForceRestart(string tag = "")
    {
        if (tag == null)
            tag = activeTag;
        
        OGAnimator.StopAllAnimations(gameObject);
        var transition = Animations.Definition.GetAnimationWithTag(tag);
        if (transition != null) transition.GetAnimationStartState(origin).ApplyTo(animationTargets, animatedProperties);
        if (startDisabled && animationTargets.gameObject) animationTargets.gameObject.SetActive(false);
    }

    public void ForceEnd(string tag = "")
    {
        if (tag == null)
            tag = activeTag;
        
        OGAnimator.StopAllAnimations(gameObject);
        var transition = Animations.Definition.GetAnimationWithTag(tag);
        if (transition != null) transition.GetAnimationEndState(origin).ApplyTo(animationTargets, animatedProperties);
        if (endDisabled && animationTargets.gameObject) animationTargets.gameObject.SetActive(false);
    }

    public void PlayAnimation(string tag = "", float speed = 1f, bool forceRestart = false, bool forceRepeat = false)
    {
        // check if our phase allows for idling
        if (blockStartWhenPlaying && !forceRestart && IsPlaying)
            return;

        if (tag == null)
            tag = activeTag;

        activeTag = tag;

        var animation = Animations.Definition.GetAnimationWithTag(tag);
        if (animation == null) return;

        events.onAnimationStart.Invoke(tag);
        if (!animationTargets.gameObject.activeSelf) animationTargets.gameObject.SetActive(true);
        ProcessAndPlayAnimationSetup(animation, speed, forceRepeat ? OGLoopType.Repeat : animation.loopType, animation.randomStartTime, 0f,
            (OGAnimator.RunningResult.EndState reason) =>
            {
                if (reason == OGAnimator.RunningResult.EndState.Completed)
                {
                    if (endDisabled && animationTargets.gameObject) animationTargets.gameObject.SetActive(false);
                    events.onAnimationEnd.Invoke(tag);
                }
            }
        );
    }

    public void Reset()
    {
        if (Animations == null) Animations = new AnimationsPresetOrCustom();
        Animations.presetDefinition = OGTransitionableDefaults.GetRegularAnimationSetPreset();
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

        if (Animations.Definition.animations.Length == 0)
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
