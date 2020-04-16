using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EditorHelpBox))]
public class EditorHelpBoxDrawer : PropertyDrawer {

    SerializedProperty message, messageType, display;

    private void EnsureValues(SerializedProperty property)
    {
        message = property.FindPropertyRelative("message");
        messageType = property.FindPropertyRelative("messageType");
        display = property.FindPropertyRelative("display");
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        EnsureValues(property); 
        if (display.propertyType != SerializedPropertyType.Boolean) return -EditorGUIUtility.singleLineHeight;
        if (message.propertyType != SerializedPropertyType.String) return - EditorGUIUtility.singleLineHeight;
        if (messageType.propertyType != SerializedPropertyType.Enum) return - EditorGUIUtility.singleLineHeight;

        //var indented = EditorGUI.IndentedRect(new Rect(0, 0, EditorGUIUtility.currentViewWidth, 0));
        var width = EditorGUIUtility.currentViewWidth - EditorGUIUtility.labelWidth;
        var height = GUI.skin.box.CalcHeight(new GUIContent(message.stringValue), width);
        return display.boolValue ? Mathf.Max(20f, height) + 10f : 0f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // call the helpbox update method
        CallUpdate(property);

        EnsureValues(property);
        if (display.propertyType != SerializedPropertyType.Boolean) return;
        if (message.propertyType != SerializedPropertyType.String) return;
        if (messageType.propertyType != SerializedPropertyType.Enum) return;
        if (!display.boolValue) return;

        position.x += EditorGUIUtility.labelWidth;
        position.width -= EditorGUIUtility.labelWidth;
        EditorGUI.HelpBox(position, message.stringValue, (MessageType)messageType.enumValueIndex);
    }

    private static void CallUpdate(SerializedProperty property)
    {
        var parent = OGEditorExtensions.GetParentObject(property);
        if (parent == null) return;
        var method = OGEditorExtensions.GetMethod(parent, "EditorHelpBoxUpdate");
        if (method == null)
        {
            Debug.LogError("Missing 'EditorHelpBoxUpdate' method on object " + parent + " in property path " + property.propertyPath);
            return;
        }
        method.Invoke(parent, new object[0]);
    }
}
