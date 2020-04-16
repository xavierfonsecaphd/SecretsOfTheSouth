using GameToolkit.Localization;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class OGFormBaseGroup<T,W> : OGFormBaseElementGeneric<T> where T : OGForm.Setups.BaseGroupSetup<W> where W : OGForm.BaseElement
{
    public List<OGFormBaseElementBehaviour> subelements = new List<OGFormBaseElementBehaviour>();

    [Header("References")]
    public LocalizedTextBehaviour labelField;
    public Transform groupContainer;

    void Empty()
    {
        foreach (var e in subelements)
        {
            e.Remove();
        }
        subelements.Clear();
    }

    public override void PutSelectablesInList(List<Selectable> selectables)
    {
        foreach (var e in subelements)
        {
            e.PutSelectablesInList(selectables);
        }
    }

    public override void ReadDataFrom(OGForm.Data data, bool resetOtherwise)
    {
        foreach (var e in subelements)
        {
            e.ReadDataFrom(data, resetOtherwise);
        }
    }

    public override void WriteDataTo(OGForm.Data data)
    {
        foreach (var e in subelements)
        {
            e.WriteDataTo(data);
        }
    }

    public override void CalculatePoints(ref int points)
    {
        foreach (var e in subelements)
        {
            e.CalculatePoints(ref points);
        }
    }

    protected virtual OGFormBaseElementBehaviour GetPrefabFromElementSetup(OGForm.BaseElement elementSetup)
    {
        return OGFormManager.GetInstance().GetPrefabFromElementSetup(elementSetup);
    }

    protected override void BuildSetup()
    {
        Empty();

        if (elementSetup.labelText != null)
        {
            labelField.gameObject.SetActive(true);
            labelField.FormattedAsset = elementSetup.labelText;
        }
        else
        {
            labelField.LocalizedAsset = OGLocalization.EmptyText;
            labelField.gameObject.SetActive(false);
        }

        foreach (var subelementSetup in elementSetup.subelements)
        {
            var baseSetup = elementSetup as OGForm.Setups.BaseElementSetup;
            var prefab = GetPrefabFromElementSetup(subelementSetup);
            var parent = groupContainer;

            if (baseSetup != null && baseSetup.alternativeParent != null)
                parent = baseSetup.alternativeParent;

            var copy = OGPool.placeCopy(prefab, parent);
            copy.Setup(parentForm, subelementSetup, this);
            subelements.Add(copy);
        }
    }

    public override void Remove()
    {
        Empty();
        OGPool.removeCopy(this);
    }
}
