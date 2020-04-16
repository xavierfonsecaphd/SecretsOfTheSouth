using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OGFlasher : MonoBehaviour {

    public float fadeSpeed = 1f;
    public bool useUnscaledTime = false;

    private Image image;
    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void Flash()
    {
        var color = image.color;
        color.a = 1f;
        image.color = color;

        StopAllCoroutines();
        StartCoroutine(fade());
    }

    IEnumerator fade()
    {
        var color = image.color;
        while (color.a >= 0f)
        {
            yield return null;
            color.a = Mathf.Max(0, color.a - (useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime) * fadeSpeed);
            image.color = color;
        }
    }

    public static void FlashChildren(GameObject go)
    {
        go.BroadcastMessage("Flash", SendMessageOptions.DontRequireReceiver);
    }

    public static void FlashChildren(Component component)
    {
        component.BroadcastMessage("Flash", SendMessageOptions.DontRequireReceiver);
    }
}
