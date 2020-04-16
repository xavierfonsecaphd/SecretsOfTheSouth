using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OGSocket : MonoBehaviour {

    [Header("Definition")]
    public OGSocketDefinition definition;
    public List<OGSocketHole> holes;
    [Buttons("Generate Holes", "Generate", "Move to Plug", "MoveToPlug", "Find Holes", "FindHoles")]
    public ButtonsContainer generate;
    public void Generate()
    {
        OGConstructorMenuCommands.GenerateSocketHoles(gameObject);
    }
    public void MoveToPlug()
    {
        var parentPlug = GetComponentInParent<OGPlug>();
        if (parentPlug != null)
        {
            if (parentPlug.gameObject == this.gameObject)
            {
                throw new System.Exception("Socket is already on a plug");
            }
            var socket = parentPlug.gameObject.AddComponent<OGSocket>();
            socket.definition = definition;
        }
        else
        {
            throw new System.Exception("No parent plug found");
        }

        var index = this.transform.GetSiblingIndex();
        while (transform.childCount > 0)
        {
            var child = transform.GetChild(transform.childCount - 1);
            child.SetParent(transform.parent);
            child.SetSiblingIndex(index);
        }

        var components = GetComponents<Component>();

        if (components.Length > 2)
        {
            gameObject.name = "GameObject";
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }
    public void FindHoles()
    {
        this.holes = new List<OGSocketHole>();
        definitionToObject = new Dictionary<OGSocketHoleDefinition, OGSocketHole>();
        var foundHoles = GetComponentsInChildren<OGSocketHole>(true);
        foreach (var hole in foundHoles)
        {
            if (definition.holes.Contains(hole.definition))
            {
                this.holes.AddUnique(hole);
            }
        }
        OGEditorExtensions.SaveAsset(this);
        if (holes.Count != definition.holes.Count)
        {
            throw new System.Exception("It seems like not all defined holes have been accounted for socket " + definition);
        }
    }

    private Dictionary<OGSocketHoleDefinition, OGSocketHole> definitionToObject;

    private void Initialize()
    {
        if (definitionToObject == null)
        {
            if (holes.Count != definition.holes.Count)
            {
                throw new System.Exception("It seems like not all defined holes have been accounted for socket " + definition);
            }

            definitionToObject = new Dictionary<OGSocketHoleDefinition, OGSocketHole>();
            foreach (var hole in holes)
            {
                if (definition.holes.Contains(hole.definition))
                {
                    definitionToObject.Add(hole.definition, hole);
                }
            }
        }
    }

    public OGSocketHole GetSocketHole(OGSocketHoleDefinition fromDefinition)
    {
        Initialize();
        if (definitionToObject.ContainsKey(fromDefinition))
        {
            return definitionToObject[fromDefinition];
        }
        else
        {
            return null;
        }
    }

    public void Unplug()
    {
        if (currentPlug != null)
        {
            OGPool.removeCopy(currentPlug);
            currentPlug = null;
        }
    }

    OGPlug currentPlug;

    public void ApplyVariableValuesToPlug(OGConstructionVariables variableValues)
    {
        if (currentPlug != null)
        {
            currentPlug.ApplyVariableValues(variableValues);
        }
    }

    public OGPlug Plugin(OGPlug plug, OGConstructionPlan composition = null)
    {
        Initialize();

        Unplug();

        if (plug != null)
        {
            if (plug.definition.forSocket != definition)
            {
                throw new System.Exception("Can't plug for socket " + plug.definition.forSocket + " on this socket " + definition);
            }

            currentPlug = OGPool.placeCopy(plug, transform);
            currentPlug.PlugPins(this);
            currentPlug.BuildConstructionPlan(composition);
        }
        return currentPlug;
    }

    public OGPlug GetCurrentPlug()
    {
        return currentPlug;
    }
}
