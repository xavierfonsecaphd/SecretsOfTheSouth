using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(OGAnimationCurvePreset))]
public class OGAnimationCurvePresetDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 10f;
    }

    public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
    {
        var propertyRect = pos;
        propertyRect.height = EditorGUIUtility.singleLineHeight;

        var curveRect = pos;
        curveRect.y += EditorGUIUtility.singleLineHeight;
        curveRect.height = EditorGUIUtility.singleLineHeight * 9;
        curveRect.width -= EditorGUIUtility.labelWidth;
        curveRect.x = EditorGUIUtility.labelWidth;

        EditorGUI.PropertyField(propertyRect, prop);

        var reference = prop.objectReferenceValue;
        if (reference == null)
        {
            EditorGUI.CurveField(curveRect, AnimationCurve.Linear(0, 0, 1, 1));
        }
        else
        {
            var preset = (OGAnimationCurvePreset)reference;
            EditorGUI.CurveField(curveRect, preset.curve);
        }
    }
} 
