using GameToolkit.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Socket_", menuName = "Construction/Socket Definition")]
public class OGSocketDefinition : OGBaseDefinition
{
    [Header("Settings")]
    public List<OGSocketHoleDefinition> holes;
    public bool emptyAllowed;

    [Buttons("Rename Holes", "RenameHoles")]
    public ButtonsContainer holeNames;
    public void RenameHoles()
    {
#if UNITY_EDITOR

        // check uniqueness
        HashSet<string> ids = new HashSet<string>();
        foreach (var hole in holes)
        {
            if (ids.Contains(hole.extraID))
            {
                throw new Exception("Holes have overlapping ids");
            }
            ids.Add(hole.extraID);
        }

        foreach (var hole in holes) {
            var prefix = GetFilePrefix();
            var newName = GetUniqueID().Replace('.', ' ').Replace('_', ' ').ToTitleCase().Replace(' ', '_');
            if (!prefix.IsNullOrEmpty())
            {
                newName = prefix + "_" + newName;
            }
            newName = newName + "_Hole";
            if (!hole.extraID.IsNullOrEmpty())
            {
                var processed = hole.extraID.Replace('_', ' ').ToTitleCase();
                newName = newName + "_" + processed.Replace(' ', '_').azAZ09_();
            }
            var path = UnityEditor.AssetDatabase.GetAssetPath(hole);
            UnityEditor.AssetDatabase.RenameAsset(path, newName + ".asset");
        }
#endif
    }

    protected override string GetFilePrefix()
    {
        return "Socket";
    }
}
