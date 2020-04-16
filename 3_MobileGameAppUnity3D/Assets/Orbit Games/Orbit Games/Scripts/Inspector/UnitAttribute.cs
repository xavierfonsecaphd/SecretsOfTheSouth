using UnityEngine;

/// <summary>
/// This class is part of the OlivierUtils library, which is created by Olivier Hokke (OH).
/// OH has given TU Delft Gamelab permission for this library to be used solely within the scope of the Held project for the Hartstichting.
/// This code can thus NOT be used outside of this project or be modified by anyone other than OH.
/// </summary>
public class UnitAttribute : PropertyAttribute
{
    public string unit;
    public bool prefix = false; // broken
    public UnitAttribute(string unit) { this.unit = unit; }

    // broken: see the drawer
    // public UnitAttribute(string unit, bool prefix = false) { this.unit = unit; this.prefix = prefix; }
}