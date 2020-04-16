using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;

// built along with http://www.sharkbombs.com/2015/02/17/unity-editor-enum-flags-as-toggle-buttons/

[CustomPropertyDrawer(typeof(ButtonsAttribute))]
public class ButtonDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var attr = (attribute as ButtonsAttribute);
        return base.GetPropertyHeight(property, label) + (attr.tiny ? 0f : 10f);
    }
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        var attr = (attribute as ButtonsAttribute);
        List<EditorButton> buttons = attr.buttons;
        DrawButtons(rect, property, label, buttons);
    }

    public static void DrawButtons(Rect rect, SerializedProperty property, GUIContent label, List<EditorButton> buttons)
    {
        EditorGUI.LabelField(rect, label);
        rect.x += EditorGUIUtility.labelWidth;
        rect.width -= EditorGUIUtility.labelWidth;

        rect.width /= buttons.Count;
        foreach (EditorButton btn in buttons)
        {
            bool clicked = GUI.Toggle(rect, false, btn.label, "Button");
            if (clicked)
            {
                System.Object obj = OGEditorExtensions.GetParentObject(property);
                Component component = property.serializedObject.targetObject as Component;
                MethodInfo[] methods = obj.GetType().GetMethods();
                foreach (var method in methods)
                {
                    if (method.Name.Equals(btn.method))
                    {
                        var parameters = method.GetParameters();
                        var arguments = new object[parameters.Length];

                        for (int p = 0; p < parameters.Length; p++)
                        {
                            var parameterType = parameters[p].ParameterType;
                            if (component != null && parameterType.IsSubclassOf(typeof(Component)))
                            {
                                arguments[p] = component.GetComponent(parameterType);
                            }
                            else if (component != null && parameterType == typeof(GameObject))
                            {
                                arguments[p] = component.gameObject;
                            }
                            else
                            {
                                arguments[p] = GetDefault(parameterType);
                            }
                        }

                        if (component != null)
                        {
                            OGEditorExtensions.RecordUndo(component, "Press button " + btn.label + " to call method " + btn.method);
                        }

                        method.Invoke(obj, arguments);
                        break;
                    }
                }
            }
            rect.x += rect.width;
        }
    }

    public static object GetDefault(Type type)
    {
        if (type.IsValueType)
        {
            return Activator.CreateInstance(type);
        }
        return null;
    }
}