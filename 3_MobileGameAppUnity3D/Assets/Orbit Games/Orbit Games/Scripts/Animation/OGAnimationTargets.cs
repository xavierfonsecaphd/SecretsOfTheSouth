using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class OGAnimationTargets
{
    public GameObject gameObject;
    public Transform transform;
    public Image image;
    public CanvasGroup canvasGroup;

    public OGAnimationTargets(GameObject gameObject)
    {
        this.gameObject = gameObject;
        transform = gameObject.transform;
        image = gameObject.GetComponent<Image>();
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
    }
}
