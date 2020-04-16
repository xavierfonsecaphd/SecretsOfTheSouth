using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OGConstructor : OGBaseConstructionObject
{
    protected override void InitializeObject() { }
    protected override void ResetObject() { }
    
    public OGConstructionPlan ExtractConstructionPlanFromBuild()
    {
        var constructionComp = new OGConstructionPlan();

        HashSet<object> uniqueCheck = new HashSet<object>();
        var sockets = GetComponentsInChildren<OGSocket>(true);
        foreach (var socket in sockets)
        {
            if (uniqueCheck.Contains(socket.definition)) continue;
            var plug = socket.GetCurrentPlug();
            if (plug != null)
            {
                constructionComp.plugs.Add(plug.definition.GetUniqueID());
                uniqueCheck.Add(socket.definition);
            }
        }

        uniqueCheck.Clear();
        var features = GetComponentsInChildren<OGFeature>(true);
        foreach (var feature in features)
        {
            if (uniqueCheck.Contains(feature.definition)) continue;
            var design = feature.GetCurrentDesign();
            if (design != null)
            {
                constructionComp.features.Add(design.GetUniqueID());
                uniqueCheck.Add(feature.definition);
            }
        }

        uniqueCheck.Clear();
        var adjustables = GetComponentsInChildren<OGBaseAdjustableElement>(true);
        foreach (var adjustable in adjustables)
        {
            if (uniqueCheck.Contains(adjustable.definition)) continue;
            constructionComp.adjustables.Add(adjustable.definition.GetUniqueID());
            constructionComp.adjustableSettings.Add(adjustable.ToStoredValue());
            uniqueCheck.Add(adjustable.definition);
        }

        uniqueCheck.Clear();
        var variables = GetComponentsInChildren<OGBaseVariable>(true);
        foreach (var variable in variables)
        {
            if (uniqueCheck.Contains(variable.definition)) continue;
            constructionComp.variables.Add(variable.definition.GetUniqueID());
            constructionComp.variableValues.Add(variable.ToStoredValue());
            uniqueCheck.Add(variable.definition);
        }

        return constructionComp;
    }
    [TextArea(3, 10)]
    public string fromJson;
    [Buttons("FromJSON", "FromJSON", "ToJSON", "ToJSON", "Random", "DebugGenerateButton")]
    public ButtonsContainer buttons;
    [TextArea(3, 10)]
    public string toJson;
    [TextArea(3, 10)]
    public string randomJson;
    [TextArea(3, 10)]
    public string compressedJson;

    public void ToJSON()
    {
        toJson = ExtractConstructionPlanFromBuild().ToJson();
    }

    public void FromJSON()
    {
        BuildConstructionPlan(OGConstructionPlan.FromJson(fromJson));
    }

    public void DebugGenerateButton()
    {
        GenerateRandomConstructionPlan(GenerateRandomConstructionVariableValues());
    }

    public OGConstructionPlan GenerateRandomConstructionPlan()
    {
        return GenerateRandomConstructionPlan(GenerateRandomConstructionVariableValues());
    }

    public OGConstructionPlan GenerateRandomConstructionPlan(OGConstructionVariables variablesInput)
    {
        if (variablesInput == null)
        {
            variablesInput = new OGConstructionVariables();
        }

        var constructionComp = new OGConstructionPlan();
        var sockets = OGConstructionManager.I.GetSocketDefinitions();
        HashSet<OGAdjustableDefinition> adjustables = new HashSet<OGAdjustableDefinition>();
        HashSet<OGBaseVariableDefinition> variables = new HashSet<OGBaseVariableDefinition>();

        foreach (var socket in sockets)
        {
            var plugs = OGConstructionManager.I.GetPlugs(socket);
            if (plugs.IsNullOrEmpty()) continue;
            var plug = plugs[Random.Range(0, plugs.Count)];
            adjustables.AddRangeUnique(plug.GetAdjustableDefinitions());
            variables.AddRangeUnique(plug.GetVariableDefinitions());
            constructionComp.plugs.Add(plug.definition.GetUniqueID());
        }

        var features = OGConstructionManager.I.GetFeatureDefinitions();
        foreach (var feat in features)
        {
            var designs = OGConstructionManager.I.GetDesigns(feat);
            if (designs.IsNullOrEmpty()) continue;
            var design = designs[Random.Range(0, designs.Count)];
            constructionComp.features.Add(design.GetUniqueID());
        }

        foreach (var adjustable in adjustables)
        {
            var random = (Random.Range(0f, 1f) + Random.Range(0f, 1f) + Random.Range(0f, 1f) + Random.Range(0f, 1f)) / 4f;
            constructionComp.adjustables.Add(adjustable.GetUniqueID());
            constructionComp.adjustableSettings.Add(random.toRoundTripString());
        }

        foreach (var variable in variables)
        {
            if (variablesInput.HasValue(variable))
            {
                constructionComp.variables.Add(variable.GetUniqueID());
                constructionComp.variableValues.Add(variablesInput.GetSerializedValue(variable));
            }
            else
            {
                Debug.Log("Construction plan was missing values for variable " + variable);
            }
        }

        BuildConstructionPlan(constructionComp);
        var extractedPlan = ExtractConstructionPlanFromBuild();
        randomJson = extractedPlan.ToJson();
        compressedJson = randomJson.Compress();
        return extractedPlan;
    }

    private OGConstructionVariables GenerateRandomConstructionVariableValues()
    {
        var variableValues = new OGConstructionVariables();
        var variables = OGConstructionManager.I.GetVariableDefinitions();

        foreach (var variable in variables)
        {
            if (variable is OGStringVariableDefinition)
            {
                variableValues.SetStringValue(variable, "Random " + (Random.value * 100f).toNumberString(2));
            }
            else if (variable is OGColorVariableDefinition)
            {
                variableValues.SetColorValue(variable, HSLColor.RandomHue(1f, 0.5f).ToColor());
            }
        }
        return variableValues;
    }
}
