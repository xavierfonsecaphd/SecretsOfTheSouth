using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameToolkit.Localization;

public class OGFormCheckboxOption : OGFormBaseElementGeneric<OGForm.CheckboxOptionElement>
{
    [Header("References")]
    public LocalizedTextBehaviour labelField;
    public Button selectButton;
    public Toggle toggle;

    public override void PutSelectablesInList(List<Selectable> selectables)
    {
        selectables.Add(selectButton);
    }

    public override void ReadDataFrom(OGForm.Data data, bool resetOtherwise)
    {
        if (data != null && data[elementSetup.variableName] != null)
        {
            var value = data[elementSetup.variableName];
            toggle.isOn = value == bool.TrueString;
        }
        else if (resetOtherwise)
        {
            toggle.isOn = elementSetup.defaultValue == bool.TrueString; // bool.Parse(elementSetup.defaultValue);
        }
    }

    public override void WriteDataTo(OGForm.Data data)
    {
        data[elementSetup.variableName] = toggle.isOn.ToString();
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
        labelField.FormattedAsset = elementSetup.labelText;
        if (elementSetup.disabled) Debug.LogError("elementSetup.disabled not yet implemented for " + GetType());
    }

    public override void Remove()
    {
        OGPool.removeCopy(this);
    }

    public void OnToggle()
    {
        parentForm.SetValue(elementSetup.variableName, (!toggle.isOn).ToString());
    }
}
