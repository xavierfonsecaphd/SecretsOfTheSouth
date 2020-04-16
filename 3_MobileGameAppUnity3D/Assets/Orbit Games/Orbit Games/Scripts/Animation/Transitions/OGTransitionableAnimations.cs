using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OGTransitionableAnimations : IOGInitializable
{
    public PresetableInTransition[] inTransitions;
    public PresetableIdleAnimation[] idleAnimations;
    public PresetableOutTransition[] outTransitions;

    [HideInInspector]
    public bool initialized = false;

    public void Initialize()
    {
        if (initialized)
        {
            foreach (var t in inTransitions) t.Initialize();
            foreach (var t in idleAnimations) t.Initialize();
            foreach (var t in outTransitions) t.Initialize();
            return;
        }

        initialized = true;

        if (inTransitions == null) inTransitions = new PresetableInTransition[0];
        if (outTransitions == null) outTransitions = new PresetableOutTransition[0];
        if (idleAnimations == null) idleAnimations = new PresetableIdleAnimation[0];
    }

    public bool OnGizmoUpdate(OGTransitionable transitionable)
    {
        var noError = true;
        foreach (var t in inTransitions)
            if (t != null)
                noError &= t.Definition.OnTransitionableGizmoUpdate(transitionable);

        foreach (var t in idleAnimations)
            if (t != null)
                noError &= t.Definition.OnTransitionableGizmoUpdate(transitionable);

        foreach (var t in outTransitions)
            if (t != null)
                noError &= t.Definition.OnTransitionableGizmoUpdate(transitionable);

        return noError;
    }

    public OGTransitionInAnimation GetInTransitionWithTag(string tag = "")
    {
        if (tag == null) tag = "";
        for (int i = 0; i < inTransitions.Length; i++)
        {
            if (inTransitions[i].Definition.tag == tag)
            {
                return inTransitions[i].Definition;
            }
        }
        return null;
    }

    public OGTransitionOutAnimation GetOutTransitionWithTag(string tag = "")
    {
        if (tag == null) tag = "";
        for (int i = 0; i < outTransitions.Length; i++)
        {
            if (outTransitions[i].Definition.tag == tag)
            {
                return outTransitions[i].Definition;
            }
        }
        return null;
    }

    public OGIdleAnimation GetIdleAnimationWithTag(string tag = "")
    {
        if (tag == null) tag = "";
        for (int i = 0; i < idleAnimations.Length; i++)
        {
            if (idleAnimations[i].Definition.tag == tag)
            {
                return idleAnimations[i].Definition;
            }
        }
        return null;
    }

    [System.Serializable]
    public class PresetableInTransition : OGPresetOrCustom<OGTransitionInPreset, OGTransitionInAnimation>
    {
    }

    [System.Serializable]
    public class PresetableOutTransition : OGPresetOrCustom<OGTransitionOutPreset, OGTransitionOutAnimation>
    {
    }

    [System.Serializable]
    public class PresetableIdleAnimation : OGPresetOrCustom<OGIdleAnimationPreset, OGIdleAnimation>
    {
    }
}
