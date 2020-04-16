using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class QuickButtonsAttribute : PropertyAttribute
{
    public bool showUpDownButtons;
    public QuickButtonsAttribute(bool showUpDownButtons = true)
    {
        this.showUpDownButtons = showUpDownButtons;
    }
}
