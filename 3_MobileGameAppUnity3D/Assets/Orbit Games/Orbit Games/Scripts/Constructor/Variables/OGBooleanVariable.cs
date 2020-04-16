using GameToolkit.Localization;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OGBooleanVariable: OGBaseVariable<bool>
{
    [Header("States")]
    public OGAnimationMask mask;
    public OGAnimationTargets targets;
    public OGAnimationState falseState;
    public OGAnimationState trueState;

    [Buttons(true, "Get targets", "GetTargets")]
    public ButtonsContainer get;
    [Buttons("● FALSE", "SetFalse", "● TRUE", "SetTrue")]
    public ButtonsContainer record;
    [Buttons("►  FALSE", "ShowFalse", "►  TRUE", "ShowTrue")]
    public ButtonsContainer show;

    public void GetTargets()
    {
        targets = new OGAnimationTargets(gameObject);
    }

    public void SetFalse()
    {
        falseState = new OGAnimationState(gameObject);
    }

    public void SetTrue()
    {
        trueState = new OGAnimationState(gameObject);
    }

    public void ShowFalse()
    {
        falseState.ApplyTo(targets, mask);
    }

    public void ShowTrue()
    {
        trueState.ApplyTo(targets, mask);
    }

    public override void ResetToDefault()
    {
        value = false;
    }

    public override string ToDisplayValue()
    {
        return value ? "True" : "False";
    }

    protected override void OnValueUpdate()
    {
        if (!(definition is OGBooleanVariableDefinition))
        {
            Debug.LogError("variable definition is not of type Boolean");
        }

        if (value) trueState.ApplyTo(targets, mask);
        else falseState.ApplyTo(targets, mask);
    }
}
