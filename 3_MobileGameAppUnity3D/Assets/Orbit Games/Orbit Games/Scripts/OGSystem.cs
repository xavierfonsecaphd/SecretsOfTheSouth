
using System;
using System.IO;
using System.Linq;
using UnityEngine;

public static class OGSystem  {

    public static void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public static void OpenURL(string url)
    {
        if (Directory.Exists(url))
        {
#if UNITY_STANDALONE_OSX
            //OpenInMacFileBrowser(url);
#elif UNITY_EDITOR_OSX
            //OpenInMacFileBrowser(url);
#else
            Application.OpenURL(url);
#endif
        }
        else
        {
            Application.OpenURL(url);
        }
    }

    public static Resolution GetMaxResolution()
    {
        var resolutions = Screen.resolutions;
        var bestResolution = new Resolution();
        foreach (var resolution in resolutions)
        {
            if (resolution.width >= bestResolution.width
                && resolution.height >= bestResolution.height)
            {
                bestResolution = resolution;
            }
        }
        return bestResolution;
    }

    // https://answers.unity.com/questions/43422/how-to-implement-show-in-explorer.html
    private static void OpenMacFinderAtPath(string path)
    {
        bool openInsidesOfFolder = false;

        // try mac
        string macPath = path.Replace("\\", "/"); // mac finder doesn't like backward slashes

        if (Directory.Exists(macPath)) // if path requested is a folder, automatically open insides of that folder
        {
            openInsidesOfFolder = true;
        }

        //Debug.Log("macPath: " + macPath);
        //Debug.Log("openInsidesOfFolder: " + openInsidesOfFolder);

        if (!macPath.StartsWith("\""))
        {
            macPath = "\"" + macPath;
        }
        if (!macPath.EndsWith("\""))
        {
            macPath = macPath + "\"";
        }
        string arguments = (openInsidesOfFolder ? "" : "-R ") + macPath;
        //Debug.Log("arguments: " + arguments);
        try
        {
            System.Diagnostics.Process.Start("open", arguments);
        }
        catch (Exception e)
        {
            Debug.LogError("Can't open " + path + " in Mac finder");
            Debug.LogException(e);
        }
    }
}
