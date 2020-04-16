using GameToolkit.Localization;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OGStringVariable: OGBaseVariable<string>
{
    [Header("Color to set to")]
    public TMP_Text tmpText;
    public Text regularText;

    [Buttons("GetComponents")]
    public ButtonsContainer getters;
    public void GetComponents()
    {
        tmpText = GetComponent<TMP_Text>();
        regularText = GetComponent<Text>();
    }

    public override void ResetToDefault()
    {
        value = "Some variable text";
    }

    public override string ToDisplayValue()
    {
        return value;
    }

    protected override void OnValueUpdate()
    {
        if (!(definition is OGStringVariableDefinition))
        {
            Debug.LogError("variable definition is not of type String");
        }

        if (tmpText) tmpText.text = value;
        if (regularText) regularText.text = value;
    }
}
