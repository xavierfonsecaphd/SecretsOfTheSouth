using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class ConditionalFieldAttribute : PropertyAttribute
{
    public Condition[] Conditions;
    public bool OnlyDisableField;
    public bool OrConditions;

    public ConditionalFieldAttribute(string propertyToCheck, bool shouldEqual, object compareValue)
    {
        Conditions = new Condition[1] { new Condition(propertyToCheck, shouldEqual, compareValue) };
        OnlyDisableField = false;
        OrConditions = false;
    }

    public ConditionalFieldAttribute(string propertyToCheck, bool shouldEqual, params object[] compareValues)
    {
        Conditions = new Condition[1] { new Condition(propertyToCheck, shouldEqual, compareValues) };
        OnlyDisableField = false;
        OrConditions = false;
    }

    public ConditionalFieldAttribute(string propertyToCheck, params object[] compareValues)
    {
        Conditions = new Condition[1] { new Condition(propertyToCheck, true, compareValues) };
        OnlyDisableField = false;
        OrConditions = false;
    }

    public ConditionalFieldAttribute(string propertyToCheck, object compareValue)
    {
        Conditions = new Condition[1] { new Condition(propertyToCheck, true, compareValue) };
        OnlyDisableField = false;
        OrConditions = false;
    }

    public ConditionalFieldAttribute(string propertyToCheck1, object compareValue1, string propertyToCheck2, object compareValue2)
    {
        Conditions = new Condition[2] { new Condition(propertyToCheck1, true, compareValue1), new Condition(propertyToCheck2, true, compareValue2) };
        OnlyDisableField = false;
        OrConditions = false;
    }

    public ConditionalFieldAttribute(string propertyToCheck1, object compareValue1, string propertyToCheck2, object compareValue2, string propertyToCheck3, object compareValue3)
    {
        Conditions = new Condition[3] { new Condition(propertyToCheck1, true, compareValue1), new Condition(propertyToCheck2, true, compareValue2), new Condition(propertyToCheck3, true, compareValue3) };
        OnlyDisableField = false;
        OrConditions = false;
    }

    public ConditionalFieldAttribute(params Condition[] conditions)
    {
        Conditions = conditions;
        OnlyDisableField = false;
        OrConditions = false;
    }

    public ConditionalFieldAttribute(bool onlyDisableField, string propertyToCheck, bool shouldEqual, object compareValue)
    {
        Conditions = new Condition[1] { new Condition(propertyToCheck, shouldEqual, compareValue) };
        OnlyDisableField = onlyDisableField;
        OrConditions = false;
    }

    public ConditionalFieldAttribute(bool onlyDisableField, string propertyToCheck, bool shouldEqual, params object[] compareValues)
    {
        Conditions = new Condition[1] { new Condition(propertyToCheck, shouldEqual, compareValues) };
        OnlyDisableField = onlyDisableField;
        OrConditions = false;
    }

    public ConditionalFieldAttribute(bool onlyDisableField, string propertyToCheck, params object[] compareValues)
    {
        Conditions = new Condition[1] { new Condition(propertyToCheck, true, compareValues) };
        OnlyDisableField = onlyDisableField;
        OrConditions = false;
    }

    public ConditionalFieldAttribute(bool onlyDisableField, string propertyToCheck, object compareValue)
    {
        Conditions = new Condition[1] { new Condition(propertyToCheck, true, compareValue) };
        OnlyDisableField = onlyDisableField;
        OrConditions = false;
    }

    public ConditionalFieldAttribute(bool onlyDisableField, string propertyToCheck1, object compareValue1, string propertyToCheck2, object compareValue2)
    {
        Conditions = new Condition[2] { new Condition(propertyToCheck1, true, compareValue1), new Condition(propertyToCheck2, true, compareValue2) };
        OnlyDisableField = onlyDisableField;
        OrConditions = false;
    }

    public ConditionalFieldAttribute(bool orConditions, bool onlyDisableField, string propertyToCheck1, object compareValue1, string propertyToCheck2, object compareValue2)
    {
        Conditions = new Condition[2] { new Condition(propertyToCheck1, true, compareValue1), new Condition(propertyToCheck2, true, compareValue2) };
        OnlyDisableField = onlyDisableField;
        OrConditions = orConditions;
    }

    public ConditionalFieldAttribute(bool onlyDisableField, string propertyToCheck1, object compareValue1, string propertyToCheck2, object compareValue2, string propertyToCheck3, object compareValue3)
    {
        Conditions = new Condition[3] { new Condition(propertyToCheck1, true, compareValue1), new Condition(propertyToCheck2, true, compareValue2), new Condition(propertyToCheck3, true, compareValue3) };
        OnlyDisableField = onlyDisableField;
        OrConditions = false;
    }

    public ConditionalFieldAttribute(bool onlyDisableField, params Condition[] conditions)
    {
        Conditions = conditions;
        OnlyDisableField = onlyDisableField;
        OrConditions = false;
    }

    public ConditionalFieldAttribute(bool orConditions, bool onlyDisableField, params Condition[] conditions)
    {
        Conditions = conditions;
        OnlyDisableField = onlyDisableField;
        OrConditions = orConditions;
    }

}

public class Condition
{
    public string propertyToCheck;
    public object[] compareValues;
    public bool shouldEqual;

    public Condition(string propertyToCheck, bool shouldEqual, object[] compareValues)
    {
        this.propertyToCheck = propertyToCheck;
        this.compareValues = compareValues;
        this.shouldEqual = shouldEqual;
    }

    public Condition(string propertyToCheck, bool shouldEqual, object compareValue)
        : this(propertyToCheck, shouldEqual, new object[1] { compareValue }) { }
}