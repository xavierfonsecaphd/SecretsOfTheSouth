using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

// TODO: fix the rounding issue where sliding the integer version causes the values to jump due to rounding

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        string value;
        switch (property.propertyType) {
            case SerializedPropertyType.AnimationCurve:
                value = property.animationCurveValue.ToString();
                break;
            case SerializedPropertyType.ArraySize:
                value = property.arraySize.ToString();
                break;
            case SerializedPropertyType.Boolean:
                value = property.boolValue.ToString();
                break;
            case SerializedPropertyType.Bounds:
                value = property.boundsValue.ToString();
                break;
            case SerializedPropertyType.Character:
                value = property.stringValue.ToString();
                break;
            case SerializedPropertyType.Color:
                value = property.colorValue.ToString();
                break;
            case SerializedPropertyType.Enum:
                value = property.enumDisplayNames[property.enumValueIndex];
                break;
            case SerializedPropertyType.Float:
                value = property.floatValue.ToString();
                break;
            case SerializedPropertyType.Integer:
                value = property.intValue.ToString();
                break;
            case SerializedPropertyType.LayerMask:
                value = property.intValue.ToString();
                break;
            case SerializedPropertyType.ObjectReference:
                value = property.objectReferenceValue == null ? "None" : property.objectReferenceValue.ToString();
                break;
            case SerializedPropertyType.Quaternion:
                value = property.quaternionValue.ToString();
                break;
            case SerializedPropertyType.Rect:
                value = property.rectValue.ToString();
                break;
            case SerializedPropertyType.String:
                value = property.stringValue.ToString();
                break;
            case SerializedPropertyType.Vector2:
                value = property.vector2Value.ToString();
                break;
            case SerializedPropertyType.Vector3:
                value = property.vector3Value.ToString();
                break;
            case SerializedPropertyType.Vector4:
                value = property.vector4Value.ToString();
                break;
            default:
                value = "ERROR: Unknown property type";
                break;
        }
        EditorGUI.LabelField(position, label.text, value);
    }
}