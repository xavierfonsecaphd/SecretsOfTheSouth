using GameToolkit.Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Socket__Hole_", menuName = "Construction/Socket Hole Definition")]
public class OGSocketHoleDefinition : ScriptableObject
{
    public string extraID = "";

    public override string ToString()
    {
        return "OGSocketHoleDefinition (Name = " + name + ")";
    }
}
