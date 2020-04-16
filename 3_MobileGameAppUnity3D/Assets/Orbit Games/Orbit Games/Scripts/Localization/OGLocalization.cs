using GameToolkit.Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OGLocalization : OGSingletonBehaviour<OGLocalization> {

    protected override void OnSingletonInitialize()
    {

    }

    public LocalizedText emptyText;
    public LocalizedText variableText;
    public OGFormattedLocalizedText emptyFormattedText;

    public static LocalizedText EmptyText
    {
        get
        {
            return I.emptyText;
        }
    }

    public static OGFormattedLocalizedText EmptyFormattedText
    {
        get
        {
            if (I.emptyFormattedText == null)
            {
                I.emptyFormattedText = new OGFormattedLocalizedText
                {
                    LocalizedAsset = I.emptyText,
                    FormatArgs = new string[0] { }
                };
            }
            return I.emptyFormattedText;
        }
    }

    public static LocalizedText CreateLocalizedTextAsset(string text)
    {
        LocalizedText localized = ScriptableObject.CreateInstance<LocalizedText>();
        localized.SetText(text);
        return localized;
    }

    public static OGFormattedLocalizedText Create(string text)
    {
        if (text.IsNullOrEmpty())
        {
            return EmptyFormattedText;
        }
        else
        {
            return new OGFormattedLocalizedText
            {
                LocalizedAsset = I.variableText,
                FormatArgs = new string[1] { text }
            };
        }
    }
}

public static class LocalizedTextExtenstions
{
    public static OGFormattedLocalizedText Format(this LocalizedText asset, params string[] text)
    {
        string[] formatArgs = new string[text.Length];
        text.CopyTo(formatArgs, 0);
        return new OGFormattedLocalizedText
        {
            LocalizedAsset = asset,
            FormatArgs = formatArgs
        };
    }
}