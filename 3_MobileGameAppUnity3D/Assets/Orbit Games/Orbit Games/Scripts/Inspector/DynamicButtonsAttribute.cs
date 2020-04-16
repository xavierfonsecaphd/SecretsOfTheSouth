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
public class DynamicButtonsAttribute : PropertyAttribute
{
    public string getButtonsMethod = null;
    public bool tiny;

    public DynamicButtonsAttribute(string getButtonsListMethod, bool tiny = false)
    {
        getButtonsMethod = getButtonsListMethod;
        this.tiny = tiny;
    }
}
