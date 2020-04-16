using GameToolkit.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Feature_", menuName = "Construction/Feature Definition")]
public class OGFeatureDefinition : OGBaseDefinition
{
    [Header("Settings")]
    public List<OGFeatureDetailDefinition> details;
    public bool emptyAllowed;

    [Buttons("Rename Details", "RenameDetails")]
    public ButtonsContainer holeNames;
    public void RenameDetails()
    {
#if UNITY_EDITOR

        // check uniqueness
        HashSet<string> ids = new HashSet<string>();
        foreach (var detail in details)
        {
            if (ids.Contains(detail.extraID))
            {
                throw new Exception("Holes have overlapping ids");
            }
            ids.Add(detail.extraID);
        }

        foreach (var detail in details)
        {
            var prefix = GetFilePrefix();
            var newName = GetUniqueID().Replace('.', ' ').Replace('_', ' ').ToTitleCase().Replace(' ', '_');
            if (!prefix.IsNullOrEmpty())
            {
                newName = prefix + "_" + newName;
            }
            newName = newName + "_Detail";
            if (!detail.extraID.IsNullOrEmpty())
            {
                var processed = detail.extraID.Replace('_', ' ').ToTitleCase();
                newName = newName + "_" + processed.Replace(' ', '_').azAZ09_();
            }
            var path = UnityEditor.AssetDatabase.GetAssetPath(detail);
            UnityEditor.AssetDatabase.RenameAsset(path, newName + ".asset");
        }
#endif
    }

    protected override string GetFilePrefix()
    {
        return "Feature";
    }
}
