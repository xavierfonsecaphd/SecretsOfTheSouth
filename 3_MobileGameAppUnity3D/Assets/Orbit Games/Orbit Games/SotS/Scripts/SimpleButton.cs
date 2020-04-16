using GameToolkit.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SimpleButton : MonoBehaviour {

    public EditorHelpBox helpBox;
    public void EditorHelpBoxUpdate()
    {
        if (iconField == null) helpBox = EditorHelpBox.Error("Missing icon Image");
        else if (buttonBarField == null) helpBox = EditorHelpBox.Error("Missing button bar Image");
        else if (localizedTextField == null) helpBox = EditorHelpBox.Error("Missing localized text field");
        else if (icon == null) helpBox = EditorHelpBox.Warning("Missing icon");
        else if (buttonColor == Color.clear) helpBox = EditorHelpBox.Warning("Missing color");
        else if (onClickEvent.GetPersistentEventCount() == 0) helpBox = EditorHelpBox.Warning("No on click event is registered in inspector.");
        else helpBox = EditorHelpBox.None();
    }

    [Header("Settings")]
    public OGFormattedLocalizedText localizedText;
    public Sprite icon;
    public Color buttonColor;
    public UnityEvent onClickEvent;

    private Action onClickByCode;

    [Header("References")]
    public Image buttonBarField;
    public Image iconField;
    public LocalizedTextBehaviour localizedTextField;

    public void Setup(OGFormattedLocalizedText localizedText, Sprite icon, Color buttonColor, Action onClick)
    {
        onClickByCode = onClick;
        this.localizedText = localizedText;
        this.icon = icon;
        this.buttonColor = buttonColor;
        SetSettings();
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (!iconField) return;
        if (!localizedTextField) return;
        SetSettings();
    }
#endif

    private void SetSettings()
    {
        iconField.sprite = icon;
        buttonBarField.color = buttonColor;
        localizedTextField.FormattedAsset = localizedText;
    }

    public void OnClick()
    {
        onClickEvent.Invoke();
        if (onClickByCode != null)
            onClickByCode.Invoke();
    }
}
