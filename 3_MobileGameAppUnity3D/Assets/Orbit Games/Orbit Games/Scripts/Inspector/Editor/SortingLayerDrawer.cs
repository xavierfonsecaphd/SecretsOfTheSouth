using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(SortingLayerAttribute))]
public class SortingLayerDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect positionRect, SerializedProperty serializedProperty, GUIContent propertyLabel)
    {
        List<string> layerNames = OGListPool<string>.Get();
        var layers = SortingLayer.layers;
        for (int i = 0; i < layers.Length; i++)
            layerNames.Add(layers[i].name);

        if (serializedProperty.propertyType == SerializedPropertyType.Integer)
        {
            //EditorGUI.BeginProperty(positionRect, propertyLabel, serializedProperty);

            // find the layer that was selected
            string previouslySelected = SortingLayer.IDToName(serializedProperty.intValue);
            int previouslySelectedIndex = layerNames.IndexOf(previouslySelected);
            
            // obtaining a possibily newly selected index
            int newlySelectedIndex = EditorGUI.Popup(positionRect, propertyLabel.text, previouslySelectedIndex, layerNames.ToArray());

            // set the new int value if the index had changed
            if (newlySelectedIndex != previouslySelectedIndex)
            {
                serializedProperty.intValue = SortingLayer.NameToID(layerNames[newlySelectedIndex]);
            }

            //EditorGUI.EndProperty();
        }
        else
        {
            EditorGUI.LabelField(positionRect, "Error: not an integer, or no sorting layers found");
        }

        OGListPool<string>.Put(ref layerNames);
    }
}