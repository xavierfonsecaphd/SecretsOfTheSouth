using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;

// built along with http://www.sharkbombs.com/2015/02/17/unity-editor-enum-flags-as-toggle-buttons/

[CustomPropertyDrawer(typeof(EnumButtonsAttribute))]
public class EnumButtonsDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        string[] names = property.enumDisplayNames;
        var columns = (attribute as EnumButtonsAttribute).columns;
        return (base.GetPropertyHeight(property, label)) * Mathf.CeilToInt(names.Length / columns);
    }
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        EditorGUI.LabelField(rect, label);
        rect.x += EditorGUIUtility.labelWidth;
        rect.width -= EditorGUIUtility.labelWidth;

        string[] names = property.enumDisplayNames;
        var selectedIndex = property.enumValueIndex;

        var maxColumns = (attribute as EnumButtonsAttribute).columns;
        var maxRows = Mathf.CeilToInt(names.Length / maxColumns);
        rect.width /= maxColumns;
        rect.height /= maxRows;

        GUIStyle selectedStyle = new GUIStyle();
        selectedStyle.fontStyle = FontStyle.Bold;
        var startX = rect.x;

        for (int i = 0; i < names.Length; i++)
        {
            if (i == selectedIndex)
            {
                GUI.Toggle(rect, true, names[i]);
            }
            else
            {
                bool clicked = GUI.Toggle(rect, false, names[i]);

                if (clicked)
                {
                    property.enumValueIndex = i;
                }
            }

            rect.x += rect.width;
            if ((i + 1) % maxColumns == 0)
            {
                rect.y += rect.height;
                rect.x = startX;
            }
        }
    }
}