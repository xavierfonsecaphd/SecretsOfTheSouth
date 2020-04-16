using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameToolkit.Localization;

public class OGFormTextField: OGFormBaseElementGeneric<OGForm.TextFieldElement>
{
    [Header("References")]
    public LocalizedTextBehaviour labelField;
    public LocalizedTextBehaviour hintField;
    public RectMask2D textareaMask;
    public TMP_InputField inputField;
    public LayoutElement inputLayout;

    public override void PutSelectablesInList(List<Selectable> selectables)
    {
        selectables.Add(inputField);
    }

    public override void ReadDataFrom(OGForm.Data data, bool resetOtherwise)
    {
        if (data != null && data[elementSetup.variableName] != null)
        {
            inputField.text = data[elementSetup.variableName];
        }
        else if (resetOtherwise)
        {
            inputField.text = elementSetup.defaultValue;
        }
    }

    public override void WriteDataTo(OGForm.Data data)
    {
        data[elementSetup.variableName] = inputField.text;
    }

    public override void CalculatePoints(ref int points)
    {
        // nothing to do here
    }

    protected override void BuildSetup()
    {
        labelField.gameObject.SetActive(elementSetup.labelText != null);
        labelField.FormattedAsset = elementSetup.labelText;
        inputField.text = elementSetup.defaultValue;
        inputField.lineType = TMP_InputField.LineType.SingleLine;

        inputField.contentType = elementSetup.contentType;
        inputField.characterLimit = elementSetup.characterLimit;
        
        hintField.FormattedAsset = elementSetup.hintText;
        inputLayout.preferredWidth = elementSetup.preferredWidth;

        if (elementSetup.disabled) Debug.LogError("elementSetup.disabled not yet implemented for " + GetType());

        // INSANE HACK FOR RECT MASK ISSUE WHEN POOLING OBJECTS WITH MASK 
        if (textareaMask != null)
        {
            var temp = textareaMask.gameObject;
            temp.SetActive(false);
            Destroy(textareaMask);

            OGRun.NextFrame(() =>
            {
                textareaMask = temp.AddComponent<RectMask2D>();
                temp.SetActive(true);
            });
        }
    }

    public override void Remove()
    {
        OGPool.removeCopy(this);
    }
}
