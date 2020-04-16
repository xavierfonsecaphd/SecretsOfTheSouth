using UnityEngine;

public static class OGColorExtension
{
    private const float EPSILON = 1e-8f;

    public static HSVColor ToHSV(this Color rgb)
    {
        return HSVColor.RGBtoHSV(rgb);
    }

    public static HSLColor ToHSL(this Color rgb)
    {
        return HSLColor.RGBtoHSL(rgb);
    }

    public static LABColor ToLAB(this Color rgb)
    {
        return LABColor.RGBtoLAB(rgb);
    }
}