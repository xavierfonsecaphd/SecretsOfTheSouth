using Fenderrio.ImageWarp;
using GameToolkit.Localization;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OGColorVariable: OGBaseVariable<Vector4>
{
    [Header("Color to set to")]
    public Image image;
    public RawImageWarp warpableImage;
    public Text regularText;
    public TMP_Text tmpText;

    [Buttons("GetComponents")]
    public ButtonsContainer getters;
    public void GetComponents()
    {
        image = GetComponent<Image>();
        warpableImage = GetComponent<RawImageWarp>();
        regularText = GetComponent<Text>();
        tmpText = GetComponent<TMP_Text>();
    }

    public override void ResetToDefault()
    {
        value = Color.magenta;
    }

    public override string ToDisplayValue()
    {
        return value.ToString();
    }

    protected override void OnValueUpdate()
    {
        if (!(definition is OGColorVariableDefinition))
        {
            Debug.LogError("variable definition is not of type Color");
        }

        if (image) image.color = value;
        if (warpableImage) warpableImage.color = value;
        if (regularText) regularText.color = value;
        if (tmpText) tmpText.color = value;
    }
}
