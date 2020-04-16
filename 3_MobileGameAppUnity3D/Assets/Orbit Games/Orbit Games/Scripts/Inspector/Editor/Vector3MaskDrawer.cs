using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Vector3Mask))]
public class Vector3MaskDrawer : PropertyDrawer
{

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(SerializedPropertyType.Boolean, label);
    }

    // TODO: Skip array fields
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        var x = property.FindPropertyRelative("x").boolValue;
        var y = property.FindPropertyRelative("y").boolValue;
        var z = property.FindPropertyRelative("z").boolValue;
        var w = property.FindPropertyRelative("w").boolValue;

        EditorGUI.LabelField(rect, label);
        rect.x += EditorGUIUtility.labelWidth;
        rect.width -= EditorGUIUtility.labelWidth;
        rect.width /= 4;

        w = GUI.Toggle(rect, w, "All"); rect.x += rect.width;

        EditorGUI.BeginDisabledGroup(w);
        x = GUI.Toggle(rect, x, "x"); rect.x += rect.width;
        y = GUI.Toggle(rect, y, "y"); rect.x += rect.width;
        z = GUI.Toggle(rect, z, "z"); rect.x += rect.width;
        EditorGUI.EndDisabledGroup();

        property.FindPropertyRelative("x").boolValue = x;
        property.FindPropertyRelative("y").boolValue = y;
        property.FindPropertyRelative("z").boolValue = z;
        property.FindPropertyRelative("w").boolValue = w;
    }
}
