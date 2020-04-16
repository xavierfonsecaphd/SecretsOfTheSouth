using GameToolkit.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OGFormDropdown : OGFormBaseGroup<OGForm.DropdownElement, OGForm.DropdownOptionElement>
{
    public LocalizedTextBehaviour valueField;
    public GameObject dropdownListContainer;
    public Button dropdownButton;
    public Canvas dropdownListCanvas;
    public int listSortingOrder = 32760;
    //private List<string> values = new List<string>();
    //private OGForm.Data data = new OGForm.Data();

    private void DetermineValue()
    {
        foreach (var e in subelements)
        {
            if (e is OGFormDropdownOption)
            {
                OGFormDropdownOption option = e as OGFormDropdownOption;
                if (option.IsToggled())
                {
                    valueField.FormattedAsset = option.elementSetup.labelText;
                }
            }
        }
    }

    public override void ReadDataFrom(OGForm.Data data, bool resetOtherwise)
    {
        base.ReadDataFrom(data, resetOtherwise);
        DetermineValue();
    }

    protected override void BuildSetup()
    {
        base.BuildSetup();
        DetermineValue();
    }

    public override void PutSelectablesInList(List<Selectable> selectables)
    {
        selectables.Add(dropdownButton);
        base.PutSelectablesInList(selectables);
    }

    public void OpenDropdownList()
    {
        if (subelements.Count == 0) return;
        OGTransition.In(dropdownListContainer);

        dropdownListCanvas.sortingOrder = listSortingOrder;

        // let's move the dropdown list to the first selected item
        float firstY = subelements[0].transform.localPosition.y;
        foreach (var e in subelements)
        {
            OGFormDropdownOption option = e as OGFormDropdownOption;
            if (option != null)
            {
                if (option.IsToggled())
                {
                    // due to some layout building their items after the dropdown is opened,
                    // we need to do our position setting a frame later to be sure it always
                    // works properly
                    OGRun.NextFrame(() =>
                    {
                        Vector3 newPosition = groupContainer.transform.localPosition;
                        newPosition.y = firstY;
                        newPosition.y -= e.transform.localPosition.y;
                        groupContainer.transform.localPosition = newPosition;
                        groupContainer.transform.localPosition = newPosition;
                    });
                    break;
                }
            }
        }
    }

    public void CloseDropdownList()
    {
        OGTransition.Out(dropdownListContainer);
    }
}

