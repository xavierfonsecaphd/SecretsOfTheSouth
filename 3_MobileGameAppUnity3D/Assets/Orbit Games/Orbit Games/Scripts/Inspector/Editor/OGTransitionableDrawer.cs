using System;
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer(typeof(OGTransitionable))]
public class OGTransitionableDrawer : PropertyDrawer
{

    // was working on a way to preview animations without having the preset in the scene, but it got a bit difficult for now

    //position.y += position.height;
    //        position.height = 200f;

    //        var prim = GameObject.CreatePrimitive(PrimitiveType.Cube);
    //var previewRenderer = new PreviewRenderUtility();
    //previewRenderer.AddSingleGO(prim);
    //        previewRenderer.BeginPreview(new Rect(0, 0, 200, 200), GUIStyle.none);
    //        previewRenderer.EndAndDrawPreview(position);
    //        Debug.Log(prim.transform.position);
    //        Debug.Log(prim.scene.name);
}

