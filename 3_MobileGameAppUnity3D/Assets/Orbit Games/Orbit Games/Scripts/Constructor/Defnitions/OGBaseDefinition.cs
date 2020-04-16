using GameToolkit.Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OGBaseDefinition : LocalizedAsset<string>
{
    [Header("Internal ID")]
    [SerializeField]
    private string uniqueID;
    [Buttons(true, "Generate unique ID", "Generate")]
    public ButtonsContainer generate;
    [Buttons("From Title", "FromTitle", "To Title", "ToTitle", "To Name", "ToName")]
    public ButtonsContainer nameConversion;

    public void Generate()
    {
        uniqueID = System.Guid.NewGuid().ToString().Substring(0, 6);
        OGEditorExtensions.SaveAsset(this);
    }

    public void FromTitle()
    {
        uniqueID = GetTitle().Replace(' ', '_').azAZ09_().ToLower();
        OGEditorExtensions.SaveAsset(this);
    }

    public void ToTitle()
    {
        m_LocaleItems[0].Value = uniqueID.Replace('_',' ').ToTitleCase();
        OGEditorExtensions.SaveAsset(this);
    }

    public string GetNameFromID()
    {
        var prefix = GetFilePrefix();
        var newName = GetUniqueID().Replace('.', ' ').Replace('_', ' ').ToTitleCase().Replace(' ', '_');
        if (!prefix.IsNullOrEmpty())
        {
            newName = prefix + "_" + newName;
        }
        return newName;
    }

    public void ToName()
    {
#if UNITY_EDITOR
        var newName = GetNameFromID();
        var path = UnityEditor.AssetDatabase.GetAssetPath(this);
        UnityEditor.AssetDatabase.RenameAsset(path, newName + ".asset");

        OGEditorExtensions.SaveAsset(this);
#endif
    }

    protected abstract string GetFilePrefix();

    public virtual string GetUniqueID()
    {
        return uniqueID;
    }

    protected void SetUniqueID(string uniqueID)
    {
        this.uniqueID = uniqueID;
    }

    public void ForceOverrideUniqueID(string uniqueID)
    {
        this.uniqueID = uniqueID;
    }

    [Header("Title")]
    [SerializeField]
    private TextLocaleItem[] m_LocaleItems = new TextLocaleItem[1];
    public override LocaleItemBase[] LocaleItems { get { return m_LocaleItems; } }
    [System.Serializable]
    private class TextLocaleItem : LocaleItem<string> { };

    public override string ToString()
    {
        return name + " (" + GetType() + ")";
    }

    public string GetTitle()
    {
        return m_LocaleItems[0].Value;
    }

}
