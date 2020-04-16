using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;

// built along with http://www.sharkbombs.com/2015/02/17/unity-editor-enum-flags-as-toggle-buttons/

[CustomPropertyDrawer(typeof(DynamicButtonsAttribute))]
public class DynamicButtonDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var attr = (attribute as DynamicButtonsAttribute);
        return base.GetPropertyHeight(property, label) + (attr.tiny ? 0f : 10f);
    }
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        var attr = (attribute as DynamicButtonsAttribute);
        var targetObject = OGEditorExtensions.GetParentObject(property);
        var buttonsMethod = OGEditorExtensions.GetMethod(targetObject, attr.getButtonsMethod);
        List<EditorButton> buttons = buttonsMethod.Invoke(targetObject, new object[0]) as List<EditorButton>;
        if (buttons == null)
        {
            Debug.LogError("Method '" + attr.getButtonsMethod + "' to generate buttons was not found.");
            return;
        }

        ButtonDrawer.DrawButtons(rect, property, label, buttons);
    }
}