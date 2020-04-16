using GameToolkit.Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Feature_Detail_", menuName = "Construction/Feature Detail Definition")]
public class OGFeatureDetailDefinition : ScriptableObject
{
    public string extraID = "";

    [Header("Dummy")]
    public Sprite dummySprite;
}
