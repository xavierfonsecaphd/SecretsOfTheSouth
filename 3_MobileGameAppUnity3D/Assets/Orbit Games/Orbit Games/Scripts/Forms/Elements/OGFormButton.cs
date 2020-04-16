using GameToolkit.Localization;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OGFormButton : OGFormBaseElementGeneric<OGForm.ButtonElement>
{
    [Header("References")]
    public LocalizedTextBehaviour buttonField;
    public Button button;
    public Image buttonFrame;
    public Image icon;

    public override void PutSelectablesInList(List<Selectable> selectables)
    {
        selectables.Add(button);
    }

    public override void ReadDataFrom(OGForm.Data data, bool resetOtherwise)
    {
        // nothing to do here
    }

    public override void WriteDataTo(OGForm.Data data)
    {
        // nothing to do here
    }

    public override void CalculatePoints(ref int points)
    {
        // nothing to do here
    }

    protected override void BuildSetup()
    {
        buttonField.FormattedAsset = elementSetup.labelText;
        button.transition = Selectable.Transition.ColorTint; // hack to reset tint transition animation
        button.interactable = !elementSetup.disabled;

        if (elementSetup.overrideColor.HasValue)
        {
            buttonFrame.color = elementSetup.overrideColor.Value;
        }
        else
        {
            buttonFrame.color = OGFormManager.I.GetButtonStyleColor(elementSetup.style);
        }

        if (elementSetup.overrideIcon != null)
        {
            icon.sprite = elementSetup.overrideIcon;
        }
        else
        {
            icon.sprite = OGFormManager.I.GetButtonStyleIcon(elementSetup.style);
        }
    }

    public override void Remove()
    {
        OGPool.removeCopy(this);
    }

    public void OnPress()
    {
        var setup = elementSetup;
        switch (setup.buttonType)
        {
            case OGForm.ButtonType.Submit:
                parentForm.SubmitForm();
                break;
            case OGForm.ButtonType.Reset:
                parentForm.ResetForm();
                break;
            default:
                break;
        }
        if (setup.onPress != null)
        {
            setup.onPress.Invoke();
        }
    }
}