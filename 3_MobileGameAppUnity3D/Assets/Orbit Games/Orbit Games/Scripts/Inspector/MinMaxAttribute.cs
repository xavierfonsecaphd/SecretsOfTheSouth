using UnityEngine;

public class MinMaxAttribute : PropertyAttribute
{
    public float MinLimit = 0;
    public float MaxLimit = 1;
    public bool ShowEditRange;
    public bool ShowDebugValues;

    public MinMaxAttribute(int min, int max)
    {
        ShowEditRange = true;
        MinLimit = min;
        MaxLimit = max;
    }

    public MinMaxAttribute(float min, float max)
    {
        ShowEditRange = true;
        MinLimit = min;
        MaxLimit = max;
    }
}