using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameToolkit.Localization;

public class OGFormRadioButtonOption : OGFormBaseElementGeneric<OGForm.RadioButtonOptionElement>
{
    [Header("References")]
    public LocalizedTextBehaviour labelField;
    public Button selectButton;
    public Toggle toggle;
    public OGForm.RadioButtonsElement radioSetup;

    public override void PutSelectablesInList(List<Selectable> selectables)
    {
        selectables.Add(selectButton);
    }

    public override void ReadDataFrom(OGForm.Data data, bool resetOtherwise)
    {
        if (radioSetup == null)
        {
            Debug.LogError("Radio option has no parent radio element");
            return;
        }
        if (data != null && data[radioSetup.variableName] != null)
        {
            var value = data[radioSetup.variableName];
            toggle.isOn = value == elementSetup.optionValue;
        }
        else if (resetOtherwise)
        {
            toggle.isOn = radioSetup.defaultValue == elementSetup.optionValue;
        }
    }

    public override void WriteDataTo(OGForm.Data data)
    {
        if (radioSetup == null)
        {
            Debug.LogError("Radio option has no parent radio element");
            return;
        }
        if (toggle.isOn)
        {
            data[radioSetup.variableName] = elementSetup.optionValue;
        }
    }

    public override void CalculatePoints(ref int points)
    {
        if (toggle.isOn)
        {
            points += elementSetup.optionPoints;
        }
    }

    protected override void BuildSetup()
    {
        if (parentElement is OGFormRadioButtons)
        {
            radioSetup = (parentElement as OGFormRadioButtons).elementSetup;
        }
        labelField.gameObject.SetActive(elementSetup.labelText != null);
        labelField.FormattedAsset = elementSetup.labelText;
        if (elementSetup.disabled) Debug.LogError("elementSetup.disabled not yet implemented for " + GetType());
    }

    public override void Remove()
    {
        OGPool.removeCopy(this);
    }

    public void OnToggle()
    {
        if (radioSetup == null)
        {
            Debug.LogError("Radio option has no parent radio element");
            return;
        }
        parentForm.SetValue(radioSetup.variableName, elementSetup.optionValue);
    }
}
