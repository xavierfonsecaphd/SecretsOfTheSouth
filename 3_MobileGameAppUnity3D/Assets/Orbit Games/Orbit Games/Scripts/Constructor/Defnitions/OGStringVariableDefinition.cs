using GameToolkit.Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VarString_", menuName = "Construction/String Variable Definition")]
public class OGStringVariableDefinition : OGBaseVariableDefinition
{
    protected override string GetFilePrefix()
    {
        return "VarString";
    }
}
