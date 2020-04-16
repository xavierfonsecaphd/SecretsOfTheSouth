using GameToolkit.Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OGBaseSlidableElement : OGBaseAdjustableElement
{
    public abstract string GetMinDisplayValue();
    public abstract string GetMaxDisplayValue();
}
