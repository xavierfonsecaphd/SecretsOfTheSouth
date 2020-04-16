using GameToolkit.Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Plug_", menuName = "Construction/Plug Definition")]
public class OGPlugDefinition : OGBaseDefinition
{
    [Header("Settings")]
    public OGSocketDefinition forSocket;

    public override string GetUniqueID()
    {
        return forSocket.GetUniqueID() + "." + base.GetUniqueID();
    }

    protected override string GetFilePrefix()
    {
        return "Plug";
    }
}
