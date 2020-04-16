using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(UnitAttribute))]
public class UnitDrawer : PropertyDrawer
{
    public override void OnGUI(Rect rectangle, SerializedProperty prop, GUIContent label)
    {
        // get the attribute
        var attr = attribute as UnitAttribute;
        float unitWidth = 10 + attr.unit.Length * 7.5f;
        var unitRect = rectangle;
        var propertyRect = rectangle;

        if (attr.prefix)
        {
            // TODO
            // broken because PropertyField draws label and field, and does not allow 
            // moving the field up a bit to make space for a custom prefix
            // need to implement the PropertyField ourselves, will be a hassle
            // to also add functionality of draggable labels in order to increase/decrease
            // floats and ints
            unitRect.xMin = rectangle.xMin;
            unitRect.xMax = unitWidth;

            propertyRect.xMin = unitRect.xMax;
            propertyRect.xMax = rectangle.xMax;
        }
        else
        {
            propertyRect.xMax -= unitWidth;

            unitRect.xMin = propertyRect.xMax;
            unitRect.xMax = unitRect.xMin + unitWidth;
        }

        EditorGUI.PropertyField(propertyRect, prop);

        var temp = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        EditorGUI.LabelField(unitRect, attr.unit);
        EditorGUI.indentLevel = temp;
    }
}