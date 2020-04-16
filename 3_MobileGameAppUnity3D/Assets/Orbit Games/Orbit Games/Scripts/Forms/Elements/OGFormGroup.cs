using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OGFormGroup : OGFormBaseGroup<OGForm.ElementsGroup, OGForm.BaseElement>
{
    public Transform horizontalGroupContainer;
    public Transform verticalGroupContainer;

    protected override void BuildSetup()
    {
        if (elementSetup.direction == OGForm.ElementsGroup.Direction.Horizontal)
        {
            verticalGroupContainer.gameObject.SetActive(false);
            horizontalGroupContainer.gameObject.SetActive(true);
            groupContainer = horizontalGroupContainer;
        }
        else
        {
            verticalGroupContainer.gameObject.SetActive(true);
            horizontalGroupContainer.gameObject.SetActive(false);
            groupContainer = verticalGroupContainer;
        }
        base.BuildSetup();
    }
}
