using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;

// built along with http://www.sharkbombs.com/2015/02/17/unity-editor-enum-flags-as-toggle-buttons/

[CustomPropertyDrawer(typeof(QuickButtonsAttribute))]
public class QuickButtonsDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position.x += EditorGUIUtility.labelWidth;
        position.width -= EditorGUIUtility.labelWidth;
        var attr = (QuickButtonsAttribute)attribute;
        position.width /= attr.showUpDownButtons ? 4 : 2;

        var parent = OGEditorExtensions.GetParentObject(property);

        if (GUI.Button(position, "◇ Copy"))
        {
            OGEditorExtensions.CopyValues(parent);
        }

        position.x += position.width;

        if (GUI.Button(position, "◆ Paste"))
        {
            Undo.RecordObject(parent as UnityEngine.Object, "Paste values");
            OGEditorExtensions.PasteValues(parent);
        }

        if (attr.showUpDownButtons)
        {
            position.x += position.width;

            if (GUI.Button(position, "▴ Up"))
            {
                UnityEditorInternal.ComponentUtility.MoveComponentUp((property.serializedObject.targetObject as Component));
            }

            position.x += position.width;

            if (GUI.Button(position, "▾ Down"))
            {
                UnityEditorInternal.ComponentUtility.MoveComponentDown((property.serializedObject.targetObject as Component));
            }
        }
    }
}