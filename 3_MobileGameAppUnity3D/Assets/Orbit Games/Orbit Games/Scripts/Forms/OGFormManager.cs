using GameToolkit.Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OGFormManager : OGSingletonBehaviour<OGFormManager> {

    [Header("Localization")]
    public LocalizedText Label_NothingSelected;

    [Header("References")]
    public OGFormTitle titlePrefab;
    public OGFormHeader headerPrefab;
    public OGFormPlainText plainTextPrefab;
    public OGFormTextField textFieldPrefab;
    public OGFormTextArea textAreaPrefab;
    public OGFormRichTextArea richTextAreaPrefab;
    public OGFormRadioButtons radioButtonsPrefab;
    public OGFormRadioButtonOption radioButtonOptionPrefab;
    public OGFormCheckboxes checkboxesPrefab;
    public OGFormCheckboxOption checkboxOptionPrefab;
    public OGFormDropdown dropdownPrefab;
    public OGFormDropdownOption dropdownOptionPrefab;
    public OGFormGroup groupPrefab;
    public OGFormButton buttonPrefab;

    [Header("Icons")]
    public Sprite defaultIcon;
    public Color defaultColor;
    public Sprite sendIcon;
    public Color sendColor;
    public Sprite acceptIcon;
    public Color acceptColor;
    public Sprite cancelIcon;
    public Color cancelColor;
    public Sprite deleteIcon;
    public Color deleteColor;
    public Sprite resetIcon;
    public Color resetColor;

    protected override void OnSingletonInitialize() { }

    public Sprite GetButtonStyleIcon(OGForm.ButtonStyle style)
    {
        switch (style)
        {
            case OGForm.ButtonStyle.Send:
                return sendIcon;
            case OGForm.ButtonStyle.Accept:
                return acceptIcon;
            case OGForm.ButtonStyle.Cancel:
                return cancelIcon;
            case OGForm.ButtonStyle.Delete:
                return defaultIcon;
            case OGForm.ButtonStyle.Reset:
                return resetIcon;
            case OGForm.ButtonStyle.Default:
                return defaultIcon;
            case OGForm.ButtonStyle.RandomColor:
                return defaultIcon;
            default:
                break;
        }
        return defaultIcon;
    }

    public Color GetButtonStyleColor(OGForm.ButtonStyle style)
    {
        switch (style)
        {
            case OGForm.ButtonStyle.Send:
                return sendColor;
            case OGForm.ButtonStyle.Accept:
                return acceptColor;
            case OGForm.ButtonStyle.Cancel:
                return cancelColor;
            case OGForm.ButtonStyle.Delete:
                return defaultColor;
            case OGForm.ButtonStyle.Reset:
                return resetColor;
            case OGForm.ButtonStyle.Default:
                return defaultColor;
            case OGForm.ButtonStyle.RandomColor:
                return OGColors.I.GetRandomColor(OGColors.Lightness.Full).color;
            default:
                break;
        }
        return defaultColor;
    }

    public OGFormBaseElementBehaviour GetPrefabFromElementSetup(OGForm.BaseElement elementSetup)
    {
        if (elementSetup.GetType() == typeof(OGForm.TitleElement))
            return titlePrefab;
        if (elementSetup.GetType() == typeof(OGForm.HeaderElement))
            return headerPrefab;
        if (elementSetup.GetType() == typeof(OGForm.PlainTextElement))
            return plainTextPrefab;
        if (elementSetup.GetType() == typeof(OGForm.TextFieldElement))
            return textFieldPrefab;
        if (elementSetup.GetType() == typeof(OGForm.TextAreaElement))
            return textAreaPrefab;
        if (elementSetup.GetType() == typeof(OGForm.RichTextAreaElement))
            return richTextAreaPrefab;
        if (elementSetup.GetType() == typeof(OGForm.RadioButtonsElement))
            return radioButtonsPrefab;
        if (elementSetup.GetType() == typeof(OGForm.RadioButtonOptionElement))
            return radioButtonOptionPrefab;
        if (elementSetup.GetType() == typeof(OGForm.CheckboxesElement))
            return checkboxesPrefab;
        if (elementSetup.GetType() == typeof(OGForm.CheckboxOptionElement))
            return checkboxOptionPrefab;
        if (elementSetup.GetType() == typeof(OGForm.DropdownElement))
            return dropdownPrefab;
        if (elementSetup.GetType() == typeof(OGForm.DropdownOptionElement))
            return dropdownOptionPrefab;
        if (elementSetup.GetType() == typeof(OGForm.ElementsGroup))
            return groupPrefab;
        if (elementSetup.GetType() == typeof(OGForm.ButtonElement))
            return buttonPrefab;
        return null;
    }
}
