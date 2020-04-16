using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OGRichTextInputFieldSetup : MonoBehaviour {

    [Header("References")]
    public Button boldButton;
    public Button italicButton;
    public Button underlineButton;
    public Button clearFormattingButton;

    public Button selectAllButton;
    public Button cutButton;
    public Button copyButton;
    public Button pasteButton;

    public Button undoButton;
    public Button redoButton;

    [Header("Indicators")]
    public GameObject boldFullUseIndicator;
    public GameObject boldMixUseIndicator;

    public GameObject italicFullUseIndicator;
    public GameObject italicMixUseIndicator;

    public GameObject underlineFullUseIndicator;
    public GameObject underlineMixUseIndicator;
}
