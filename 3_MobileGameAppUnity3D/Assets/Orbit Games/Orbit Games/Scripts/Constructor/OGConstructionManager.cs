using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OGConstructionManager : OGSingletonBehaviour<OGConstructionManager> {

    public Dictionary<string, OGSocketDefinition> idToSocketDefinition = new Dictionary<string, OGSocketDefinition>();
    public Dictionary<string, OGFeatureDefinition> idToFeatureDefinition = new Dictionary<string, OGFeatureDefinition>();
    public Dictionary<string, OGPlugDefinition> idToPlugDefinition = new Dictionary<string, OGPlugDefinition>();
    public Dictionary<string, OGAdjustableDefinition> idToAdjustableDefinition = new Dictionary<string, OGAdjustableDefinition>();
    public Dictionary<string, OGBaseVariableDefinition> idToVariableDefinition = new Dictionary<string, OGBaseVariableDefinition>();

    public Dictionary<string, OGFeatureDesign> idToFeatureDesign = new Dictionary<string, OGFeatureDesign>();
    public Dictionary<string, OGPlug> idToPlugPrefab = new Dictionary<string, OGPlug>();
    public Dictionary<OGFeatureDefinition, List<OGFeatureDesign>> featureToDesigns = new Dictionary<OGFeatureDefinition, List<OGFeatureDesign>>();
    public Dictionary<OGSocketDefinition, List<OGPlug>> socketToPlugs = new Dictionary<OGSocketDefinition, List<OGPlug>>();
    public Dictionary<string, OGBaseDefinition> allDefinitionIDs = new Dictionary<string, OGBaseDefinition>();

    [ReadOnly]
    public string resourcesSubFolder = "Construction";
    public string constructionSocketID = "construction";
    public OGSocketDefinition mainConstructionSocket;
    [TextArea(3,6)]
    [SerializeField]
    private string defaultConstruction;
    private OGConstructionPlan defaultConstructionDeserialized;

    public OGConstructionPlan DefaultConstruction
    {
        get
        {
            if (defaultConstructionDeserialized == null)
            {
                defaultConstructionDeserialized = OGConstructionPlan.FromJson(defaultConstruction);
            }
            return defaultConstructionDeserialized;
        }
    }
    
    protected override void OnSingletonInitialize()
    {
        FindAndAddDefinitions(idToSocketDefinition);
        FindAndAddDefinitions(idToFeatureDefinition);
        FindAndAddDefinitions(idToPlugDefinition);
        FindAndAddDefinitions(idToAdjustableDefinition);
        FindAndAddDefinitions(idToVariableDefinition);

        FindAndAddDefinitions(idToFeatureDesign);
        FindAndAddDefined(idToPlugPrefab);

        foreach (var item in idToPlugPrefab)
        {
            var socketDefinition = item.Value.definition.forSocket;
            if (!socketToPlugs.ContainsKey(socketDefinition))
            {
                socketToPlugs.Add(socketDefinition, new List<OGPlug>());
            }
            socketToPlugs[socketDefinition].Add(item.Value);
        }

        foreach (var item in idToFeatureDesign)
        {
            var featureDefinition = item.Value.forFeature;
            if (!featureToDesigns.ContainsKey(featureDefinition))
            {
                featureToDesigns.Add(featureDefinition, new List<OGFeatureDesign>());
            }
            featureToDesigns[featureDefinition].Add(item.Value);
        }

        if (mainConstructionSocket == null)
        {
            mainConstructionSocket = idToSocketDefinition[constructionSocketID];
        }
    }

    private void FindAndAddDefinitions<T>(Dictionary<string, T> dictionary) where T : OGBaseDefinition
    {
        var objects = OGResources.FindObjectsOfTypeAll<T>(resourcesSubFolder);
        foreach (var def in objects)
        {
            if (!dictionary.ContainsKey(def))
            {
                var id = def.GetUniqueID();
                if (id == "template")
                {
                    continue;
                }
                if (!allDefinitionIDs.ContainsKey(id))
                {
                    dictionary.Add(id, def);
                    allDefinitionIDs.Add(id, def);
                }
                else
                {
                    string assetPath1 = "not supported outside editor";
                    string assetPath2 = "not supported outside editor";
#if UNITY_EDITOR
                    assetPath1 = UnityEditor.AssetDatabase.GetAssetPath(def);
                    assetPath2 = UnityEditor.AssetDatabase.GetAssetPath(allDefinitionIDs[id]);
#endif
                    throw new System.Exception("The following objects of type " +typeof(T).ToString()+ " have the same unique ID\n" + def + " (@path "+ assetPath1 + ")\n" + allDefinitionIDs[id] + " (@path " + assetPath2 + ")");
                }
            }
            else
            {
                throw new System.Exception("Duplicate entry for definition " + def);
            }
        }
    }
    private void FindAndAddDefined<T>(Dictionary<string, T> dictionary) where T : MonoBehaviour, IOGDefined
    {
        var objects = OGResources.FindObjectsOfTypeAll<T>(resourcesSubFolder);
        foreach (var obj in objects)
        {
            if (OGEditorExtensions.IsInScene(obj))
            {
                // objects in scene are not supported, we only work with objects found in the resources folder
                continue;
            }

            var def = obj.GetDefinition();
            var id = def.GetUniqueID();
            if (id == "template")
            {
                continue;
            }
            if (!dictionary.ContainsKey(id))
            {
                dictionary.Add(id, obj);
            }
            else
            {
                throw new System.Exception("Duplicate entry for defined object " + obj.gameObject.name + " with definition " + obj.GetDefinition());
            }
        }
    }

    public Dictionary<string, OGSocketDefinition>.ValueCollection GetSocketDefinitions()
    {
        return idToSocketDefinition.Values;
    }

    public OGSocketDefinition GetSocketDefinition(string id)
    {
        return idToSocketDefinition.GetOrDefault(id);
    }

    public Dictionary<string, OGFeatureDefinition>.ValueCollection GetFeatureDefinitions()
    {
        return idToFeatureDefinition.Values;
    }

    public OGFeatureDefinition GetFeatureDefinition(string id)
    {
        return idToFeatureDefinition.GetOrDefault(id);
    }

    public Dictionary<string, OGPlugDefinition>.ValueCollection GetPlugDefinitions()
    {
        return idToPlugDefinition.Values;
    }

    public OGPlugDefinition GetPlugDefinition(string id)
    {
        return idToPlugDefinition.GetOrDefault(id);
    }

    public Dictionary<string, OGAdjustableDefinition>.ValueCollection GetAdjustableDefinitions()
    {
        return idToAdjustableDefinition.Values;
    }

    public OGAdjustableDefinition GetAdjustableDefinition(string id)
    {
        return idToAdjustableDefinition.GetOrDefault(id);
    }

    public Dictionary<string, OGBaseVariableDefinition>.ValueCollection GetVariableDefinitions()
    {
        return idToVariableDefinition.Values;
    }

    public OGBaseVariableDefinition GetVariableDefinition(string id)
    {
        return idToVariableDefinition.GetOrDefault(id);
    }

    public Dictionary<string, OGFeatureDesign>.ValueCollection GetFeatureDesigns()
    {
        return idToFeatureDesign.Values;
    }

    public List<OGFeatureDesign> GetDesigns(OGFeatureDefinition feature)
    {
        return featureToDesigns.GetOrDefault(feature);
    }

    public OGFeatureDesign GetFeatureDesign(string id)
    {
        return idToFeatureDesign.GetOrDefault(id);
    }

    public Dictionary<string, OGPlug>.ValueCollection GetPlugs()
    {
        return idToPlugPrefab.Values;
    }

    public List<OGPlug> GetPlugs(OGSocketDefinition socket)
    {
        return socketToPlugs.GetOrDefault(socket);
    }

    public OGPlug GetPlug(string id)
    {
        return idToPlugPrefab.GetOrDefault(id);
    }
}
