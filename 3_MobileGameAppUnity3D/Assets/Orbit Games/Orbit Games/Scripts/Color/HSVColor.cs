/*
 * Created by C.J. Kimberlin (http://cjkimberlin.com)
 * 
 * The MIT License (MIT)
 * 
 * Copyright (c) 2015 
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 * 
 * 
 * 
 * ============= Description =============
 * 
 * An ColorHSV struct for interpreting a color in hue/saturation/value instead of red/green/blue.
 * NOTE! hue will be a value from 0 to 1 instead of 0 to 360.
 * 
 * ColorHSV hsvRed = new ColorHSV(1, 1, 1, 1); // RED
 * ColorHSV hsvGreen = new ColorHSV(0.333f, 1, 1, 1); // GREEN
 * 
 * 
 * Also supports implicit conversion between Color and Color32.
 * 
 * ColorHSV hsvBlue = Color.blue; // HSVA(0.667f, 1, 1, 1)
 * Color blue = hsvBlue; // RGBA(0, 0, 1, 1)
 * Color32 blue32 = hsvBlue; // RGBA(0, 0, 255, 255) 
 *
 * 
 * If functions are desired instead of implicit conversion then use the following.
 * 
 * Color yellowBefore = Color.yellow; // RBGA(1, .922f, 0.016f, 1)
 * ColorHSV hsvYellow = Color.yellowBefore.ToHSV(); // HSVA(0.153f, 0.984f, 1, 1)
 * Color yellowAfter = hsvYellow.ToRGB(); // RBGA(1, .922f, 0.016f, 1)
 * */

using UnityEngine;

[System.Serializable]
public struct HSVColor
{
    [Range(0, 1f)]
    public float h;
    [Range(0, 1f)]
    public float s;
    [Range(0, 1f)]
    public float v;
    [Range(0, 1f)]
    public float a;

    public HSVColor(float h, float s, float v, float a = 1f)
    {
        this.h = h;
        this.s = s;
        this.v = v;
        this.a = a;
    }

    public override string ToString()
    {
        return string.Format("HSVA: ({0:F3}, {1:F3}, {2:F3}, {3:F3})", h, s, v, a);
    }

    public static bool operator ==(HSVColor lhs, HSVColor rhs)
    {
        if (lhs.a != rhs.a)
        {
            return false;
        }

        if (lhs.v == 0 && rhs.v == 0)
        {
            return true;
        }

        if (lhs.s == 0 && rhs.s == 0)
        {
            return lhs.v == rhs.v;
        }

        return lhs.h == rhs.h &&
               lhs.s == rhs.s &&
               lhs.v == rhs.v;
    }

    private static float Rand01 { get { return UnityEngine.Random.value; } }
    private static float Rand01Symmetry { get { return (Rand01 - 0.5f) * 2f; } }

    public void MutateHue(float amount = 1f)
    {
        h = Mathf.Clamp01(h + Rand01Symmetry * amount);
    }

    public void MutateSaturation(float amount = 1f)
    {
        s = Mathf.Clamp01(s + Rand01Symmetry * amount);
    }

    public void MutateValue(float amount = 1f)
    {
        v = Mathf.Clamp01(v + Rand01Symmetry * amount);
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

    public void MutateValueLooped(float amount = 1f)
    {
        v = (1f + v + Rand01Symmetry * amount) % 1f;
    }

    public void MutateAlphaLooped(float amount = 1f)
    {
        a = (1f + a + Rand01Symmetry * amount) % 1f;
    }

    public static HSVColor Random(float a = 1f)
    {
        return new HSVColor(Rand01, Rand01, Rand01, a);
    }

    public static HSVColor Random(Vector2 minMaxH, Vector2 minMaxS, Vector2 minMaxV, Vector2? minMaxA = null)
    {
        return new HSVColor(
            UnityEngine.Random.Range(minMaxH.x, minMaxH.y),
            UnityEngine.Random.Range(minMaxS.x, minMaxS.y),
            UnityEngine.Random.Range(minMaxV.x, minMaxV.y),
            minMaxA.HasValue ? UnityEngine.Random.Range(minMaxH.x, minMaxH.y) : 1f);
    }

    public static HSVColor RandomHue(float s, float v, float a = 1f)
    {
        return new HSVColor(Rand01, s, v, a);
    }

    public static HSVColor RandomSaturation(float h, float v, float a = 1f)
    {
        return new HSVColor(h, Rand01, v, a);
    }

    public static HSVColor RandomValue(float h, float s, float a = 1f)
    {
        return new HSVColor(h, s, Rand01, a);
    }

    public static implicit operator HSVColor(Color c)
    {
        return RGBtoHSV(c);
    }

    public static implicit operator Color(HSVColor hsv)
    {
        return HSVtoRGB(hsv);
    }

    public static implicit operator HSVColor(Color32 c32)
    {
        return RGBtoHSV((Color)c32);
    }

    public static implicit operator Color32(HSVColor hsv)
    {
        return HSVtoRGB(hsv);
    }

    public static bool operator !=(HSVColor lhs, HSVColor rhs)
    {
        return !(lhs == rhs);
    }

    public override bool Equals(object other)
    {
        if (other == null)
        {
            return false;
        }

        if (other is HSVColor || other is Color || other is Color32)
        {
            return this == (HSVColor)other;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return h.GetHashCode() ^ s.GetHashCode() ^ v.GetHashCode() ^ a.GetHashCode();
    }

    public static Color HSVtoRGB(HSVColor hsv)
    {
        Vector3 rgb = HUEtoRGB(hsv.h);
        Vector3 vc = ((rgb - Vector3.one) * hsv.s + Vector3.one) * hsv.v;

        return new Color(vc.x, vc.y, vc.z, hsv.a);
    }

    public static HSVColor RGBtoHSV(Color rgb)
    {
        Vector3 hcv = RGBtoHCV(rgb);
        float s = hcv.y / (hcv.z + EPSILON);

        return new HSVColor(hcv.x, s, hcv.z, rgb.a);
    }

    private static Vector3 HUEtoRGB(float h)
    {
        float r = Mathf.Abs(h * 6 - 3) - 1;
        float g = 2 - Mathf.Abs(h * 6 - 2);
        float b = 2 - Mathf.Abs(h * 6 - 4);

        return new Vector3(Mathf.Clamp01(r), Mathf.Clamp01(g), Mathf.Clamp01(b));
    }

    private const float EPSILON = 1e-8f;

    private static Vector3 RGBtoHCV(Color rgb)
    {
        Vector4 p = rgb.g < rgb.b ? new Vector4(rgb.b, rgb.g, -1, 2f / 3f) : new Vector4(rgb.g, rgb.b, 0, -1f / 3f);
        Vector4 q = rgb.r < p.x ? new Vector4(p.x, p.y, p.w, rgb.r) : new Vector4(rgb.r, p.y, p.z, p.x);
        float c = q.x - Mathf.Min(q.w, q.y);
        float h = Mathf.Abs((q.w - q.y) / (6 * c + EPSILON) + q.z);

        return new Vector3(h, c, q.x);
    }
}

