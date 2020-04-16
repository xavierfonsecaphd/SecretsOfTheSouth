using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OGBaseConstructionObject : MonoBehaviour, IOGPoolEventsListener {
    
    private Dictionary<OGSocketDefinition, List<OGSocket>> sockets;
    private Dictionary<OGFeatureDefinition, List<OGFeature>> features;
    private Dictionary<OGAdjustableDefinition, List<OGBaseAdjustableElement>> adjustables;
    private Dictionary<OGBaseVariableDefinition, List<OGBaseVariable>> variables;
    [NonSerialized]
    private bool initialized = false;

    [Buttons("Log")]
    public ButtonsContainer triggerbutton;

    protected abstract void ResetObject();
    protected abstract void InitializeObject();

    private void ResetContents()
    {
        foreach (var kvp in sockets)
        {
            foreach (var component in kvp.Value)
            {
                component.Unplug();
            }
        }

        foreach (var kvp in features)
        {
            foreach (var component in kvp.Value)
            {
                component.EmptyDesign();
            }
        }

        foreach (var kvp in adjustables)
        {
            foreach (var component in kvp.Value)
            {
                component.ResetToDefault();
            }
        }

        foreach (var kvp in variables)
        {
            foreach (var component in kvp.Value)
            {
                component.ResetToDefault();
            }
        }
    }

    //public void Log()
    //{
    //    var foundSockets = GetComponentsInChildren<OGSocket>(true);
    //    var foudnFeatures = GetComponentsInChildren<OGFeature>(true);
    //    var foundAdjustables = GetComponentsInChildren<OGBaseAdjustableElement>(true);
    //    var logString = "---------- " + this + " ------------- ";
    //    logString += "\n\nFound Sockets: " + foundSockets.ToDebugString();
    //    logString += "\n\nSockets: " + sockets.ToDebugString();
    //    logString += "\n\nFound Features: " + foudnFeatures.ToDebugString();
    //    logString += "\n\nFeatures: " + features.ToDebugString();
    //    logString += "\n\nFound Adjustables: " + foundAdjustables.ToDebugString();
    //    logString += "\n\nAdjustables: " + adjustables.ToDebugString();
    //    logString += "\n\n";
    //    Debug.Log(logString);
    //}

    protected void Initialize()
    {
        if (!initialized)
        {

            initialized = true;

            sockets = new Dictionary<OGSocketDefinition, List<OGSocket>>();
            var foundSockets = GetComponentsInChildren<OGSocket>(true);
            foreach (var socket in foundSockets)
            {
                sockets.CreateOrAddToList(socket.definition, socket);
            }

            features = new Dictionary<OGFeatureDefinition, List<OGFeature>>();
            var foudnFeatures = GetComponentsInChildren<OGFeature>(true);
            foreach (var feature in foudnFeatures)
            {
                features.CreateOrAddToList(feature.definition, feature);
            }

            adjustables = new Dictionary<OGAdjustableDefinition, List<OGBaseAdjustableElement>>();
            var foundAdjustables = GetComponentsInChildren<OGBaseAdjustableElement>(true);
            foreach (var adjustable in foundAdjustables)
            {
                adjustables.CreateOrAddToList(adjustable.definition, adjustable);
            }

            variables = new Dictionary<OGBaseVariableDefinition, List<OGBaseVariable>>();
            var foundVariables = GetComponentsInChildren<OGBaseVariable>(true);
            foreach (var variable in foundVariables)
            {
                variables.CreateOrAddToList(variable.definition, variable);
            }

            //Log();

            InitializeObject();
        }
    }

    public ICollection<OGAdjustableDefinition> GetAdjustableDefinitions()
    {
        Initialize();
        return adjustables.Keys;
    }

    public ICollection<OGBaseVariableDefinition> GetVariableDefinitions()
    {
        Initialize();
        return variables.Keys;
    }

    public ICollection<OGFeatureDefinition> GetFeatureDefinitions()
    {
        Initialize();
        return features.Keys;
    }

    public ICollection<OGSocketDefinition> GetSocketDefinitions()
    {
        Initialize();
        return sockets.Keys;
    }

    public void BuildConstructionPlan(OGConstructionPlan plan)
    {
        Initialize();

        if (plan == null || plan.plugs.Count == 0)
        {
            plan = OGConstructionManager.I.DefaultConstruction;
        }
        if (plan != null && plan.plugs.Count > 0)
        {
            foreach (var kvp in features)
            {
                var design = plan.GetFeatureDesign(kvp.Key);
                foreach (var component in kvp.Value)
                {
                    component.SetFeatureDesign(design);
                }
            }

            foreach (var kvp in adjustables)
            {
                var setting = plan.GetAdjustableSetting(kvp.Key);
                foreach (var component in kvp.Value)
                {
                    component.FromStoredValue(setting);
                }
            }

            foreach (var kvp in variables)
            {
                var value = plan.GetVariableValue(kvp.Key);
                foreach (var component in kvp.Value)
                {
                    component.FromStoredValue(value);
                }
            }

            foreach (var kvp in sockets)
            {
                var plug = plan.GetPlug(kvp.Key);
                foreach (var component in kvp.Value)
                {
                    component.Plugin(plug, plan);
                }
            }
        }
    }

    public void ApplyVariableValues(OGConstructionVariables variableValues)
    {
        foreach (var kvp in variables)
        {
            if (variableValues.HasValue(kvp.Key))
            {
                var value = variableValues.GetSerializedValue(kvp.Key);
                foreach (var component in kvp.Value)
                {
                    component.FromStoredValue(value);
                }
            }
        }

        foreach (var kvp in sockets)
        {
            foreach (var component in kvp.Value)
            {
                component.ApplyVariableValuesToPlug(variableValues);
            }
        }
    }
    
    public void OnRemovedToPool() { ResetContents(); ResetObject(); }
    public void OnPlacedFromPool() { ResetContents(); ResetObject(); }
    public void OnCreatedForPool() { Initialize(); }
}
