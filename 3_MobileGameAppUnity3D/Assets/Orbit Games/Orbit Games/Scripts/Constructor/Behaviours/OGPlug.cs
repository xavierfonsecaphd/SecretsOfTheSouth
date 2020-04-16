using GameToolkit.Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OGPlug : OGBaseConstructionObject, IOGDefined {

    [Header("Definition")]
    public OGPlugDefinition definition;
    public List<OGPlugPin> pins;
    [Buttons("Generate Pins", "Generate", "Just Rename", "Rename", "Find Pins", "FindPins")]
    public ButtonsContainer generate;

    public void Generate()
    {
        OGConstructorMenuCommands.GeneratePlugPins(gameObject);
    }

    public void FindPins()
    {
        pins = new List<OGPlugPin>();
        var foundPins = GetComponentsInChildren<OGPlugPin>(true);
        foreach (var pin in foundPins)
        {
            pin.parentPlug = this;
            if (definition.forSocket.holes.Contains(pin.forSocketHole))
            {
                pins.AddUnique(pin);
            }
        }

        OGEditorExtensions.SaveAsset(this);
        if (pins.Count == 0)
        {
            throw new System.Exception("Can't find any pins to plug into socket " + definition.forSocket);
        }
    }

    public void Rename()
    {
        gameObject.name = definition.name;
        OGEditorExtensions.SaveAsset(this);
    }

    public OGBaseDefinition GetDefinition()
    {
        return definition;
    }

    private Dictionary<OGSocketHoleDefinition, OGPlugPin> holeDefinitionToPin = null;
    protected override void InitializeObject()
    {
        holeDefinitionToPin = new Dictionary<OGSocketHoleDefinition, OGPlugPin>();
        foreach (var pin in pins)
        {
            if (definition.forSocket.holes.Contains(pin.forSocketHole))
            {
                pin.parentPlug = this;
                holeDefinitionToPin.Add(pin.forSocketHole, pin);
            }
        }
        if (pins.Count == 0)
        {
            throw new System.Exception("Can't find any pins to plug into socket " + definition.forSocket);
        }
    }

    protected override void ResetObject()
    {
        foreach (var pin in pins)
        {
            pin.Reset();
        }
    }

    public void PlugPins(OGSocket socket)
    {
        Initialize();

        if (socket == null)
        {
            ResetObject();
        }
        else
        {
            if (socket.definition != definition.forSocket)
            {
                throw new System.Exception("Can't plug into socket " + socket.definition + " when design for " + definition.forSocket);
            }

            foreach (var kvp in holeDefinitionToPin)
            {
                kvp.Value.ConnectToSocketHole(socket.GetSocketHole(kvp.Key));
            }
        }
    }
}
