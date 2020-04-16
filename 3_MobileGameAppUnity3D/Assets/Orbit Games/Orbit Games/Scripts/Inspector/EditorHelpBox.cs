using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EditorHelpBox
{
    public enum MessageType
    {
        None = 0,
        Info = 1,
        Warning = 2,
        Error = 3
    }

    public string message;
    public MessageType messageType;
    public bool display;

    public static EditorHelpBox CreateBox(string message, MessageType type, bool display = true)
    {
        return new EditorHelpBox()
        {
            display = display,
            message = message,
            messageType = type
        };
    }

    public static EditorHelpBox None()
    {
        return CreateBox("", MessageType.None, false);
    }

    public static EditorHelpBox Info(string message)
    {
        return CreateBox(message, MessageType.Info);
    }

    public static EditorHelpBox Warning(string message)
    {
        return CreateBox(message, MessageType.Warning);
    }

    public static EditorHelpBox Error(string message)
    {
        return CreateBox(message, MessageType.Error);
    }
}
