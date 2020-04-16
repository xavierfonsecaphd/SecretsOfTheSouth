using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

// TODO: fix the rounding issue where sliding the integer version causes the values to jump due to rounding

[CustomPropertyDrawer(typeof(HeightAttribute))]
public class HeightDrawer : PropertyDrawer
{
    CurvePropertyDrawer curvePropertyDrawer;

    public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
    {
        var attr = attribute as HeightAttribute;
        return attr.height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.AnimationCurve)
        {
            if (curvePropertyDrawer == null) curvePropertyDrawer = new CurvePropertyDrawer();
            curvePropertyDrawer.OnGUI(position, property, label);
        } 
        else
        {
            EditorGUI.PropertyField(position, property, label);
        }
    }
}