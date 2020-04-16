using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OGConstructionVariables : IEnumerable<OGBaseVariableDefinition>
{
    private Dictionary<OGColorVariableDefinition, Color> colorValues = new Dictionary<OGColorVariableDefinition, Color>();
    private Dictionary<OGStringVariableDefinition, string> stringValues = new Dictionary<OGStringVariableDefinition, string>();
    private Dictionary<OGBooleanVariableDefinition, bool> boolValues = new Dictionary<OGBooleanVariableDefinition, bool>();

    public IEnumerator<OGBaseVariableDefinition> GetEnumerator()
    {
        foreach (var kvp in colorValues)
        {
            yield return kvp.Key;
        }
        foreach (var kvp in stringValues)
        {
            yield return kvp.Key;
        }
        foreach (var kvp in boolValues)
        {
            yield return kvp.Key;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public bool HasValue(OGBaseVariableDefinition variable)
    {
        if (variable is OGStringVariableDefinition)
        {
            return stringValues.ContainsKey((OGStringVariableDefinition)variable);
        }
        else if (variable is OGColorVariableDefinition)
        {
            return colorValues.ContainsKey((OGColorVariableDefinition)variable);
        }
        else if (variable is OGBooleanVariableDefinition)
        {
            return boolValues.ContainsKey((OGBooleanVariableDefinition)variable);
        }
        return false;
    }

    public string GetSerializedValue(OGBaseVariableDefinition variable)
    {
        if (variable is OGStringVariableDefinition)
        {
            return OGStringVariable.ToStoredValue(stringValues[(OGStringVariableDefinition)variable]);
        }
        else if (variable is OGColorVariableDefinition)
        {
            return OGColorVariable.ToStoredValue(colorValues[(OGColorVariableDefinition)variable]);
        }
        else if (variable is OGBooleanVariableDefinition)
        {
            return OGBooleanVariable.ToStoredValue(boolValues[(OGBooleanVariableDefinition)variable]);
        }
        return null;
    }

    public OGColorVariableDefinition[] GetColorVariables()
    {
        OGColorVariableDefinition[] keys = new OGColorVariableDefinition[colorValues.Keys.Count];
        colorValues.Keys.CopyTo(keys, 0);
        return keys;
    }

    public void SetColorValue(string variableID, Color value)
    {
        var definition = OGConstructionManager.I.GetVariableDefinition(variableID);
        if (definition == null)
        {
            Debug.LogError("Could not find variable definition for id: " + variableID);
            return;
        }
        if (!(definition is OGColorVariableDefinition))
        {
            Debug.LogError("Definition for id: " + variableID + " was not of type Color variable");
            return;
        }

        SetColorValue((OGColorVariableDefinition)definition, value);
    }

    public void SetColorValue(OGBaseVariableDefinition variable, Color value)
    {
        SetColorValue((OGColorVariableDefinition)variable, value);
    }

    public void SetColorValue(OGColorVariableDefinition variable, Color value)
    {
        if (variable == null)
        {
            Debug.LogError("Received empty variable definition for setting color value");
            return;
        }

        colorValues.SetOrAdd(variable, value);
    }
    
    public Color GetColorValue(string variableID)
    {
        var definition = OGConstructionManager.I.GetVariableDefinition(variableID);
        if (definition == null)
        {
            Debug.LogError("Could not find variable definition for id: " + variableID);
            return Color.magenta;
        }
        if (!(definition is OGColorVariableDefinition))
        {
            Debug.LogError("Definition for id: " + variableID + " was not of type Color variable");
            return Color.magenta;
        }

        return GetColorValue((OGColorVariableDefinition)definition);
    }

    public Color GetColorValue(OGBaseVariableDefinition variable)
    {
        return GetColorValue((OGColorVariableDefinition)variable);
    }

    public Color GetColorValue(OGColorVariableDefinition variable)
    {
        if (variable == null)
        {
            Debug.LogError("Received empty variable definition for setting color value");
            return Color.magenta;
        }

        return colorValues.GetOrDefault(variable);
    }
    
    public bool HasColorValue(string variableID)
    {
        var definition = OGConstructionManager.I.GetVariableDefinition(variableID);
        if (definition == null)
        {
            Debug.LogError("Could not find variable definition for id: " + variableID);
            return false;
        }
        if (!(definition is OGColorVariableDefinition))
        {
            Debug.LogError("Definition for id: " + variableID + " was not of type Color variable");
            return false;
        }

        return HasColorValue((OGColorVariableDefinition)definition);
    }

    public bool HasColorValue(OGBaseVariableDefinition variable)
    {
        return HasColorValue((OGColorVariableDefinition)variable);
    }

    public bool HasColorValue(OGColorVariableDefinition variable)
    {
        if (variable == null)
        {
            Debug.LogError("Received empty variable definition for setting color value");
            return false;
        }

        return colorValues.ContainsKey(variable);
    }




    public OGStringVariableDefinition[] GetStringVariables()
    {
        OGStringVariableDefinition[] keys = new OGStringVariableDefinition[colorValues.Keys.Count];
        stringValues.Keys.CopyTo(keys, 0);
        return keys;
    }

    public void SetStringValue(string variableID, string value)
    {
        var definition = OGConstructionManager.I.GetVariableDefinition(variableID);
        if (definition == null)
        {
            Debug.LogError("Could not find variable definition for id: " + variableID);
            return;
        }
        if (!(definition is OGStringVariableDefinition))
        {
            Debug.LogError("Definition for id: " + variableID + " was not of type String variable");
            return;
        }

        SetStringValue((OGStringVariableDefinition)definition, value);
    }

    public void SetStringValue(OGBaseVariableDefinition variable, string value)
    {
        SetStringValue((OGStringVariableDefinition)variable, value);
    }

    public void SetStringValue(OGStringVariableDefinition variable, string value)
    {
        if (variable == null)
        {
            Debug.LogError("Received empty variable definition for setting color value");
            return;
        }

        stringValues.SetOrAdd(variable, value);
    }

    public string GetStringValue(string variableID)
    {
        var definition = OGConstructionManager.I.GetVariableDefinition(variableID);
        if (definition == null)
        {
            Debug.LogError("Could not find variable definition for id: " + variableID);
            return null;
        }
        if (!(definition is OGStringVariableDefinition))
        {
            Debug.LogError("Definition for id: " + variableID + " was not of type String variable");
            return null;
        }
        return GetStringValue((OGStringVariableDefinition)definition);
    }

    public string GetStringValue(OGBaseVariableDefinition variable, string value)
    {
        return GetStringValue((OGStringVariableDefinition)variable, value);
    }

    public string GetStringValue(OGStringVariableDefinition variable, string value)
    {
        if (variable == null)
        {
            Debug.LogError("Received empty variable definition for setting color value");
            return null;
        }

        return stringValues.GetOrDefault(variable);
    }

    public bool HasStringValue(string variableID)
    {
        var definition = OGConstructionManager.I.GetVariableDefinition(variableID);
        if (definition == null)
        {
            Debug.LogError("Could not find variable definition for id: " + variableID);
            return false;
        }
        if (!(definition is OGStringVariableDefinition))
        {
            Debug.LogError("Definition for id: " + variableID + " was not of type String variable");
            return false;
        }
        return HasStringValue((OGStringVariableDefinition)definition);
    }

    public bool HasStringValue(OGBaseVariableDefinition variable)
    {
        return HasStringValue((OGStringVariableDefinition)variable);
    }

    public bool HasStringValue(OGStringVariableDefinition variable)
    {
        if (variable == null)
        {
            Debug.LogError("Received empty variable definition for setting color value");
            return false;
        }

        return stringValues.ContainsKey(variable);
    }





    public OGBooleanVariableDefinition[] GetBooleanVariables()
    {
        OGBooleanVariableDefinition[] keys = new OGBooleanVariableDefinition[colorValues.Keys.Count];
        boolValues.Keys.CopyTo(keys, 0);
        return keys;
    }

    public void SetBooleanValue(string variableID, bool value)
    {
        var definition = OGConstructionManager.I.GetVariableDefinition(variableID);
        if (definition == null)
        {
            Debug.LogError("Could not find variable definition for id: " + variableID);
            return;
        }
        if (!(definition is OGBooleanVariableDefinition))
        {
            Debug.LogError("Definition for id: " + variableID + " was not of type Boolean variable");
            return;
        }

        SetBooleanValue((OGBooleanVariableDefinition)definition, value);
    }

    public void SetBooleanValue(OGBaseVariableDefinition variable, bool value)
    {
        SetBooleanValue((OGBooleanVariableDefinition)variable, value);
    }

    public void SetBooleanValue(OGBooleanVariableDefinition variable, bool value)
    {
        if (variable == null)
        {
            Debug.LogError("Received empty variable definition for setting color value");
            return;
        }

        boolValues.SetOrAdd(variable, value);
    }

    public bool GetBooleanValue(string variableID)
    {
        var definition = OGConstructionManager.I.GetVariableDefinition(variableID);
        if (definition == null)
        {
            Debug.LogError("Could not find variable definition for id: " + variableID);
            return false;
        }
        if (!(definition is OGBooleanVariableDefinition))
        {
            Debug.LogError("Definition for id: " + variableID + " was not of type Boolean variable");
            return false;
        }
        return GetBooleanValue((OGBooleanVariableDefinition)definition);
    }

    public bool GetBooleanValue(OGBaseVariableDefinition variable, bool value)
    {
        return GetBooleanValue((OGBooleanVariableDefinition)variable, value);
    }

    public bool GetBooleanValue(OGBooleanVariableDefinition variable, bool value)
    {
        if (variable == null)
        {
            Debug.LogError("Received empty variable definition for setting color value");
            return false;
        }

        return boolValues.GetOrDefault(variable);
    }

    public bool HasBooleanValue(string variableID)
    {
        var definition = OGConstructionManager.I.GetVariableDefinition(variableID);
        if (definition == null)
        {
            Debug.LogError("Could not find variable definition for id: " + variableID);
            return false;
        }
        if (!(definition is OGBooleanVariableDefinition))
        {
            Debug.LogError("Definition for id: " + variableID + " was not of type Boolean variable");
            return false;
        }
        return HasBooleanValue((OGBooleanVariableDefinition)definition);
    }

    public bool HasBooleanValue(OGBaseVariableDefinition variable)
    {
        return HasBooleanValue((OGBooleanVariableDefinition)variable);
    }

    public bool HasBooleanValue(OGBooleanVariableDefinition variable)
    {
        if (variable == null)
        {
            Debug.LogError("Received empty variable definition for setting color value");
            return false;
        }

        return boolValues.ContainsKey(variable);
    }
}
