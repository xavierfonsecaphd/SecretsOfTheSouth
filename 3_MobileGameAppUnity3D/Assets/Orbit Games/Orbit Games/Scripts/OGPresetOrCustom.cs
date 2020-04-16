using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OGPresetOrCustom<W, T> : IOGInitializable where W : OGPresetScriptableObject<T> where T : IOGInitializable, new()
{
    public EditorHelpBox presetMessage;
    public ForceUse behaviour;
    public W presetDefinition;
    [Buttons(true, "▲ Copy to preset", "CopyToPreset", "▼ Copy to custom", "CopyToCustom")]
    public ButtonsContainer presetCopying;
    [Buttons(true, "● Save custom definition as preset...", "SaveToPreset")]
    public ButtonsContainer presetSaving;
    public T customDefinition;

    public void EditorHelpBoxUpdate()
    {
        switch (behaviour)
        {
            case ForceUse.UsePresetIfAvailable:
                if (presetDefinition != null)
                {
                    presetMessage = EditorHelpBox.Info("PRESET is in efect");
                }
                else
                {
                    presetMessage = EditorHelpBox.Warning("Custom definition is in effect");
                }
                break;
            case ForceUse.UsePreset:
                if (presetDefinition != null)
                {
                    presetMessage = EditorHelpBox.Info("PRESET is in efect");
                }
                else
                {
                    presetMessage = EditorHelpBox.Error("No preset is set!");
                }
                break;
            case ForceUse.UseCustom:
                presetMessage = EditorHelpBox.Warning("Custom definition is in effect");
                break;
        }
    }

    public enum ForceUse
    {
        UsePresetIfAvailable, UsePreset, UseCustom
    }

    public void Initialize()
    {
        if (presetDefinition != null) presetDefinition.Initialize();
        if (customDefinition != null) customDefinition.Initialize();
    }

    public T Definition
    {
        get
        {
            switch (behaviour)
            {
                case ForceUse.UsePresetIfAvailable:
                    return presetDefinition == null ? customDefinition : presetDefinition.preset;
                case ForceUse.UsePreset:
                    return presetDefinition == null ? default(T) : presetDefinition.preset;
                case ForceUse.UseCustom:
                    return customDefinition == null ? default(T) : customDefinition;
            }
            return default(T);
        }
    }

    public void SaveToPreset()
    {
        var result = OGEditorExtensions.SaveAsPreset<W, T>(customDefinition, "Save to preset", "Animations", true);
        if (result != null)
        {
            presetDefinition = result;
        }
    }

    public void CopyToPreset()
    {
        if (presetDefinition == null)
        {
            Debug.LogError("Can't copy to preset, because there is no preset set");
            return;
        }

        OGEditorExtensions.RecordUndo(presetDefinition, "Copy values to preset");
        OGEditorExtensions.CopyPasteValues(customDefinition, presetDefinition.preset);
        OGEditorExtensions.SaveAsset(presetDefinition);
    }

    public void CopyToCustom()
    {
        if (presetDefinition == null)
        {
            Debug.LogError("Can't copy from preset, because there is no preset set");
            return;
        }
        
        OGEditorExtensions.CopyPasteValues(presetDefinition.preset, customDefinition);
    }
}
