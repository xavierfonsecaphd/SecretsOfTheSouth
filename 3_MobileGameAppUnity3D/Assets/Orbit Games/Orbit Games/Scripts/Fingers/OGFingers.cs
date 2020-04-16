using DigitalRubyShared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OGFingers : OGSingletonBehaviour<OGFingers>
{
    public FingersScript fingersScript;

    protected override void OnSingletonInitialize()
    {
        if (Application.isEditor || Debug.isDebugBuild)
        {
            fingersScript.ShowTouches = true;
        }
        else
        {
            fingersScript.ShowTouches = false;
        }
    }
}
