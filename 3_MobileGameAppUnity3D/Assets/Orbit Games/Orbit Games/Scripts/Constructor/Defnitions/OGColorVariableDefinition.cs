using GameToolkit.Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VarColor_", menuName = "Construction/Color Variable Definition")]
public class OGColorVariableDefinition : OGBaseVariableDefinition
{
    protected override string GetFilePrefix()
    {
        return "VarColor";
    }
}
