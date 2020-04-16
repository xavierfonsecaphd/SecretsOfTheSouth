using GameToolkit.Localization;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OGFormPlainText : OGFormBaseElementGeneric<OGForm.PlainTextElement>
{
    [Header("References")]
    public LocalizedTextBehaviour textField;
    public LayoutElement textLayout;

    public override void PutSelectablesInList(List<Selectable> selectables)
    {
        // nothing to do here
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
        textField.gameObject.SetActive(elementSetup.labelText != null);
        textField.FormattedAsset = elementSetup.labelText;
        textLayout.preferredWidth = elementSetup.preferredWidth;
    }

    public override void Remove()
    {
        OGPool.removeCopy(this);
    }
}

