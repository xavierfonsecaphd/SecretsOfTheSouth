using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OGRegularAnimations : IOGInitializable
{
    public PresetableAnimation[] animations;

    [HideInInspector]
    public bool initialized = false;

    public void Initialize()
    {
        if (initialized)
        {
            foreach (var t in animations) t.Initialize();
            return;
        }

        initialized = true;

        if (animations == null) animations = new PresetableAnimation[0];
    }

    public bool OnGizmoUpdate(OGBaseAnimatableBehaviour animatable)
    {
        var noError = true;
        foreach (var t in animations)
            if (t != null)
                noError &= t.Definition.OnTransitionableGizmoUpdate(animatable);

        return noError;
    }

    public OGStateAnimation GetAnimationWithTag(string tag = "")
    {
        if (tag == null) tag = "";
        for (int i = 0; i < animations.Length; i++)
        {
            if (animations[i].Definition.tag == tag)
            {
                return animations[i].Definition;
            }
        }
        return null;
    }

    [System.Serializable]
    public class PresetableAnimation : OGPresetOrCustom<OGRegularAnimationPreset, OGRegularAnimation>
    {
    }
}
