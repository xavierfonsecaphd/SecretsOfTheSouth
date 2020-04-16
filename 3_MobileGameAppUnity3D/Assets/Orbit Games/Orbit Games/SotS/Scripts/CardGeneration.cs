using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGeneration : MonoBehaviour {

    [Header("Card Generation")]
    [Buttons("Full", "GenerateFull", "Colors", "GenerateColors")]
    public ButtonsContainer generate;

    [Header("Color override")]
    public OGColorVariableDefinition textColorDefinition;
    public Color textColor;

    [Header("Text definitions")]
    public OGStringVariableDefinition cardsCollected;
    public OGStringVariableDefinition challengesComplete;
    public OGStringVariableDefinition coinsCollected;
    public OGStringVariableDefinition coinsFromCards;
    public OGStringVariableDefinition coinsFromChallenges;
    public OGStringVariableDefinition cardDetails;
    public OGStringVariableDefinition playerName;
    public OGStringVariableDefinition teamName;

    [Header("Boolean Definitions")]
    public OGBooleanVariableDefinition gem1Achieved;
    public OGBooleanVariableDefinition gem2Achieved;
    public OGBooleanVariableDefinition gem3Achieved;
    public OGBooleanVariableDefinition gem4Achieved;
    public OGBooleanVariableDefinition gem5Achieved;
    public OGBooleanVariableDefinition gem6Achieved;

    [Header("Generation settings")]
    public List<ColorGenerationSetting> settings;

    [System.Serializable]
    public class ColorGenerationSetting
    {
        public OGColorVariableDefinition colorDefinition;
        public Type typeIndex;
        public ColorSet set;
        public OGColors.Lightness lightness;
    }

    public enum Type
    {
        Main, Second, Third
    }

    public enum ColorSet
    {
        Card, Avatar
    }

    public void GenerateColors()
    {
        var constructor = GetComponent<OGConstructor>();
        var variableValues = new OGConstructionVariables();
        var cardTypes = OGColors.I.GetRandomUniqueColorTypes(3);
        var avatarTypes = OGColors.I.GetRandomUniqueColorTypes(3);

        foreach (var setting in settings)
        {
            variableValues.SetColorValue(setting.colorDefinition, OGColors.I.GetColor(
                (setting.set == ColorSet.Card ? cardTypes : avatarTypes)[(int)setting.typeIndex], setting.lightness).color);
        }

        variableValues.SetColorValue(textColorDefinition, textColor);

        variableValues.SetBooleanValue(gem1Achieved, Random.value > 0.5f);
        variableValues.SetBooleanValue(gem2Achieved, Random.value > 0.5f);
        variableValues.SetBooleanValue(gem3Achieved, Random.value > 0.5f);
        variableValues.SetBooleanValue(gem4Achieved, Random.value > 0.5f);
        variableValues.SetBooleanValue(gem5Achieved, Random.value > 0.5f);
        variableValues.SetBooleanValue(gem6Achieved, Random.value > 0.5f);

        constructor.ApplyVariableValues(variableValues);
    }

    public void GenerateFull()
    {
        var constructor = GetComponent<OGConstructor>();
        var variableValues = new OGConstructionVariables();
        var cardTypes = OGColors.I.GetRandomUniqueColorTypes(3);
        var avatarTypes = OGColors.I.GetRandomUniqueColorTypes(3);

        foreach (var setting in settings)
        {
            variableValues.SetColorValue(setting.colorDefinition, OGColors.I.GetColor(
                (setting.set == ColorSet.Card ? cardTypes : avatarTypes)[(int)setting.typeIndex], setting.lightness).color);
        }

        variableValues.SetColorValue(textColorDefinition, textColor);

        variableValues.SetStringValue(cardsCollected, "10 x");
        variableValues.SetStringValue(challengesComplete, "15 x");
        variableValues.SetStringValue(coinsCollected, "4621");
        variableValues.SetStringValue(coinsFromCards, "922");
        variableValues.SetStringValue(coinsFromChallenges, "1152");
        variableValues.SetStringValue(cardDetails, "The copy of this card was made on 26 december 2018 for Olivier and was worth 30 coins");
        variableValues.SetStringValue(playerName, "Wonderkid");
        variableValues.SetStringValue(teamName, "Team 007");

        variableValues.SetBooleanValue(gem1Achieved, Random.value > 0.5f);
        variableValues.SetBooleanValue(gem2Achieved, Random.value > 0.5f);
        variableValues.SetBooleanValue(gem3Achieved, Random.value > 0.5f);
        variableValues.SetBooleanValue(gem4Achieved, Random.value > 0.5f);
        variableValues.SetBooleanValue(gem5Achieved, Random.value > 0.5f);
        variableValues.SetBooleanValue(gem6Achieved, Random.value > 0.5f);

        constructor.GenerateRandomConstructionPlan(variableValues);
    }

}
