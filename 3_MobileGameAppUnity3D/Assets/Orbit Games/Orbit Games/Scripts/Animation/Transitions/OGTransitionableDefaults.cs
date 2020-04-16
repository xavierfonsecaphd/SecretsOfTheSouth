using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Transition Defaults", menuName = "Transition Defaults", order = 1)]
public class OGTransitionableDefaults : ScriptableObject
{
    [Header("Animation sets")]
    public OGTransitionableAnimationsPreset transitionSetPreset;
    public OGRegularAnimationsPreset animationSetPreset;

    [Header("Individual animations")]
    public OGRegularAnimationPreset regularAnimationPreset;
    public OGTransitionInPreset inTransitionPreset;
    public OGIdleAnimationPreset idleAnimationPreset;
    public OGTransitionOutPreset outTransitionPreset;

    [Header("Animation curve sets")]
    public OGAnimationCurvesPreset regularAnimationCurvesPreset;
    public OGAnimationCurvesPreset inTransitionCurvesPreset;
    public OGAnimationCurvesPreset IdleAnimationCurvesPreset;
    public OGAnimationCurvesPreset outTransitionCurvesPreset;

    [Header("Animation curve")]
    public OGAnimationCurvePreset animationCurvePreset;
    public OGAnimationTimeMapperPreset animationTimeMapperPreset;

    private static OGTransitionableDefaults FindDefaultsObject()
    {
        var settings = Resources.LoadAll<OGTransitionableDefaults>("");
        if (settings.Length == 0)
        {
            Debug.LogWarning("No OGTransitionableDefaults asset was found. Can't obtain default presets");
            return null;
        }
        if (settings.Length > 1)
        {
#if UNITY_EDITOR
            Debug.LogWarning("Found multiple OGTransitionableDefaults assets. Using the first one found: " + UnityEditor.AssetDatabase.GetAssetPath(settings[0]));
#else
            Debug.LogWarning("Found multiple OGTransitionableDefaults assets. Using the first one found.");
#endif
        }
        return settings[0];
    }

    public static OGTransitionableAnimationsPreset GetTransitionableAnimationSetPreset()
    {
        var settings = FindDefaultsObject();
        if (settings == null) return null;
        return settings.transitionSetPreset;
    }

    public static OGRegularAnimationsPreset GetRegularAnimationSetPreset()
    {
        var settings = FindDefaultsObject();
        if (settings == null) return null;
        return settings.animationSetPreset;
    }

    public static OGTransitionInPreset GetInTransitionPreset()
    {
        var settings = FindDefaultsObject();
        if (settings == null) return null;
        return settings.inTransitionPreset;
    }

    public static OGRegularAnimationPreset GetRegularAnimationPreset()
    {
        var settings = FindDefaultsObject();
        if (settings == null) return null;
        return settings.regularAnimationPreset;
    }

    public static OGIdleAnimationPreset GetIdleAnimationPreset()
    {
        var settings = FindDefaultsObject();
        if (settings == null) return null;
        return settings.idleAnimationPreset;
    }

    public static OGTransitionOutPreset GetOutTransitionPreset()
    {
        var settings = FindDefaultsObject();
        if (settings == null) return null;
        return settings.outTransitionPreset;
    }

    public static OGAnimationCurvesPreset GetRegularAnimationCurvesPreset()
    {
        var settings = FindDefaultsObject();
        if (settings == null) return null;
        return settings.regularAnimationCurvesPreset;
    }

    public static OGAnimationCurvesPreset GetInTransitionCurvesPreset()
    {
        var settings = FindDefaultsObject();
        if (settings == null) return null;
        return settings.inTransitionCurvesPreset;
    }

    public static OGAnimationCurvesPreset GetIdleAnimationCurvesPreset()
    {
        var settings = FindDefaultsObject();
        if (settings == null) return null;
        return settings.IdleAnimationCurvesPreset;
    }

    public static OGAnimationCurvesPreset GetOutTransitionCurvesPreset()
    {
        var settings = FindDefaultsObject();
        if (settings == null) return null;
        return settings.outTransitionCurvesPreset;
    }

    public static OGAnimationCurvePreset GetAnimationCurvePreset()
    {
        var settings = FindDefaultsObject();
        if (settings == null) return null;
        return settings.animationCurvePreset;
    }

    public static OGAnimationTimeMapperPreset GetAnimationTimeMapperPreset()
    {
        var settings = FindDefaultsObject();
        if (settings == null) return null;
        return settings.animationTimeMapperPreset;
    }
}
