using GameToolkit.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OGPopup : OGSingletonBehaviour<OGPopup> {

    protected override void OnSingletonInitialize() { }
    
    private static Stack<Setup> popupsShowing =  new Stack<Setup>();

    public static bool IsPopupActive()
    {
        return popupsShowing.Count > 0;
    }

    private static void PushPopup(Setup popupSetup)
    {
        popupsShowing.Push(popupSetup);
    }

    private static void PopPopup()
    {
        popupsShowing.Pop();
    }

    [Header("References")]
    public Transform popupBase;
    public OGPopupWindow popupWindowPrefab;

    public Sprite notificationIcon;
    public Color notificationColor;
    public Sprite confirmIcon;
    public Color confirmColor;
    public Sprite promptIcon;
    public Color promptColor;
    public Sprite warningIcon;
    public Color warningColor;
    public Sprite errorIcon;
    public Color errorColor;

    [Header("Localization")]
    public LocalizedText cancelButtonText;
    public LocalizedText acceptButtonText;
    public LocalizedText submitButtonText;
    public LocalizedText closeButtonText;
    public LocalizedText errorTitle;
    public LocalizedText warningTitle;
    public LocalizedText notificationTitle;
    public LocalizedText confirmTitle;
    public LocalizedText confirmDeleteDescription;

    [Space(10f)]
    public LocalizedText titleUnsavedChanges;
    public LocalizedText descriptionUnsavedChanges;
    public LocalizedText labelKeepAndStay;
    public LocalizedText labelDiscardAndContinue;


    public enum PopupType
    {
        NOTIFY, CONFIRM, PROMPT, WARNING, ERROR
    }

    public Sprite GetIcon(PopupType type)
    {
        switch (type)
        {
            case PopupType.CONFIRM:
                return confirmIcon;
            case PopupType.PROMPT:
                return promptIcon;
            case PopupType.WARNING:
                return warningIcon;
            case PopupType.ERROR:
                return errorIcon;
            case PopupType.NOTIFY:
            default:
                return notificationIcon;
        }
    }

    public Color GetColor(PopupType type)
    {
        switch (type)
        {
            case PopupType.CONFIRM:
                return confirmColor;
            case PopupType.PROMPT:
                return promptColor;
            case PopupType.WARNING:
                return warningColor;
            case PopupType.ERROR:
                return errorColor;
            case PopupType.NOTIFY:
            default:
                return notificationColor;
        }
    }

    public static Setup MakeNotificationPopup(OGFormattedLocalizedText title, OGFormattedLocalizedText message, Action onClose = null)
    {
        return ((TypeSetup)new Setup(title, message)).SetNotify(onClose);
    }

    public static Setup MakeNotificationPopup(OGFormattedLocalizedText message, Action onClose = null)
    {
        return ((TypeSetup)new Setup(I.notificationTitle, message)).SetNotify(onClose);
    }

    public static Setup MakeErrorPopup(OGFormattedLocalizedText title, OGFormattedLocalizedText message, Action onClose = null)
    {
        return ((TypeSetup)new Setup(title, message)).SetError(onClose);
    }

    public static Setup MakeErrorPopup(OGFormattedLocalizedText message, Action onClose = null)
    {
        return ((TypeSetup)new Setup(I.errorTitle, message)).SetError(onClose);
    }

    public static Setup MakeWarningPopup(OGFormattedLocalizedText title, OGFormattedLocalizedText message, Action onClose = null)
    {
        return ((TypeSetup)new Setup(title, message)).SetWarning(onClose);
    }

    public static Setup MakeWarningPopup(OGFormattedLocalizedText message, Action onClose = null)
    {
        return ((TypeSetup)new Setup(I.warningTitle, message)).SetWarning(onClose);
    }

    public static Setup MakeConfirmPopup(OGFormattedLocalizedText title, OGFormattedLocalizedText message, Action onPositive = null, Action onNegative = null)
    {
        return ((TypeSetup)new Setup(title, message)).SetConfirm(onNegative, onPositive);
    }

    public static Setup MakeConfirmPopup(OGFormattedLocalizedText message, Action onPositive = null, Action onNegative = null)
    {
        return ((TypeSetup)new Setup(I.confirmTitle, message)).SetConfirm(onNegative, onPositive);
    }

    public static Setup MakeConfirmDeletePopup(Action onPositive = null, Action onNegative = null)
    {
        return ((TypeSetup)new Setup(I.confirmTitle, I.confirmDeleteDescription)).SetConfirm(onNegative, onPositive);
    }

    public static Setup MakeConfirmDiscardChangesPopup(Action onPositive = null, Action onNegative = null)
    {
        var popup = OGPopup.MakeConfirmPopup(I.titleUnsavedChanges, I.descriptionUnsavedChanges, onPositive, onNegative);
        popup.OverrideNegativeButtonText(I.labelKeepAndStay);
        popup.OverridePositiveButtonText(I.labelDiscardAndContinue);
        return popup;
    }

    public static Setup MakePromptPopup(OGFormattedLocalizedText title, OGFormattedLocalizedText message, Func<OGForm.Data, bool> verifier, Action<OGForm.Data> onSubmit = null, Action onCancel = null)
    {
        return ((TypeSetup)new Setup(title, message)).SetPrompt(verifier, onSubmit, onCancel);
    }

    public static Setup MakePromptPopup(OGFormattedLocalizedText title, OGFormattedLocalizedText message, Action<OGForm.Data> onSubmit = null, Action onCancel = null)
    {
        return ((TypeSetup)new Setup(title, message)).SetPrompt(SimplyAcceptTheData, onSubmit, onCancel);
    }

    public static Setup MakeDemandPopup(OGFormattedLocalizedText title, OGFormattedLocalizedText message, Func<OGForm.Data, bool> verifier, Action<OGForm.Data> onSubmit = null)
    {
        return ((TypeSetup)new Setup(title, message)).SetDemand(verifier, onSubmit);
    }

    public static Setup MakeDemandPopup(OGFormattedLocalizedText title, OGFormattedLocalizedText message, Action<OGForm.Data> onSubmit = null)
    {
        return ((TypeSetup)new Setup(title, message)).SetDemand(SimplyAcceptTheData, onSubmit);
    }
    
    public static Setup ShowNotificationPopup(OGFormattedLocalizedText title, OGFormattedLocalizedText message, Action onClose = null)
    {
        return ((TypeSetup)new Setup(title, message)).SetNotify(onClose).Show();
    }

    public static Setup ShowNotificationPopup(OGFormattedLocalizedText message, Action onClose = null)
    {
        return ((TypeSetup)new Setup(I.notificationTitle, message)).SetNotify(onClose).Show();
    }

    public static Setup ShowErrorPopup(OGFormattedLocalizedText title, OGFormattedLocalizedText message, Action onClose = null)
    {
        return ((TypeSetup)new Setup(title, message)).SetError(onClose).Show();
    }

    public static Setup ShowErrorPopup(OGFormattedLocalizedText message, Action onClose = null)
    {
        return ((TypeSetup)new Setup(I.errorTitle, message)).SetError(onClose).Show();
    }

    public static Setup ShowWarningPopup(OGFormattedLocalizedText title, OGFormattedLocalizedText message, Action onClose = null)
    {
        ShowWarningPopup("", "", () =>
        {
            
        });

        return ((TypeSetup)new Setup(title, message)).SetWarning(onClose).Show();
    }

    public static Setup ShowWarningPopup(OGFormattedLocalizedText message, Action onClose = null)
    {
        return ((TypeSetup)new Setup(I.warningTitle, message)).SetWarning(onClose).Show();
    }

    public static Setup ShowConfirmPopup(OGFormattedLocalizedText title, OGFormattedLocalizedText message, Action onPositive = null, Action onNegative = null)
    {
        return ((TypeSetup)new Setup(title, message)).SetConfirm(onNegative, onPositive).Show();
    }

    public static Setup ShowConfirmPopup(OGFormattedLocalizedText message, Action onPositive = null, Action onNegative = null)
    {
        return ((TypeSetup)new Setup(I.confirmTitle, message)).SetConfirm(onNegative, onPositive).Show();
    }

    public static Setup ShowConfirmDeletePopup(Action onPositive = null, Action onNegative = null)
    {
        return ((TypeSetup)new Setup(I.confirmTitle, I.confirmDeleteDescription)).SetConfirm(onNegative, onPositive).Show();
    }

    public static Setup ShowConfirmDiscardChangesPopup(Action onPositive = null, Action onNegative = null)
    {
        var popup = OGPopup.MakeConfirmPopup(I.titleUnsavedChanges, I.descriptionUnsavedChanges, onPositive, onNegative);
        popup.OverrideNegativeButtonText(I.labelKeepAndStay);
        popup.OverridePositiveButtonText(I.labelDiscardAndContinue);
        return popup.Show();
    }

    public static Setup ShowPromptPopup(OGFormattedLocalizedText title, OGFormattedLocalizedText message, Func<OGForm.Data, bool> verifier, Action<OGForm.Data> onSubmit = null, Action onCancel = null)
    {
        return ((TypeSetup)new Setup(title, message)).SetPrompt(verifier, onSubmit, onCancel).Show();
    }

    public static Setup ShowPromptPopup(OGFormattedLocalizedText title, OGFormattedLocalizedText message, Action<OGForm.Data> onSubmit = null, Action onCancel = null)
    {
        return ((TypeSetup)new Setup(title, message)).SetPrompt(SimplyAcceptTheData, onSubmit, onCancel).Show();
    }

    public static Setup ShowDemandPopup(OGFormattedLocalizedText title, OGFormattedLocalizedText message, Func<OGForm.Data, bool> verifier, Action<OGForm.Data> onSubmit = null)
    {
        return ((TypeSetup)new Setup(title, message)).SetDemand(verifier, onSubmit).Show();
    }

    public static Setup ShowDemandPopup(OGFormattedLocalizedText title, OGFormattedLocalizedText message, Action<OGForm.Data> onSubmit = null)
    {
        return ((TypeSetup)new Setup(title, message)).SetDemand(SimplyAcceptTheData, onSubmit).Show();
    }

    private static bool SimplyAcceptTheData(OGForm.Data data)
    {
        return true;
    }

    private interface TypeSetup
    {
        Setup SetError(Action onClose);
        Setup SetWarning(Action onClose);
        Setup SetNotify(Action onClose);
        Setup SetConfirm(Action onNegative, Action onPositive);
        Setup SetPrompt(Func<OGForm.Data, bool> dataVerifier, Action<OGForm.Data> onSubmit, Action onCancel);
        Setup SetDemand(Func<OGForm.Data, bool> dataVerifier, Action<OGForm.Data> onSubmit);
    }

    public class Setup : TypeSetup
    {
        public OGPopup popupSystem { get; private set; }
        public OGPopupWindow popupWindow { get; private set; }
        public OGForm.Setup formSetup { get; private set; }
        public OGForm.SetupHelper formHelper { get; private set; }

        // closing/canceling
        private Action onClose { get; set; }

        // raw image
        private Texture2D textureInsteadOfIcon;

        // submitting
        private Action onAccept { get; set; }
        private Action<OGForm.Data> onSubmit { get; set; }
        private Func<OGForm.Data, bool> dataVerifier { get; set; }

        public PopupType type { get; private set; }
        private bool closable = true;

        public Setup(OGFormattedLocalizedText title, OGFormattedLocalizedText message)
        {
            popupSystem = GetInstance();
            formHelper = OGForm.GenerateBasicForm(title, message, popupSystem.closeButtonText, (data) => {
                if (dataVerifier != null)
                {
                    if (!dataVerifier(data))
                    {
                        Debug.Log("data could not be verified");
                        return;
                    }
                }
                if (onSubmit != null) onSubmit.Invoke(data);
                if (onAccept != null) onAccept.Invoke();
                Close();
            });
            formSetup = formHelper.formSetup;
            type = PopupType.NOTIFY;
        }

        public void Close()
        {
            if (!closable) return;
            if (!IsShowing()) return;
            popupWindow.CloseWindow();
            popupWindow = null;
            PopPopup();
        }

        public void SetTextureInsteadOfIcon(Texture2D texture)
        {
            textureInsteadOfIcon = texture;
        }

        public Texture2D GetTextureInsteadOfIcon()
        {
            return textureInsteadOfIcon;
        }

        public bool HasTextureInsteadOfIcon()
        {
            return textureInsteadOfIcon != null;
        }

        public void SetClosable(bool closable)
        {
            this.closable = closable;
        }

        public Setup SetOnClose(Action onClose)
        {
            this.onClose = onClose;
            return this;
        }

        public Setup SetOnAccept(Action onAccept)
        {
            this.onAccept = onAccept;
            return this;
        }

        public Setup SetOnSubmit(Action<OGForm.Data> onSubmit)
        {
            this.onSubmit = onSubmit;
            return this;
        }

        public Setup SetDataVerifier(Func<OGForm.Data, bool> dataVerifier)
        {
            this.dataVerifier = dataVerifier;
            return this;
        }

        Setup TypeSetup.SetError(Action onClose)
        {
            type = PopupType.ERROR;
            SetButtonStyle(0, OGForm.ButtonStyle.Default);
            return SetOnClose(onClose).SetOnAccept(onClose);
        }

        Setup TypeSetup.SetWarning(Action onClose)
        {
            type = PopupType.WARNING;
            SetButtonStyle(0, OGForm.ButtonStyle.Default);
            return SetOnClose(onClose).SetOnAccept(onClose);
        }

        Setup TypeSetup.SetNotify(Action onClose)
        {
            type = PopupType.NOTIFY;
            SetButtonStyle(0, OGForm.ButtonStyle.Default);
            return SetOnClose(onClose).SetOnAccept(onClose);
        }

        Setup TypeSetup.SetConfirm(Action onNegative, Action onPositive)
        {
            type = PopupType.CONFIRM;
            SetOnClose(onNegative);
            SetOnAccept(onPositive);

            // create the negative button
            formHelper.AddCustomButton(popupSystem.cancelButtonText, () =>
            {
                if (onClose != null) onClose.Invoke();
                Close();
            });

            // swap buttons to set the positive button as our second button
            SwapButtons();

            SetButtonStyle(0, OGForm.ButtonStyle.Cancel);
            SetButtonStyle(1, OGForm.ButtonStyle.Accept);

            // set the positive button text
            OverridePositiveButtonText(popupSystem.acceptButtonText);
            return this;
        }

        Setup TypeSetup.SetPrompt(Func<OGForm.Data, bool> dataVerifier, Action<OGForm.Data> onSubmit, Action onCancel)
        {
            type = PopupType.PROMPT;
            SetOnClose(onCancel);
            SetOnAccept(null);
            SetOnSubmit(onSubmit);
            SetDataVerifier(dataVerifier);

            // add the cancel button
            formHelper.AddCustomButton(popupSystem.cancelButtonText, () =>
            {
                if (onClose != null) onClose.Invoke();
                Close();
            });

            // swap buttons to set the submit button as our positive second button
            SwapButtons();

            SetButtonStyle(0, OGForm.ButtonStyle.Cancel);
            SetButtonStyle(1, OGForm.ButtonStyle.Send);

            // set the submit button text
            OverrideSubmitButtonText(popupSystem.submitButtonText);
            return this;
        }

        Setup TypeSetup.SetDemand(Func<OGForm.Data, bool> dataVerifier, Action<OGForm.Data> onSubmit)
        {
            type = PopupType.PROMPT;
            SetOnClose(null);
            SetOnAccept(null);
            SetOnSubmit(onSubmit);
            SetDataVerifier(dataVerifier);

            SetButtonStyle(0, OGForm.ButtonStyle.Send);

            // set the submit button text
            OverrideCloseButtonText(popupSystem.submitButtonText);
            return this;
        }

        public void RemoveButtons()
        {
            formHelper.buttons.subelements.Clear();
        }

        public void AddClosingButton(OGFormattedLocalizedText label, Action onClick, OGForm.ButtonStyle style = OGForm.ButtonStyle.Default)
        {
            formHelper.AddCustomButton(label, () =>
            {
                if (onClick != null) onClick.Invoke();
                Close();
            }, style);
        }

        public void AddSubmitButton(OGFormattedLocalizedText label, Action onClick, OGForm.ButtonStyle style = OGForm.ButtonStyle.Default)
        {
            formHelper.AddSubmitButton(label, () =>
            {
                if (onClick != null) onClick.Invoke();
                Close();
            }, style);
        }

        public void AddNonClosingButton(OGFormattedLocalizedText label, Action onClick, OGForm.ButtonStyle style = OGForm.ButtonStyle.Default)
        {
            formHelper.AddCustomButton(label, () =>
            {
                if (onClick != null) onClick.Invoke();
            }, style);
        }

        private void SwapButtons()
        {
            var temp = formHelper.buttons.subelements[0];
            formHelper.buttons.subelements[0] = formHelper.buttons.subelements[1];
            formHelper.buttons.subelements[1] = temp;
        }

        public void SetButtonStyle(int buttonID, OGForm.ButtonStyle style)
        {
            OGForm.ButtonElement button = (OGForm.ButtonElement)formHelper.buttons.subelements[buttonID];
            button.style = style;
        }

        public void PressNegativeButton()
        {
            OGForm.ButtonElement button = (OGForm.ButtonElement)formHelper.buttons.subelements[0];
            if (button.onPress != null) button.onPress.Invoke();
        }

        public void PressCloseButton()
        {
            OGForm.ButtonElement button = (OGForm.ButtonElement)formHelper.buttons.subelements[0];
            if (button.onPress != null) button.onPress.Invoke();
        }

        public void PressAcceptButton()
        {
            // the last button is always the accept button
            OGForm.ButtonElement button = (OGForm.ButtonElement)formHelper.buttons.subelements[formHelper.buttons.subelements.Count - 1];
            if (button.onPress != null) button.onPress.Invoke();
        }

        public void PressPositiveButton()
        {
            OGForm.ButtonElement button = (OGForm.ButtonElement)formHelper.buttons.subelements[1];
            if (button.onPress != null) button.onPress.Invoke();
        }

        public void PressSubmitButton()
        {
            OGForm.ButtonElement button = (OGForm.ButtonElement)formHelper.buttons.subelements[1];
            if (button.onPress != null) button.onPress.Invoke();
        }

        public Setup OverrideCloseButtonText(OGFormattedLocalizedText label)
        {
            OGForm.ButtonElement button = (OGForm.ButtonElement)formHelper.buttons.subelements[0];
            button.labelText = label;
            return this;
        }

        public Setup OverrideNegativeButtonText(OGFormattedLocalizedText label)
        {
            return OverrideCloseButtonText(label);
        }

        public Setup OverridePositiveButtonText(OGFormattedLocalizedText label)
        {
            OGForm.ButtonElement button = (OGForm.ButtonElement)formHelper.buttons.subelements[formHelper.buttons.subelements.Count - 1];
            button.labelText = label;
            return this;
        }

        public Setup OverrideSubmitButtonText(OGFormattedLocalizedText label)
        {
            return OverridePositiveButtonText(label);
        }

        public bool IsShowing()
        {
            return popupWindow != null;
        }

        public Setup Show()
        {
            if (IsShowing())
            {
                throw new Exception("A window of this setup is already being shown");
            }
            var instance = I;
            popupWindow = OGPool.placeCopy(instance.popupWindowPrefab, instance.popupBase);
            popupWindow.Setup(this);
            PushPopup(this);

            return this;
        }

        public void SetData(string variable, string value)
        {
            if (popupWindow == null)
            {
                throw new Exception("Popup was not yet shown, so no form was available to set data to");
            }
            popupWindow.form.SetValue(variable, value);
        }
    }
}