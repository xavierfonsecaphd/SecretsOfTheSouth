using Fenderrio.ImageWarp;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OGFeatureDetail : MonoBehaviour {

    [Header("Definition")]
    public OGFeatureDetailDefinition definition;

    [NonSerialized]
    private bool initialized = false;
    private Image image;
    private RawImageWarp warpableImage;
    private RawImageWarpSprite warpableImageSprite;
    private bool isInactive = false;
    private void Initialized()
    {
        if (initialized) return;
        initialized = true;

        image = GetComponent<Image>();
        warpableImage = GetComponent<RawImageWarp>();
        warpableImageSprite = GetComponent<RawImageWarpSprite>();
        isInactive = !gameObject.activeSelf;
    }

    public void SetFeatureDetail(Sprite sprite)
    {
        Initialized();
        if (isInactive) return;

        if (warpableImageSprite != null) warpableImageSprite.SetSprite(sprite);
        else if (image != null) image.sprite = sprite;
        else if (warpableImage != null) warpableImage.texture = sprite != null ? sprite.texture : null;

        if (sprite != null)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
