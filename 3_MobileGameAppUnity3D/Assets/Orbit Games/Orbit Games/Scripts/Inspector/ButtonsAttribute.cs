using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Allows you to create buttons that run methods on your serializable objects. On a monobehaviour or a serializable object 
/// within your monobehaviour (can be as deep as unity allows list nesting in the inspector), you can use a bunch of components 
/// and/or gameobject as input which will then be dependency injected by getting them
/// from the GameObject that the script is currently attached to. 
/// 
/// e.g.
/// public void doSomeStuff(GameObject thisGameObject, Transform transform)
/// 
/// This is especially useful when you wish to get the actual gameobject within a nested serialized object
/// </summary>
public class ButtonsAttribute : PropertyAttribute
{
    public List<EditorButton> buttons = new List<EditorButton>();
    public bool tiny;

    public ButtonsAttribute(string method)
    {
        buttons.Add(new EditorButton(method, method));
    }

    public ButtonsAttribute(string[] labels, string[] methods)
    {
        for (int i = 0; i < labels.Length; i++)
        {
            buttons.Add(new EditorButton(labels[i], methods[i]));
        }
    }

    public ButtonsAttribute(params string[] labelMethodPairs)
    {
        for (int i = 0; i < labelMethodPairs.Length; i += 2)
        {
            buttons.Add(new EditorButton(labelMethodPairs[i], labelMethodPairs[i + 1]));
        }
    }

    public ButtonsAttribute(bool tiny, string method)
    {
        this.tiny = tiny;
        buttons.Add(new EditorButton(method, method));
    }

    public ButtonsAttribute(bool tiny, string[] labels, string[] methods)
    {
        this.tiny = tiny;
        for (int i = 0; i < labels.Length; i++)
        {
            buttons.Add(new EditorButton(labels[i], methods[i]));
        }
    }

    public ButtonsAttribute(bool tiny, params string[] labelMethodPairs)
    {
        this.tiny = tiny;
        for (int i = 0; i < labelMethodPairs.Length; i += 2)
        {
            buttons.Add(new EditorButton(labelMethodPairs[i], labelMethodPairs[i + 1]));
        }
    }

    public ButtonsAttribute(EditorButton btn)
    {
        buttons.Add(btn);
    }

    public ButtonsAttribute(params EditorButton[] btns)
    {
        foreach (var btn in btns)
        {
            buttons.Add(btn);
        }
    }
}

public class EditorButton
{
    public string method;
    public string label;

    public EditorButton(string method)
    {
        this.label = method;
        this.method = method;
    }

    public EditorButton(string label, string method)
    {
        this.label = label;
        this.method = method;
    }
}