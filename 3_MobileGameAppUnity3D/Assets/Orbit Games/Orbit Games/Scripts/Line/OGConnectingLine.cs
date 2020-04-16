using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class OGConnectinOGine : MonoBehaviour
{
    [Header("References")]
    public RectTransform rectTransform;
    public Transform from;
    public Transform to;

    [Header("Settings")]
    public bool ignoreZ = false;

    private void Reset()
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }
    }

    public void LateUpdate() 
	{
        if (from == null || to == null || rectTransform == null) return;
        Vector3 fromPos = from.position;
        Vector3 toPos = to.position;
        Vector3 diff = ignoreZ ? (Vector3)((Vector2)toPos - (Vector2)fromPos) : toPos - fromPos;

        rectTransform.position = fromPos + diff * 0.5f;
        rectTransform.rotation = Quaternion.LookRotation(Vector3.forward, diff);
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, diff.magnitude / transform.parent.lossyScale.y);
	}
}

