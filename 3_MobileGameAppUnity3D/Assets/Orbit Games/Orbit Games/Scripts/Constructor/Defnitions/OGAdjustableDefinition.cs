using GameToolkit.Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Adjustable_", menuName = "Construction/Adjustable Definition")]
public class OGAdjustableDefinition : OGBaseDefinition
{
    protected override string GetFilePrefix()
    {
        return "Adjustable";
    }
}
