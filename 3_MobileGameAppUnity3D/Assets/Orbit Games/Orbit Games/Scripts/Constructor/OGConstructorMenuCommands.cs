using Fenderrio.ImageWarp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class OGConstructorMenuCommands
{
#if UNITY_EDITOR
    [MenuItem("GameObject/Construction/Construction", false, 0)]
    static void CreateConstruction(MenuCommand menuCommand)
    {
        var context = menuCommand.context;
        if (context is GameObject)
        {
            CreateConstruction((GameObject)context);
        }
    }

    public static void CreateConstruction(GameObject gameObject)
    {
        GameObject go = new GameObject("Construction", typeof(RectTransform));
        Undo.RegisterCreatedObjectUndo(go, "Create Construction");
        var constructor = go.AddComponent<OGConstructor>();
        constructor.fromJson = new OGConstructionPlan().ToJson();

        go.transform.SetParent(gameObject.transform);
        go.ResetTransform();

        var socket = go.AddComponent<OGSocket>();
        var socketHole = go.AddComponent<OGSocketHole>();

        var constructionDefinition = OGConstructionManager.I.mainConstructionSocket;
        if (constructionDefinition == null || constructionDefinition.holes == null || constructionDefinition.holes.Count == 0)
        {
            throw new System.Exception("Please make sure the OGConstructionManager component exists in your scene, and a main socket has been defined with its hole");
        }
        socket.definition = constructionDefinition;
        socketHole.definition = constructionDefinition.holes[0];
        socket.FindHoles();

        Selection.activeObject = go;
    }

    [MenuItem("GameObject/Construction/Socket", false, 0)]
    static void CreateSocket(MenuCommand menuCommand)
    {
        var context = menuCommand.context;
        if (context is GameObject)
        {
            CreateSocket((GameObject)context);
        }
    }

    public static void CreateSocket(GameObject gameObject)
    {
        GameObject go = new GameObject(":: SOCKET", typeof(RectTransform));
        Undo.RegisterCreatedObjectUndo(go, "Create Socket");

        go.transform.SetParent(gameObject.transform);
        go.ResetTransform();

        Selection.activeObject = go;
    }

    [MenuItem("GameObject/Construction/Generate Socket Holes", false, 1)]
    static void GenerateSocketHoles(MenuCommand menuCommand)
    {
        var context = menuCommand.context;
        if (context is GameObject)
        {
            GenerateSocketHoles((GameObject)context);
        }
    }
    
    public static void GenerateSocketHoles(GameObject gameObject)
    {
        var socket = gameObject.GetComponent<OGSocket>();

        if (socket == null)
        {
            throw new System.Exception("Object is missing a socket component");
        }

        if (socket.definition == null)
        {
            throw new System.Exception("Object is missing a socket definition");
        }

        if (socket.definition.holes.Count == 0)
        {
            throw new System.Exception("Socket definition is missing hole definitions");
        }

        var plug = gameObject.GetComponent<OGPlug>();
        if (plug == null)
        {
            socket.gameObject.name = ":: SOCKET " + socket.definition.GetTitle();
        }

        if (socket.holes.IsNullOrEmpty())
        {
            socket.holes = new List<OGSocketHole>();
        }

        foreach (var hole in socket.definition.holes)
        {
            GameObject holeObject = new GameObject(". HOLE " + socket.definition.GetTitle() + (hole.extraID.IsNullOrEmpty() ? "" : " " + hole.extraID), typeof(RectTransform));
            Undo.RegisterCreatedObjectUndo(holeObject, "Generate socket holes");
            holeObject.transform.SetParent(socket.transform);
            holeObject.ResetTransform();

            var socketHole = holeObject.AddComponent<OGSocketHole>();
            socket.holes.Add(socketHole);
            socketHole.definition = hole;
            Selection.activeGameObject = socketHole.gameObject;
        }
    }

    [MenuItem("GameObject/Construction/Plug", false, 0)]
    static void CreatePlug(MenuCommand menuCommand)
    {
        var context = menuCommand.context;
        if (context is GameObject)
        {
            CreatePlug((GameObject)context);
        }
    }

    public static void CreatePlug(GameObject gameObject)
    {
        GameObject go = new GameObject("~O= PLUG", typeof(RectTransform));
        Undo.RegisterCreatedObjectUndo(go, "Create Plug");

        go.transform.SetParent(gameObject.transform);
        go.ResetTransform();

        Selection.activeObject = go;
    }

    [MenuItem("GameObject/Construction/Generate Plug Pins", false, 1)]
    static void GeneratePlugPins(MenuCommand menuCommand)
    {
        var context = menuCommand.context;
        if (context is GameObject)
        {
            GeneratePlugPins((GameObject)context);
        }
    }
    public static void GeneratePlugPins(GameObject gameObject)
    {
        var plug = gameObject.GetComponent<OGPlug>();

        if (plug == null)
        {
            throw new System.Exception("Object is missing a plug component");
        }

        if (plug.definition == null)
        {
            throw new System.Exception("Object is missing a plug definition");
        }

        if (plug.definition.forSocket == null)
        {
            throw new System.Exception("Plug definition is missing a socket definition");
        }

        if (plug.definition.forSocket.holes.Count == 0)
        {
            throw new System.Exception("Socket definition is missing hole definitions");
        }

        plug.gameObject.name = plug.definition.name;
        var socket = plug.definition.forSocket;

        if (plug.pins.IsNullOrEmpty())
        {
            plug.pins = new List<OGPlugPin>();
        }

        foreach (var hole in socket.holes)
        {
            GameObject detailObject = new GameObject("- PIN " + socket.GetTitle() + (hole.extraID.IsNullOrEmpty() ? "" : " " + hole.extraID), typeof(RectTransform));
            Undo.RegisterCreatedObjectUndo(detailObject, "Generate plug pins");
            detailObject.transform.SetParent(plug.transform);
            detailObject.ResetTransform();

            var plugPin = detailObject.AddComponent<OGPlugPin>();
            plugPin.forSocketHole = hole;
            plugPin.parentPlug = plug;
            plug.pins.Add(plugPin);
            Selection.activeGameObject = plugPin.gameObject;
        }
    }

    [MenuItem("GameObject/Construction/Feature", false, 0)]
    static void CreateFeature(MenuCommand menuCommand)
    {
        var context = menuCommand.context;
        if (context is GameObject)
        {
            CreateFeature((GameObject)context);
        }
    }
    
    public static void CreateFeature(GameObject gameObject)
    {
        GameObject go = new GameObject("# FEATURE", typeof(RectTransform));
        Undo.RegisterCreatedObjectUndo(go, "Create Feature");

        go.transform.SetParent(gameObject.transform);
        go.ResetTransform();

        Selection.activeObject = go;
    }

    [MenuItem("GameObject/Construction/Generate Feature Details", false, 1)]
    static void GenerateFeatureDetails(MenuCommand menuCommand)
    {
        var context = menuCommand.context;
        if (context is GameObject)
        {
            GenerateFeatureDetails((GameObject)context);
        }
    }
    
    public static void GenerateFeatureDetails(GameObject gameObject)
    {
        var feature = gameObject.GetComponent<OGFeature>();

        if (feature == null)
        {
            throw new System.Exception("Object is missing a feature component");
        }

        if (feature.definition == null)
        {
            throw new System.Exception("Object is missing a feature definition");
        }

        if (feature.definition.details.Count == 0)
        {
            throw new System.Exception("Feature definition is missing feature detail definitions");
        }
        
        var plug = gameObject.GetComponent<OGPlug>();
        if (plug == null)
        {
            feature.gameObject.name = "# FEATURE " + feature.definition.GetTitle();
        }

        if (feature.details.IsNullOrEmpty())
        {
            feature.details = new List<OGFeatureDetail>();
        }

        foreach (var detail in feature.definition.details)
        {
            GameObject detailObject = new GameObject("* DETAIL " + feature.definition.GetTitle() + (detail.extraID.IsNullOrEmpty() ? "" : " " + detail.extraID), typeof(RectTransform));
            Undo.RegisterCreatedObjectUndo(detailObject, "Generate feature details");
            detailObject.transform.SetParent(feature.transform);
            detailObject.ResetTransform();

            var featureDetail = detailObject.AddComponent<OGFeatureDetail>();
            feature.details.Add(featureDetail);
            featureDetail.definition = detail;
            featureDetail.SetFeatureDetail(detail.dummySprite);
            Selection.activeGameObject = featureDetail.gameObject;
        }
    }
#else
    public static void CreateConstruction(GameObject gameObject)
    {
    }

    public static void CreateSocket(GameObject gameObject)
    {
    }
    
    public static void GenerateSocketHoles(GameObject gameObject)
    {
    }
    
    public static void CreatePlug(GameObject gameObject)
    {
    }
    
    public static void GeneratePlugPins(GameObject gameObject)
    {
    }
    
    public static void CreateFeature(GameObject gameObject)
    {
    }
    
    public static void GenerateFeatureDetails(GameObject gameObject)
    {
    }
#endif
}
