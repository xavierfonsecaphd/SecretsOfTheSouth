using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(OGRichTextInputFieldSetup), typeof(EventTrigger))]
public class OGRichTextInputField : TMP_InputField {
    
    public List<OGRichTextInputState> history = new List<OGRichTextInputState>();
    public List<OGRichTextInputState> future = new List<OGRichTextInputState>();

    private OGRichTextInputState initialValue;
    private OGRichTextInputState rememberedState;
    private bool rememberedRemoving;
    private bool rememberedpacing;
    private int skipValueChangesInFrame;

    private OGRichTextInputFieldSetup setup;

    protected override void Awake()
    {
        setup = GetComponent<OGRichTextInputFieldSetup>();

        base.Awake();
        
        onEndTextSelection.AddListener(OnTextSelection);
        onValueChanged.AddListener(OnValueChanged);
        onDeselect.AddListener(OnDeselect);
    }

    private void Update()
    {
        caretBlinkRate = isFocused ? 1f : 0.000001f;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        HandleEditButtons();
    }

    private bool BlockTextMeshProCommand()
    {
        return OGInput.IsHoldingEditorCommand();
    }

    private void OnTextSelection(string text, int anchor, int focus)
    {
        if (BlockTextMeshProCommand()) return;
        RememberCurrentState();
        HandleEditButtons();
    }

    private Coroutine handleRichTextCoroutine;
    private void HandleEditButtons()
    {
        setup.undoButton.interactable = history.Count > 0;
        setup.redoButton.interactable = future.Count > 0;

        if (handleRichTextCoroutine != null)
        {
            StopCoroutine(handleRichTextCoroutine);
            handleRichTextCoroutine = null;
        }

        if (gameObject.activeInHierarchy) {
            handleRichTextCoroutine = StartCoroutine(HandleRichTextEditButtons());
        }
    }

    private IEnumerator HandleRichTextEditButtons()
    {
        yield return null;

        setup.boldFullUseIndicator.SetActive(false);
        setup.boldMixUseIndicator.SetActive(false);
        setup.italicFullUseIndicator.SetActive(false);
        setup.italicMixUseIndicator.SetActive(false);
        setup.underlineFullUseIndicator.SetActive(false);
        setup.underlineMixUseIndicator.SetActive(false);
        
        var richText = new OGRichText(this);

        var boldPartial = richText.IsTagPartiallyAppliedToSelection(OGRichTextBooleanTag.BOLD);
        var boldFull = boldPartial && richText.IsTagFullyAppliedToSelection(OGRichTextBooleanTag.BOLD);
        var italicPartial = richText.IsTagPartiallyAppliedToSelection(OGRichTextBooleanTag.ITALIC);
        var italicFull = italicPartial && richText.IsTagFullyAppliedToSelection(OGRichTextBooleanTag.ITALIC);
        var underlinePartial = richText.IsTagPartiallyAppliedToSelection(OGRichTextBooleanTag.UNDERLINE);
        var underlineFull = underlinePartial && richText.IsTagFullyAppliedToSelection(OGRichTextBooleanTag.UNDERLINE);

        setup.boldFullUseIndicator.SetActive(boldFull);
        setup.boldMixUseIndicator.SetActive(boldPartial && !boldFull);
        setup.italicFullUseIndicator.SetActive(italicFull);
        setup.italicMixUseIndicator.SetActive(italicPartial && !italicFull);
        setup.underlineFullUseIndicator.SetActive(underlineFull);
        setup.underlineMixUseIndicator.SetActive(underlinePartial && !underlineFull);
    }

    public void Bold()
    {
        OGFlasher.FlashChildren(setup.boldButton);
        ToggleTagAtSelection(OGRichTextBooleanTag.BOLD);
    }

    public void Italic()
    {
        OGFlasher.FlashChildren(setup.italicButton);
        ToggleTagAtSelection(OGRichTextBooleanTag.ITALIC);
    }

    public void Underline()
    {
        OGFlasher.FlashChildren(setup.underlineButton);
        ToggleTagAtSelection(OGRichTextBooleanTag.UNDERLINE);
    }

    public void ClearFormatting()
    {
        OGFlasher.FlashChildren(setup.clearFormattingButton);
        var richText = new OGRichText(this);
        richText.ClearFormattingToSelection();
        SetRichText(richText.ExportToRichText());
    }

    private void ToggleTagAtSelection(OGRichTextBooleanTag tag)
    {
        var richText = new OGRichText(this);
        richText.ToggleTagOnSelection(tag);
        SetRichText(richText.ExportToRichText());
    }

    public new void SelectAll()
    {
        OGFlasher.FlashChildren(setup.selectAllButton);
        var temp = text;
        SetRichText(temp, 0, temp.Length, blockUndoRedoHandling: true);
    }

    public void Copy()
    {
        OGFlasher.FlashChildren(setup.copyButton);
        var richText = new OGRichText(this);
        GUIUtility.systemCopyBuffer = richText.ExportSelectionToRichText();
    }

    public void Cut()
    {
        OGFlasher.FlashChildren(setup.cutButton);
        var richText = new OGRichText(this);
        GUIUtility.systemCopyBuffer = richText.ExportSelectionToRichText();
        richText.RemoveSelection();
        SetRichText(richText.ExportToRichText());
    }

    public void Paste()
    {
        OGFlasher.FlashChildren(setup.pasteButton);
        var richText = new OGRichText(this);
        richText.ReplaceSelection(GUIUtility.systemCopyBuffer);
        SetRichText(richText.ExportToRichText());
    }

    private void SetRichText(string newText, int selectionAnchor, int selectionFocus, bool blockUndoRedoHandling = false)
    {
        SetRichText(new OGRichTextInputState(newText, selectionAnchor, selectionFocus), blockUndoRedoHandling);
    }

    private void SetRichText(OGRichTextInputState richTextResult, bool blockUndoRedoHandling = false)
    {
        if (!blockUndoRedoHandling)
        {
            LogCurrentStateToHistory();
        }

        SkipOnValueChangeListenerThisFrame();
        richTextResult.Apply(this);
        Select();
        RememberCurrentState();
        HandleEditButtons();
    }

    private void SkipOnValueChangeListenerThisFrame()
    {
        skipValueChangesInFrame = Time.frameCount; // 2, because text is set twice to be sure it updates its selection..
    }

    private void OnValueChanged(string newValue)
    {
        if (skipValueChangesInFrame == Time.frameCount) return;

        bool removing = Input.GetKey(KeyCode.Backspace) || Input.GetKey(KeyCode.Delete);
        bool entering = Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey(KeyCode.Return);
        bool spacing = Input.GetKey(KeyCode.Space);

        if (history.Count == 0)
        {
            LogRememberedStateToHistory();
        }
        else if (future.Count > 0)
        {
            LogRememberedStateToHistory();
        }
        else if (entering)
        {
            LogRememberedStateToHistory();
        }
        else if (!rememberedpacing && spacing)
        {
            LogRememberedStateToHistory();
        }
        else if (!rememberedRemoving && removing)
        {
            LogRememberedStateToHistory();
        }
        else if (rememberedRemoving && !removing)
        {
            LogRememberedStateToHistory();
        }

        RememberCurrentState(spacing, removing);

        HandleEditButtons();
    }

    private void OnDeselect(string newValue)
    {
        OGRun.NextFrame(() =>
        {
            var selected = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            if (selected == setup.undoButton.gameObject) return;
            if (selected == setup.redoButton.gameObject) return;

            //LogCurrentStateToHistory();
        });
    }

    public void Undo()
    {
        if (history.Count > 0)
        {
            OGFlasher.FlashChildren(setup.undoButton);
            LogFuture(new OGRichTextInputState(this));
            var historyValue = history[history.Count - 1];
            SkipOnValueChangeListenerThisFrame();
            historyValue.Apply(this);
            history.RemoveAt(history.Count - 1);
        }
        else
        {
            // OGFlasher.FlashChildren(setup.undoButton);
            if (initialValue.text != text)
            {
                LogFuture(new OGRichTextInputState(this));
            }
            SkipOnValueChangeListenerThisFrame();
            initialValue.Apply(this);
        }
        Select();
        HandleEditButtons();
    }

    public void Redo()
    {
        if (future.Count > 0)
        {
            OGFlasher.FlashChildren(setup.redoButton);
            LogHistory(new OGRichTextInputState(this), blockFutureClear: true);
            var futureValue = future[future.Count - 1];
            SkipOnValueChangeListenerThisFrame();
            futureValue.Apply(this);
            future.RemoveAt(future.Count - 1);
        }
        Select();
        HandleEditButtons();
    }

    private void RememberCurrentState(bool wasSpacing = false, bool wasRemoving = false)
    {
        rememberedState = new OGRichTextInputState(this);
        rememberedpacing = wasSpacing;
        rememberedRemoving = wasRemoving;
    }

    private void LogCurrentStateToHistory(bool blockFutureClear = false)
    {
        RememberCurrentState();
        LogRememberedStateToHistory(blockFutureClear);
    }

    private void LogRememberedStateToHistory(bool blockFutureClear = false)
    {
        LogHistory(rememberedState, blockFutureClear);
    }

    private void LogHistory(OGRichTextInputState value, bool blockFutureClear = false)
    {
        if (history.Count > 0)
        {
            if (!history[history.Count - 1].text.Equals(value.text))
            {
                history.Add(value);
            }
        }
        else
        {
            history.Add(value);
        }

        if (!blockFutureClear)
        {
            Debug.Log("Loggin a history point and clearing the future!");
            future.Clear();
        }

        HandleEditButtons();
    }

    private void LogFuture(OGRichTextInputState value)
    {
        if (future.Count > 0 && !future[future.Count - 1].text.Equals(value.text))
        {
            if (!future[future.Count - 1].text.Equals(value.text))
            {
                future.Add(value);
            }
        }
        else
        {
            future.Add(value);
        }

        HandleEditButtons();
    }

    public void InitializeInput(string initialValue)
    {
        history.Clear();
        future.Clear();

        SkipOnValueChangeListenerThisFrame();
        this.initialValue = new OGRichTextInputState(initialValue);
        this.initialValue.Apply(this);

        rememberedState = this.initialValue;
        rememberedRemoving = false;
        rememberedpacing = false;

        HandleEditButtons();
    }


    /// <summary>
    /// Handle the specified event.
    /// </summary>
    private Event m_ProcessingEvent = new Event();

    public override void OnUpdateSelected(BaseEventData eventData)
    {
        if (!isFocused)
            return;

        bool consumedEvent = false;
        while (Event.PopEvent(m_ProcessingEvent))
        {
            if (m_ProcessingEvent.rawType == EventType.KeyDown)
            {
                consumedEvent = true;

                if (PerformPossibleCommand(m_ProcessingEvent))
                {
                    UpdateLabel();
                    eventData.Use();
                    break;
                }
                else
                {
                    var shouldContinue = KeyPressed(m_ProcessingEvent);
                    if (shouldContinue == EditState.Finish)
                    {
                        SendOnSubmit();
                        DeactivateInputField();
                        break;
                    }
                }
            }

            switch (m_ProcessingEvent.type)
            {
                case EventType.ValidateCommand:
                case EventType.ExecuteCommand:
                    switch (m_ProcessingEvent.commandName)
                    {
                        case "SelectAll":
                            SelectAll();
                            consumedEvent = true;
                            break;
                    }
                    break;
            }
        }

        if (consumedEvent)
            UpdateLabel();

        eventData.Use();
    }

    private bool PerformPossibleCommand(Event evt)
    {
        var currentEventModifiers = evt.modifiers;
        RuntimePlatform rp = Application.platform;
        bool isMac = (rp == RuntimePlatform.OSXEditor || rp == RuntimePlatform.OSXPlayer);
        bool ctrl = isMac ? (currentEventModifiers & EventModifiers.Command) != 0 : (currentEventModifiers & EventModifiers.Control) != 0;
        bool shift = (currentEventModifiers & EventModifiers.Shift) != 0;
        bool alt = (currentEventModifiers & EventModifiers.Alt) != 0;
        bool ctrlOnly = ctrl && !alt && !shift;

        if (Application.isEditor)
        {
            ctrlOnly = ctrlOnly || (ctrl && alt && shift);
        }

        switch (evt.keyCode)
        {
            // Selection
            case KeyCode.DownArrow:
            case KeyCode.UpArrow:
            case KeyCode.LeftArrow:
            case KeyCode.RightArrow:
                HandleEditButtons();
                return false;

            // Select All
            case KeyCode.A:
                {
                    if (ctrlOnly)
                    {
                        SelectAll();
                        return true;
                    }
                    break;
                }
            // Undo
            case KeyCode.Z:
                {
                    if (ctrlOnly)
                    {
                        Undo();
                        return true;
                    }
                    break;
                }

            // Redo
            case KeyCode.Y:
                {
                    if (ctrlOnly)
                    {
                        Redo();
                        return true;
                    }
                    break;
                }

            // Bold
            case KeyCode.B:
                {
                    if (ctrlOnly)
                    {
                        Bold();
                        return true;
                    }
                    break;
                }

            // Italic
            case KeyCode.I:
                {
                    if (ctrlOnly)
                    {
                        Italic();
                        return true;
                    }
                    break;
                }

            // Underline
            case KeyCode.U:
                {
                    if (ctrlOnly)
                    {
                        Underline();
                        return true;
                    }
                    break;
                }

            // Copy
            case KeyCode.C:
                {
                    if (ctrlOnly)
                    {
                        Copy();
                        return true;
                    }
                    break;
                }

            // Paste
            case KeyCode.V:
                {
                    if (ctrlOnly)
                    {
                        Paste();
                        return true;
                    }
                    break;
                }

            // Cut
            case KeyCode.X:
                {
                    if (ctrlOnly)
                    {
                        Cut();
                        return true;
                    }
                    break;
                }
        }

        return false;
    }
}
