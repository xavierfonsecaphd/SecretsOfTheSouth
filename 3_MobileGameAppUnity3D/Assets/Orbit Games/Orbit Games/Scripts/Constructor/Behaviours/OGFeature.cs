using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OGFeature : MonoBehaviour, IOGPoolEventsListener
{
    [Header("Definition")]
    public OGFeatureDefinition definition;
    public List<OGFeatureDetail> details;
    [Buttons("Generate Details", "Generate", "Move to Plug", "MoveToPlug", "Find Details", "FindDetails")]
    public ButtonsContainer generate;
    public void Generate()
    {
        OGConstructorMenuCommands.GenerateFeatureDetails(gameObject);
    }
    public void MoveToPlug()
    {
        var parentPlug = GetComponentInParent<OGPlug>();
        if (parentPlug != null)
        {
            if (parentPlug.gameObject == this.gameObject)
            {
                throw new System.Exception("Feature is already on a plug");
            }
            var feature = parentPlug.gameObject.AddComponent<OGFeature>();
            feature.definition = definition;
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
    public void FindDetails()
    {
        this.details = new List<OGFeatureDetail>();
        definitionToObject = new Dictionary<OGFeatureDetailDefinition, OGFeatureDetail>();
        var foundDetails = GetComponentsInChildren<OGFeatureDetail>(true);
        foreach (var detail in foundDetails)
        {
            if (definition.details.Contains(detail.definition))
            {
                this.details.AddUnique(detail);
            }
        }
        OGEditorExtensions.SaveAsset(this);
        if (details.Count != definition.details.Count)
        {
            throw new System.Exception("It seems like not all defined details have been accounted for this feature");
        }
    }

    private Dictionary<OGFeatureDetailDefinition, OGFeatureDetail> definitionToObject;

    private void Initialize()
    {
        if (definitionToObject == null)
        {
            if (details.Count != definition.details.Count)
            {
                throw new System.Exception("It seems like not all defined details have been accounted for this feature");
            }
            definitionToObject = new Dictionary<OGFeatureDetailDefinition, OGFeatureDetail>();
            foreach (var detail in details)
            {
                if (definition.details.Contains(detail.definition))
                {
                    definitionToObject.Add(detail.definition, detail);
                }
            }

            EmptyDesign();
        }
    }

    public Dictionary<OGFeatureDetailDefinition, OGFeatureDetail> GeDefinitionToObjectMapping()
    {
        Initialize();
        return definitionToObject;
    }

    public void EmptyDesign()
    {
        currentDesign = null;
        foreach (var kvp in definitionToObject)
        {
            kvp.Value.SetFeatureDetail(null);
        }
    }

    private OGFeatureDesign currentDesign;

    public void SetFeatureDesign(OGFeatureDesign design)
    {
        Initialize();

        if (design == currentDesign) return;
        if (design == null)
        {
            EmptyDesign();
        }
        else
        {
            if (design.forFeature != definition)
            {
                throw new System.Exception("Can't set feature design " + design.forFeature + " for one that is setup for " + definition);
            }

            foreach (var kvp in definitionToObject)
            {
                var sprite = design.GetSprite(kvp.Key);
                kvp.Value.SetFeatureDetail(sprite);
            }
        }

        currentDesign = design;
    }

    public OGFeatureDesign GetCurrentDesign()
    {
        return currentDesign;
    }

    private void Reset()
    {
        SetFeatureDesign(null);
    }

    public void OnRemovedToPool() { Reset(); }
    public void OnPlacedFromPool() { Reset(); }
    public void OnCreatedForPool() { Reset(); }
}
