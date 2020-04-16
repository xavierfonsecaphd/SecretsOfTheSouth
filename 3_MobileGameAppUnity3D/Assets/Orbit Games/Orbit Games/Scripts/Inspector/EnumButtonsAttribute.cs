using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class EnumButtonsAttribute : PropertyAttribute
{
    public int columns = 4;
    public EnumButtonsAttribute(int columns = 4) { this.columns = columns; }
}