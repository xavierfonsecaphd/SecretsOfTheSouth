using GameToolkit.Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VarBoolean_", menuName = "Construction/Boolean Variable Definition")]
public class OGBooleanVariableDefinition : OGBaseVariableDefinition
{
    protected override string GetFilePrefix()
    {
        return "VarBoolean";
    }
}
