using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.Events;

[CustomPropertyDrawer(typeof(ConditionalFieldAttribute))]
public class ConditionalFieldAttributeDrawer : PropertyDrawer
{
    // some potential upgrades
    // http://www.brechtos.com/hiding-or-disabling-inspector-properties-using-propertydrawers-within-unity-5/#comment-1359

    private ConditionalFieldAttribute Attribute
    {
        get
        {
            return _attribute ?? (_attribute = attribute as ConditionalFieldAttribute);
        }
    }

    private Condition[] Conditions
    {
        get
        {
            return Attribute != null ? _attribute.Conditions : null;
        }
    }

    private bool OnlyDisableField
    {
        get
        {
            return Attribute != null ? _attribute.OnlyDisableField : false;
        }
    }

    private bool OrConditions
    {
        get
        {
            return Attribute != null ? _attribute.OrConditions : false;
        }
    }

    private ConditionalFieldAttribute _attribute;

    private string GetStringValue(SerializedProperty conditionProperty)
    {
        string conditionPropertyStringValue = "";
        switch (conditionProperty.propertyType)
        {
            case SerializedPropertyType.Integer:
                conditionPropertyStringValue = conditionProperty.intValue.ToString().ToUpper();
                break;
            case SerializedPropertyType.Boolean:
                conditionPropertyStringValue = conditionProperty.boolValue.ToString().ToUpper();
                break;
            case SerializedPropertyType.Float:
                conditionPropertyStringValue = conditionProperty.floatValue.ToString().ToUpper();
                break;
            case SerializedPropertyType.Color:
                conditionPropertyStringValue = conditionProperty.colorValue.ToString().ToUpper();
                break;
            case SerializedPropertyType.ObjectReference:
                conditionPropertyStringValue = conditionProperty.objectReferenceValue == null ? "NULL" : conditionProperty.objectReferenceValue.ToString().ToUpper();
                break;
            case SerializedPropertyType.Enum:
                conditionPropertyStringValue = conditionProperty.enumNames.GetOrDefault(conditionProperty.enumValueIndex, "").ToString().ToUpper();
                break;
            case SerializedPropertyType.Vector2:
                conditionPropertyStringValue = conditionProperty.vector2Value.ToString().ToUpper();
                break;
            case SerializedPropertyType.Vector3:
                conditionPropertyStringValue = conditionProperty.vector3Value.ToString().ToUpper();
                break;
            case SerializedPropertyType.Vector4:
                conditionPropertyStringValue = conditionProperty.vector4Value.ToString().ToUpper();
                break;
            case SerializedPropertyType.Vector2Int:
                conditionPropertyStringValue = conditionProperty.vector2IntValue.ToString().ToUpper();
                break;
            case SerializedPropertyType.Vector3Int:
                conditionPropertyStringValue = conditionProperty.vector3IntValue.ToString().ToUpper();
                break;
            case SerializedPropertyType.Character:
            case SerializedPropertyType.String:
            default:
                conditionPropertyStringValue = conditionProperty.stringValue.ToString().ToUpper();
                break;
        }
        return conditionPropertyStringValue;
    }

    private bool ShouldShowAttribute(SerializedProperty property)
    {
        var conditions = Conditions;
        var verdict = !OrConditions;
        if (!conditions.IsNullOrEmpty())
        {
            for (int i = 0; i < conditions.Length; i++)
            {
                var condition = conditions[i];
                if (!condition.compareValues.IsNullOrEmpty())
                {
                    var conditionProperty = FindPropertyRelative(property, condition.propertyToCheck);
                    var value = false;
                    if (conditionProperty != null)
                    {
                        var matched = false;
                        for (int v = 0; v < condition.compareValues.Length; v++)
                        {
                            string compareStringValue = condition.compareValues[v] == null 
                                ? "NULL" : condition.compareValues[v].ToString().ToUpper();

                            var conditionPropertyStringValue = GetStringValue(conditionProperty);
                            if (compareStringValue == conditionPropertyStringValue)
                            {
                                matched = true;
                                break;
                            }
                        }

                        value = (matched && condition.shouldEqual) || (!matched && !condition.shouldEqual);
                    }
                    else
                    {
                        object propertyParent = GetParentObject(property);
                        if (propertyParent != null)
                        {
                            var type = propertyParent.GetType();
                            var propertyRef = type.GetProperty(condition.propertyToCheck);
                            if (propertyRef != null)
                            {
                                value = (bool)(propertyRef.GetValue(propertyParent, new object[0]));
                            }
                        }
                    }

                    if (OrConditions)
                    {
                        verdict |= value;
                    }
                    else
                    {
                        verdict &= value;
                    }
                }
            }
        }
        return verdict;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (ShouldShowAttribute(property) || OnlyDisableField)
        {
            return EditorGUI.GetPropertyHeight(property);
        }
        else
        {
            return -EditorGUIUtility.standardVerticalSpacing;
        }
    }

    // TODO: Skip array fields
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var shouldBeEnabled = ShouldShowAttribute(property);
        if (shouldBeEnabled || OnlyDisableField)
        {
            EditorGUI.BeginDisabledGroup(!shouldBeEnabled);
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndDisabledGroup();
        }
    }

    private SerializedProperty FindPropertyRelative(SerializedProperty property, string toGet)
    {
        if (property.depth == 0) return property.serializedObject.FindProperty(toGet);

        var path = property.propertyPath.Replace(".Array.data[", "[");
        var elements = path.Split('.');
        SerializedProperty parent = null;


        for (int i = 0; i < elements.Length - 1; i++)
        {
            var element = elements[i];
            int index = -1;
            if (element.Contains("["))
            {
                index = Convert.ToInt32(element.Substring(element.IndexOf("[", StringComparison.Ordinal)).Replace("[", "").Replace("]", ""));
                element = element.Substring(0, element.IndexOf("[", StringComparison.Ordinal));
            }

            parent = i == 0 ?
                property.serializedObject.FindProperty(element) :
                parent.FindPropertyRelative(element);

            if (index >= 0) parent = parent.GetArrayElementAtIndex(index);
        }

        return parent.FindPropertyRelative(toGet);
    }

    public static object GetParentObject(SerializedProperty property)
    {
        string[] path = property.propertyPath.Split('.');

        object propertyObject = property.serializedObject.targetObject;
        object propertyParent = null;
        for (int i = 0; i < path.Length; ++i)
        {
            if (path[i] == "Array")
            {
                int index = (int)(path[i + 1][path[i + 1].Length - 2] - '0');
                var count = ((IList)propertyObject).Count;
                if (index >= count) return null;
                propertyObject = ((IList)propertyObject)[index];
                ++i;
            }
            else
            {
                propertyParent = propertyObject;
                propertyObject = propertyObject.GetType().GetField(path[i]).GetValue(propertyObject);
            }
        }

        return propertyParent;
    }

}