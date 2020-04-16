using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OGSubcanvasSortingHelper : MonoBehaviour {

    public Canvas myCanvas;
    public int difference = 0;

    private void OnDrawGizmos()
    {
        UpdateSortOrder();
    }

    private void OnEnable()
    {
        UpdateSortOrder();
    }

    private void UpdateSortOrder()
    {
        var canvasses = GetComponentsInParent<Canvas>();
        foreach (var canvas in canvasses)
        {
            if (canvas != null && canvas != myCanvas)
            {
                myCanvas.sortingOrder = canvas.sortingOrder + difference;
            }
        }
    }
}
