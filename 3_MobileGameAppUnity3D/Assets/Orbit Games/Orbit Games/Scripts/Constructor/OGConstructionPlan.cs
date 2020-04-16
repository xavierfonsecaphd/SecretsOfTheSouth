using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class OGConstructionPlan
{
    public List<string> plugs = new List<string>();
    public List<string> features = new List<string>();

    public List<string> adjustables = new List<string>();
    public List<string> adjustableSettings = new List<string>();

    public List<string> variables = new List<string>();
    public List<string> variableValues = new List<string>();

    [NonSerialized]
    private Dictionary<OGSocketDefinition, OGPlug> socketToPlug = new Dictionary<OGSocketDefinition, OGPlug>();
    [NonSerialized]
    private Dictionary<OGFeatureDefinition, OGFeatureDesign> featureToDesign = new Dictionary<OGFeatureDefinition, OGFeatureDesign>();
    [NonSerialized]
    private Dictionary<OGAdjustableDefinition, string> adjustableToSetting = new Dictionary<OGAdjustableDefinition, string>();
    [NonSerialized]
    private Dictionary<OGBaseVariableDefinition, string> variableToValue = new Dictionary<OGBaseVariableDefinition, string>();

    private bool initialized = false;
    private void Initialize()
    {
        if (initialized) return;
        initialized = true;

        foreach (var plugID in plugs)
        {
            var plug = OGConstructionManager.I.GetPlug(plugID);
            if (plug != null)
            {
                socketToPlug.SetOrAdd(plug.definition.forSocket, plug);
            }
        }

        foreach (var featureID in features)
        {
            var feature = OGConstructionManager.I.GetFeatureDesign(featureID);
            if (feature != null)
            {
                featureToDesign.SetOrAdd(feature.forFeature, feature);
            }
        }

        for (int i = 0; i < adjustables.Count; i++)
        {
            var adjustable = OGConstructionManager.I.GetAdjustableDefinition(adjustables[i]);
            if (adjustable != null)
            {
                adjustableToSetting.SetOrAdd(adjustable, adjustableSettings[i]);
            }
        }

        for (int i = 0; i < variables.Count; i++)
        {
            var variable = OGConstructionManager.I.GetVariableDefinition(variables[i]);
            if (variable != null)
            {
                variableToValue.SetOrAdd(variable, variableValues[i]);
            }
        }
    }

    public OGConstructionPlan Duplicate()
    {
        var copy = new OGConstructionPlan();
        copy.plugs.AddRange(plugs);
        copy.features.AddRange(features);
        copy.adjustables.AddRange(adjustables);
        copy.adjustableSettings.AddRange(adjustableSettings);
        copy.variables.AddRange(variables);
        copy.variableValues.AddRange(variableValues);
        return copy;
    }

    public void SetVariableValues(OGConstructionVariables newValues)
    {
        Initialize();
        foreach (var variable in newValues)
        {
            if (variable is OGStringVariableDefinition)
            {
                variables.Add(variable.GetUniqueID());
                variableValues.Add(newValues.GetStringValue(variable));
                variableToValue.SetOrAdd(variable, newValues.GetStringValue(variable));
            }
            else if (variable is OGColorVariableDefinition)
            {
                variables.Add(variable.GetUniqueID());
                variableValues.Add(OGColorVariable.ToStoredValue(newValues.GetColorValue(variable)));
                variableToValue.SetOrAdd(variable, OGColorVariable.ToStoredValue(newValues.GetColorValue(variable)));
            }
            else if (variable is OGBooleanVariableDefinition)
            {
                variables.Add(variable.GetUniqueID());
                variableValues.Add(OGBooleanVariable.ToStoredValue(newValues.GetBooleanValue(variable)));
                variableToValue.SetOrAdd(variable, OGBooleanVariable.ToStoredValue(newValues.GetBooleanValue(variable)));
            }
        }
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public static OGConstructionPlan FromJson(string ConstructionJson)
    {
        return JsonUtility.FromJson<OGConstructionPlan>(ConstructionJson);
    }

    public OGPlug GetPlug(OGSocketDefinition socket)
    {
        Initialize();
        return socketToPlug.GetOrDefault(socket);
    }

    public OGFeatureDesign GetFeatureDesign(OGFeatureDefinition feature)
    {
        Initialize();
        return featureToDesign.GetOrDefault(feature);
    }

    public string GetAdjustableSetting(OGAdjustableDefinition adjustable)
    {
        Initialize();
        return adjustableToSetting.GetOrDefault(adjustable);
    }

    public string GetVariableValue(OGBaseVariableDefinition variable)
    {
        Initialize();
        return variableToValue.GetOrDefault(variable);
    }
}
