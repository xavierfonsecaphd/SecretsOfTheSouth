using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameToolkit.Localization;

public class OGFormDropdownOption : OGFormBaseElementGeneric<OGForm.DropdownOptionElement>
{
    [Header("References")]
    public LocalizedTextBehaviour labelField;
    public GameObject selectedIndicator;
    public Button selectButton;
    public OGFormDropdown dropdownElement;
    public OGForm.DropdownElement dropdownSetup;

    public override void PutSelectablesInList(List<Selectable> selectables)
    {
        // not adding anything here
    }

    public override void ReadDataFrom(OGForm.Data data, bool resetOtherwise)
    {
        if (dropdownSetup == null)
        {
            Debug.LogError("Dropdown option has no parent dropdown element");
            return;
        }
        if (data != null && data[dropdownSetup.variableName] != null)
        {
            var value = data[dropdownSetup.variableName];
            selectedIndicator.SetActive(value == elementSetup.optionValue);
        }
        else if (resetOtherwise)
        {
            selectedIndicator.SetActive(dropdownSetup.defaultValue == elementSetup.optionValue);
        }
    }

    public override void WriteDataTo(OGForm.Data data)
    {
        if (dropdownSetup == null)
        {
            Debug.LogError("Dropdown option has no parent dropdown element");
            return;
        }
        if (selectedIndicator.activeSelf)
        {
            data[dropdownSetup.variableName] = elementSetup.optionValue;
        }
    }

    public override void CalculatePoints(ref int points)
    {
        if (selectedIndicator.activeSelf)
        {
            points += elementSetup.optionPoints;
        }
    }

    protected override void BuildSetup()
    {
        if (parentElement is OGFormDropdown)
        {
            dropdownElement = (parentElement as OGFormDropdown);
            dropdownSetup = dropdownElement.elementSetup;
        }
        labelField.FormattedAsset = elementSetup.labelText;
        if (elementSetup.disabled) Debug.LogError("elementSetup.disabled not yet implemented for " + GetType());
    }

    public override void Remove()
    {
        OGPool.removeCopy(this);
    }

    public bool IsToggled()
    {
        return selectedIndicator.activeSelf;
    }

    public void OnToggle()
    {
        if (dropdownSetup == null)
        {
            Debug.LogError("Dropdown option has no parent dropdown element");
            return;
        }
        dropdownElement.CloseDropdownList();
        parentForm.SetValue(dropdownSetup.variableName, elementSetup.optionValue);
    }
}
