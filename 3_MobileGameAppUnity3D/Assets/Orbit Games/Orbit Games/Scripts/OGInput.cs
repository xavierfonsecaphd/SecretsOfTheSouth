using UnityEngine;

/// <summary>
/// Bypassing unities Input Manager because it's still the old crappy confusing input system.
/// Unitys new input manager is still experimental.
/// </summary>
public static class OGInput {

    public static bool IsNavigationBlocked()
    {
        return OGForm.IsInputActive() || OGPopup.IsPopupActive();
    }

    public static bool OnNext()
    {
        return Input.GetKeyUp(KeyCode.RightArrow);
    }

    public static bool OnPrevious()
    {
        return Input.GetKeyUp(KeyCode.LeftArrow);
    }

    public static bool WantsToEscape()
    {
        return Input.GetKeyUp(KeyCode.Escape);
    }

    public static bool WantsToGoBack()
    {
        return Input.GetKeyUp(KeyCode.Backspace) || Input.GetKeyUp(KeyCode.Mouse3);
    }



    public static bool IsHoldingEditorCommand()
    {
        var ctrlOnly = IsHoldingCtrlOnly();
        if (!ctrlOnly)
        {
            return false;
        }
        if (Input.GetKey(KeyCode.V))
        {
            return true;
        }
        else if (Input.GetKeyUp(KeyCode.V))
        {
            return true;
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            return true;
        }
        else if (Input.GetKey(KeyCode.C))
        {
            return true;
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            return true;
        }
        else if (Input.GetKey(KeyCode.X))
        {
            return true;
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            return true;
        }
        else if (Input.GetKey(KeyCode.Z))
        {
            return true;
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            return true;
        }
        else if (Input.GetKey(KeyCode.Y))
        {
            return true;
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            return true;
        }
        else if (Input.GetKey(KeyCode.B))
        {
            return true;
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            return true;
        }
        else if (Input.GetKey(KeyCode.I))
        {
            return true;
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            return true;
        }
        else if (Input.GetKey(KeyCode.U))
        {
            return true;
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            return true;
        }
        return false;
    }


    public static bool OnPaste()
    {
        return IsHoldingCtrlOnly() && Input.GetKeyDown(KeyCode.V);
    }

    public static bool OnCopy()
    {
        return IsHoldingCtrlOnly() && Input.GetKeyDown(KeyCode.C);
    }

    public static bool OnCut()
    {
        return IsHoldingCtrlOnly() && Input.GetKeyDown(KeyCode.X);
    }

    public static bool OnUndo()
    {
        return IsHoldingCtrlOnly() && Input.GetKeyDown(KeyCode.Z);
    }

    public static bool ORedo()
    {
        return IsHoldingCtrlOnly() && Input.GetKeyDown(KeyCode.Y);
    }

    public static bool OnBold()
    {
        return IsHoldingCtrlOnly() && Input.GetKeyDown(KeyCode.B);
    }

    public static bool OnItalic()
    {
        return IsHoldingCtrlOnly() && Input.GetKeyDown(KeyCode.I);
    }

    public static bool OnUnderline()
    {
        return IsHoldingCtrlOnly() && Input.GetKeyDown(KeyCode.U);
    }


    public static int GetPressedNumericKey()
    {
        if (Input.GetKeyUp(KeyCode.Keypad0)) return 0;
        if (Input.GetKeyUp(KeyCode.Keypad1)) return 1;
        if (Input.GetKeyUp(KeyCode.Keypad2)) return 2;
        if (Input.GetKeyUp(KeyCode.Keypad3)) return 3;
        if (Input.GetKeyUp(KeyCode.Keypad4)) return 4;
        if (Input.GetKeyUp(KeyCode.Keypad5)) return 5;
        if (Input.GetKeyUp(KeyCode.Keypad6)) return 6;
        if (Input.GetKeyUp(KeyCode.Keypad7)) return 7;
        if (Input.GetKeyUp(KeyCode.Keypad8)) return 8;
        if (Input.GetKeyUp(KeyCode.Keypad9)) return 9;
        if (Input.GetKeyUp(KeyCode.Alpha0)) return 0;
        if (Input.GetKeyUp(KeyCode.Alpha1)) return 1;
        if (Input.GetKeyUp(KeyCode.Alpha2)) return 2;
        if (Input.GetKeyUp(KeyCode.Alpha3)) return 3;
        if (Input.GetKeyUp(KeyCode.Alpha4)) return 4;
        if (Input.GetKeyUp(KeyCode.Alpha5)) return 5;
        if (Input.GetKeyUp(KeyCode.Alpha6)) return 6;
        if (Input.GetKeyUp(KeyCode.Alpha7)) return 7;
        if (Input.GetKeyUp(KeyCode.Alpha8)) return 8;
        if (Input.GetKeyUp(KeyCode.Alpha9)) return 9;
        return -1;
    }

    public static bool WantsToReallySubmit()
    {
        return IsHoldingCtrl() && WantsToSubmit();
    }

    public static bool WantsToSubmit()
    {
        return Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter);
    }

    public static bool WantsToPressButton()
    {
        return Input.GetKeyUp(KeyCode.Space) || WantsToSubmit();
    }

    public static bool IsForwardCyclePressed()
    {
        return !IsHoldingShift() && Input.GetKeyDown(KeyCode.Tab);
    }

    public static bool IsBackwardCyclePressed()
    {
        return IsHoldingShift() && Input.GetKeyDown(KeyCode.Tab);
    }

    public static bool IsEnablingCheats()
    {
        return IsHoldingShift() && IsHoldingCtrl() && IsHoldingAlt() && Input.GetKeyUp(KeyCode.C);
    }

    public static bool IsRequestingCheat()
    {
        return IsHoldingCtrl() && IsHoldingAlt();
    }

    public static bool IsClearingAllPrefs()
    {
        return IsHoldingShift() && IsHoldingCtrl() && IsHoldingAlt() && Input.GetKeyUp(KeyCode.P);
    }

    public static bool IsClearingUnityPrefs()
    {
        return IsHoldingCtrl() && Input.GetKeyUp(KeyCode.P);
    }

    public static bool IsForcingCrash()
    {
        return IsHoldingShift() && IsHoldingCtrl() && IsHoldingAlt() && Input.GetKeyUp(KeyCode.Q);
    }

    public static bool IsHoldingShift()
    {
        return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

    public static bool IsHoldingCtrl()
    {
        return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
    }

    public static bool IsHoldingCtrlOnly()
    {
        return IsHoldingCtrl() && !IsHoldingAlt() && !IsHoldingShift();
    }

    public static bool IsHoldingAlt()
    {
        return Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
    }
}
