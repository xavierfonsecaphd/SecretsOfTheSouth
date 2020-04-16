using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OGPresetScriptableObject<T> : ScriptableObject where T : IOGInitializable, new()
{
    public T preset;

    public void Awake()
    {
        if (preset == null)
        {
            preset = new T();
            preset.Initialize();
        }
    }

    public void Initialize()
    {
        preset.Initialize();
    }
}
