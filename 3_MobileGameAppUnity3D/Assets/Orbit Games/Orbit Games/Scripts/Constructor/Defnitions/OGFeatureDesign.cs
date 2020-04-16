using GameToolkit.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Feature_Design_", menuName = "Construction/Feature Design")]
public class OGFeatureDesign : OGBaseDefinition
{
    [Header("Settings")]
    public OGFeatureDefinition forFeature;
    public bool hidden = false;

    [Header("Design")]
    public List<FeatureDetail> details;

    private Dictionary<OGFeatureDetailDefinition, Sprite> definitionToSprite;
    public Sprite GetSprite(OGFeatureDetailDefinition forDetail)
    {
        if (definitionToSprite == null)
        {
            definitionToSprite = new Dictionary<OGFeatureDetailDefinition, Sprite>();
            foreach (var detail in details)
            {
                definitionToSprite.Add(detail.definition, detail.sprite);
            }
        }
        if (definitionToSprite.ContainsKey(forDetail))
        {
            return definitionToSprite[forDetail];
        }
        else
        {
            return null;
        }
    }

    protected override string GetFilePrefix()
    {
        return "Feature_Design";
    }

    [Serializable]
    public class FeatureDetail
    {
        public OGFeatureDetailDefinition definition;
        public Sprite sprite;
    }
}