using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct HSLColor
{
    [Range(0, 1f)]
    public float h;
    [Range(0, 1f)]
    public float s;
    [Range(0, 1f)]
    public float L;
    [Range(0, 1f)]
    public float a;

    public HSLColor(float h, float s, float L, float a)
    {
        this.h = h;
        this.s = s;
        this.L = L;
        this.a = a;
    }

    public override string ToString()
    {
        return string.Format("HSLA: ({0:F3}, {1:F3}, {2:F3}, {3:F3})", h, s, L, a);
    }

    private static float Rand01 { get { return UnityEngine.Random.Range(0, 1f); } }
    private static float Rand01Symmetry { get { return (Rand01 - 0.5f) * 2f; } }

    public void MutateHue(float amount = 1f)
    {
        h = Mathf.Clamp01(h + Rand01Symmetry * amount);
    }

    public void MutateSaturation(float amount = 1f)
    {
        s = Mathf.Clamp01(s + Rand01Symmetry * amount);
    }

    public void MutateLightness(float amount = 1f)
    {
        L = Mathf.Clamp01(L + Rand01Symmetry * amount);
    }

    public void MutateAlpha(float amount = 1f)
    {
        a = Mathf.Clamp01(a + Rand01Symmetry * amount);
    }

    public void MutateHueLooped(float amount = 1f)
    {
        h = (1f + h + Rand01Symmetry * amount) % 1f;
    }

    public void MutateSaturationLooped(float amount = 1f)
    {
        s = (1f + s + Rand01Symmetry * amount) % 1f;
    }

    public void MutateLightnessLooped(float amount = 1f)
    {
        L = (1f + L + Rand01Symmetry * amount) % 1f;
    }

    public void MutateAlphaLooped(float amount = 1f)
    {
        a = (1f + a + Rand01Symmetry * amount) % 1f;
    }

    public static HSLColor Random(float a = 1f)
    {
        return new HSLColor(Rand01, Rand01, Rand01, a);
    }

    public static HSLColor Random(Vector2 minMaxH, Vector2 minMaxS, Vector2 minMaxL, Vector2? minMaxA = null)
    {
        return new HSLColor(
            UnityEngine.Random.Range(minMaxH.x, minMaxH.y),
            UnityEngine.Random.Range(minMaxS.x, minMaxS.y),
            UnityEngine.Random.Range(minMaxL.x, minMaxL.y),
            minMaxA.HasValue ? UnityEngine.Random.Range(minMaxH.x, minMaxH.y) : 1f);
    }

    public static HSLColor RandomHue(float s, float L, float a = 1f)
    {
        return new HSLColor(Rand01, s, L, a);
    }

    public static HSLColor RandomSaturation(float h, float L, float a = 1f)
    {
        return new HSLColor(h, Rand01, L, a);
    }

    public static HSLColor RandomLightness(float h, float s, float a = 1f)
    {
        return new HSLColor(h, s, Rand01, a);
    }

    public static bool operator ==(HSLColor lhs, HSLColor rhs)
    {
        if (lhs.a != rhs.a)
        {
            return false;
        }

        if (lhs.L == 0 && rhs.L == 0)
        {
            return true;
        }

        if (lhs.s == 0 && rhs.s == 0)
        {
            return lhs.L == rhs.L;
        }

        return lhs.h == rhs.h &&
               lhs.s == rhs.s &&
               lhs.L == rhs.L;
    }

    public Color ToColor()
    {
        return this;
    }

    public static implicit operator HSLColor(Color c)
    {
        return RGBtoHSL(c);
    }

    public static implicit operator Color(HSLColor hsl)
    {
        return HSLtoRGB(hsl);
    }

    public static bool operator !=(HSLColor lhs, HSLColor rhs)
    {
        return !(lhs == rhs);
    }

    public override bool Equals(object other)
    {
        if (other == null)
        {
            return false;
        }

        if (other is HSLColor || other is Color || other is Color32)
        {
            return this == (HSLColor)other;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return h.GetHashCode() ^ s.GetHashCode() ^ L.GetHashCode() ^ a.GetHashCode();
    }

    // https://www.programmingalgorithms.com/algorithm/rgb-to-hsl
    public static HSLColor RGBtoHSL(Color rgb)
    {
        HSLColor hsl = new HSLColor();

        float r = rgb.r;
        float g = rgb.g;
        float b = rgb.b;

        float min = Mathf.Min(Mathf.Min(r, g), b);
        float max = Mathf.Max(Mathf.Max(r, g), b);
        float delta = max - min;

        hsl.L = (max + min) / 2;

        if (delta == 0)
        {
            hsl.h = 0;
            hsl.s = 0.0f;
        }
        else
        {
            hsl.s = (hsl.L <= 0.5) ? (delta / (max + min)) : (delta / (2 - max - min));

            float hue;

            if (r == max)
            {
                hue = ((g - b) / 6) / delta;
            }
            else if (g == max)
            {
                hue = (1.0f / 3) + ((b - r) / 6) / delta;
            }
            else
            {
                hue = (2.0f / 3) + ((r - g) / 6) / delta;
            }

            if (hue < 0)
                hue += 1;
            if (hue > 1)
                hue -= 1;

            hsl.h = (hue);
        }

        hsl.a = rgb.a;
        return hsl;
    }

    // https://www.programmingalgorithms.com/algorithm/hsl-to-rgb
    public static Color HSLtoRGB(HSLColor hsl)
    {
        float r = 0;
        float g = 0;
        float b = 0;

        if (hsl.s == 0)
        {
            r = g = b = (float)(hsl.L);
        }
        else
        {
            float v1, v2;
            float hue = (float)hsl.h;

            v2 = (hsl.L < 0.5) ? (hsl.L * (1 + hsl.s)) : ((hsl.L + hsl.s) - (hsl.L * hsl.s));
            v1 = 2 * hsl.L - v2;

            r = (float)(HueToRGB(v1, v2, hue + (1.0f / 3)));
            g = (float)(HueToRGB(v1, v2, hue));
            b = (float)(HueToRGB(v1, v2, hue - (1.0f / 3)));
        }

        return new Color(r, g, b, hsl.a);
    }

    private static float HueToRGB(float v1, float v2, float vH)
    {
        if (vH < 0)
            vH += 1;

        if (vH > 1)
            vH -= 1;

        if ((6 * vH) < 1)
            return (v1 + (v2 - v1) * 6 * vH);

        if ((2 * vH) < 1)
            return v2;

        if ((3 * vH) < 2)
            return (v1 + (v2 - v1) * ((2.0f / 3) - vH) * 6);

        return v1;
    }
}