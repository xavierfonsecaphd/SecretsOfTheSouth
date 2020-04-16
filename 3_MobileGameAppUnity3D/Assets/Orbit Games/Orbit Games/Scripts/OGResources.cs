using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OGResources {

    private static HashSet<string> loadedFolders = new HashSet<string>();
    public static void LoadAll(string subfolder, bool force = false)
    {
        if (loadedFolders.Contains(subfolder) && !force) return;
        loadedFolders.Add(subfolder);
        Resources.LoadAll(subfolder);
    }
    public static T[] FindObjectsOfTypeAll<T>(string subfolder) where T : Object
    {
        LoadAll(subfolder);
        return Resources.FindObjectsOfTypeAll<T>();
    }
    public static Object[] FindObjectsOfTypeAll(string subfolder, System.Type type)
    {
        LoadAll(subfolder);
        return Resources.FindObjectsOfTypeAll(type);
    }
}
