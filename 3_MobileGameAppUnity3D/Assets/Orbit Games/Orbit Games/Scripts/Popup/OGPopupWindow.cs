using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OGPopupWindow : MonoBehaviour {

    [Header("References")]
    public OGForm form;
    public Transform buttonsContainer;
    public Image typeIcon;
    public Image pattern;
    public Image glow;
    public Image iconFrame;

    public RawImage rawImage;
    public RectTransform rawImageFrame;
    public RectTransform rawImageSizer;

    private OGPopup.Setup popupSetup;

    public void Setup(OGPopup.Setup popupSetup)
    {
        this.popupSetup = popupSetup;
        popupSetup.formHelper.buttons.alternativeParent = buttonsContainer;
        form.SetupForm(popupSetup.formSetup);
        
        var color = popupSetup.popupSystem.GetColor(popupSetup.type);
        pattern.color = color;
        glow.color = color;
        iconFrame.color = color;

        var rect = GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = Vector2.zero;

        if (popupSetup.HasTextureInsteadOfIcon())
        {
            var texture = popupSetup.GetTextureInsteadOfIcon();
            iconFrame.gameObject.SetActive(false);
            rawImageFrame.gameObject.SetActive(true);

            var scale = (100f / texture.width + 180f / texture.height) / 2f;

            rawImageSizer.sizeDelta = new Vector2(scale * texture.width, scale * texture.height);
            rawImage.texture = texture;
        }
        else
        {
            iconFrame.gameObject.SetActive(true);
            rawImageFrame.gameObject.SetActive(false);
            typeIcon.sprite = popupSetup.popupSystem.GetIcon(popupSetup.type);
        }
    }

    public void OnBackgroundPress()
    {
        popupSetup.PressCloseButton();
    }

    public void CloseWindow()
    {
        OGTransition.Out(this);
    }
}
