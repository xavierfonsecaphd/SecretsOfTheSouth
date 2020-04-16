using GameToolkit.Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OGFormattedLocalizedText {

    public LocalizedText LocalizedAsset;
    public string[] FormatArgs;
    
    public static implicit operator OGFormattedLocalizedText(LocalizedText d)
    {
        return new OGFormattedLocalizedText
        {
            LocalizedAsset = d,
            FormatArgs = new string[] { }
        };
    }

    public static implicit operator OGFormattedLocalizedText(string d)
    {
        return OGLocalization.Create(d);
    }

    public override string ToString()
    {
        return string.Format(LocalizedAsset.Value, FormatArgs);
    }
}
