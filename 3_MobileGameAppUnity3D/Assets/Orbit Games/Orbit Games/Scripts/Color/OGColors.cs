using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class OGColors : OGSingletonBehaviour<OGColors> {

    protected override void OnSingletonInitialize() { }

#if UNITY_EDITOR
    public UnityEngine.Object colorsPreset;
    public Color testColor;

    [Buttons("Add from presets", "AddFromPresetLibrary", "Clear", "Clear", "Test", "Test")]
    public ButtonsContainer editorButton;
    public void AddFromPresetLibrary()
    {
        SerializedObject so = new SerializedObject(colorsPreset);
        SerializedProperty presets = so.FindProperty("m_Presets");
        for (int i = 0; i < presets.arraySize; i++)
        {
            SerializedProperty element = presets.GetArrayElementAtIndex(i);
            colors.Add(ExtractColorPreset(
                element.FindPropertyRelative("m_Name").stringValue,
                element.FindPropertyRelative("m_Color").colorValue));
        }
    }
    public void Test()
    {
        colors.Add(ExtractColorPreset("test", testColor));
    }

    public void Clear()
    {
        colors.Clear();
    }

#endif

    public ColorPreset ExtractColorPreset(string name, Color color)
    {
        var hsl = color.ToHSL();
        var greyness = hsl.L < 0.01f ? Hue.Black : (hsl.L > 0.99f ? Hue.White : Hue.Green);
        var hue = hsl.s == 0f ? greyness : (Hue)Mathf.FloorToInt(((hsl.h + 0.041666667f) % 1) * 11.999999f);
        var lightness = (Lightness)Mathf.FloorToInt(hsl.L * 6.999999f);
        var saturation = (Saturation)Mathf.FloorToInt(hsl.s * 3.999999f);
        var emotion = (hsl.s == 0f) ? Emotion.None 
            : (Emotion)Mathf.Clamp(Mathf.FloorToInt(((hsl.h + 0.08333f) % 1f) * 5.9999999f), 0, 4);

        return new ColorPreset()
        {
            name = name,
            color = color,
            hue = hue,
            lightness = lightness,
            saturation = saturation,
            emotion = emotion
        };
    }

    public List<Hue> GetRandomUniqueHues(int hueCount = 3, bool excludeBack = true, bool excludeWhite = true, bool excludeGrey = true)
    {
        List<Hue> hues = new List<Hue>();

        int maxIterations = 10000;

        for (int i = 0; i < hueCount; i++)
        {
            var randomColor = GetRandomColor();
            while ((excludeBack && randomColor.hue == Hue.Black)
                || (excludeWhite && randomColor.hue == Hue.White)
                || (excludeGrey && randomColor.hue == Hue.Grey)
                || hues.Contains(randomColor.hue))
            {
                randomColor = GetRandomColor();

                // just in case we couldn't find anything even after so many tries
                maxIterations--;
                if (maxIterations < 0)
                {
                    Debug.LogError("We tried and tried, but could not find a suitable amount of unique hues");
                    return hues;
                }
            }
            hues.Add(randomColor.hue);
        }

        return hues;
    }

    public List<Type> GetRandomUniqueColorTypes(int hueCount = 3, bool excludeOther = true)
    {
        List<Type> types = new List<Type>();

        int maxIterations = 10000;

        for (int i = 0; i < hueCount; i++)
        {
            var randomColor = GetRandomColor();
            while ((excludeOther && randomColor.type == Type.Other)
                || types.Contains(randomColor.type))
            {
                randomColor = GetRandomColor();

                // just in case we couldn't find anything even after so many tries
                maxIterations--;
                if (maxIterations < 0)
                {
                    Debug.LogError("We tried and tried, but could not find a suitable amount of unique types");
                    return types;
                }
            }
            types.Add(randomColor.type);
        }

        return types;
    }

    public ColorPreset GetRandomColor(Hue withHue)
    {
        List<int> indices = OGListPool<int>.Get();
        for (int i = 0; i < colors.Count; i++)
        {
            if (colors[i].hue == withHue)
            {
                indices.Add(i);
            }
        }

        if (indices.Count == 0)
            return null;

        var preset = colors[indices[Random.Range(0, indices.Count)]];
        OGListPool<int>.Put(ref indices);
        return preset;
    }

    public ColorPreset GetRandomColor(Saturation withSaturation)
    {
        List<int> indices = OGListPool<int>.Get();
        for (int i = 0; i < colors.Count; i++)
        {
            if (colors[i].saturation == withSaturation)
            {
                indices.Add(i);
            }
        }

        if (indices.Count == 0)
            return null;

        var preset = colors[indices[Random.Range(0, indices.Count)]];
        OGListPool<int>.Put(ref indices);
        return preset;
    }

    public ColorPreset GetRandomColor(Lightness withLightness)
    {
        List<int> indices = OGListPool<int>.Get();
        for (int i = 0; i < colors.Count; i++)
        {
            if (colors[i].lightness == withLightness)
            {
                indices.Add(i);
            }
        }

        if (indices.Count == 0)
            return null;

        var preset = colors[indices[Random.Range(0, indices.Count)]];
        OGListPool<int>.Put(ref indices);
        return preset;
    }

    public ColorPreset GetRandomColor()
    {
        return colors[Random.Range(0, colors.Count)];
    }

    public ColorPreset GetColor(Hue hue, Lightness lightness)
    {
        foreach (var preset in colors)
        {
            if (preset.hue == hue && preset.lightness == lightness)
            {
                return preset;
            }
        }
        return null;
    }

    public ColorPreset GetColor(Hue hue, Lightness lightness, Saturation saturation)
    {
        foreach (var preset in colors)
        {
            if (preset.hue == hue && preset.lightness == lightness && preset.saturation == saturation)
            {
                return preset;
            }
        }
        return null;
    }

    public ColorPreset GetColor(Type type, Lightness lightness)
    {
        foreach (var preset in colors)
        {
            if (preset.type == type && preset.lightness == lightness)
            {
                return preset;
            }
        }
        return null;
    }

    public ColorPreset GetColor(Type type, Lightness lightness, Saturation saturation)
    {
        foreach (var preset in colors)
        {
            if (preset.type == type && preset.lightness == lightness && preset.saturation == saturation)
            {
                return preset;
            }
        }
        return null;
    }

    public enum Emotion
    {
        Negative, Warning, Positive, Calm, None
    }

    public enum Lightness
    {
        Deep, Shadow, Dim, Full, Glow, Shine, Bright
    }

    public enum Saturation
    {
        None, Muted, Pastel, Full
    }

    public enum Hue
    {
        Red, Orange, Yellow, Pistachio, Green, Aquamarine, Cyan, Azure, Blue, Violet, Magenta, Rose, Black, Grey, White
    }

    public enum Type
    {
        Primary, Alternative1, Alternative2, Complement, Bonus1, Bonus2, Other
    }

    [System.Serializable]
    public class ColorPreset
    {
        public string name;
        public Color color;
        public Type type;
        public Hue hue;
        public Saturation saturation;
        public Emotion emotion;
        public Lightness lightness;
    }

    public List<ColorPreset> colors;
}
