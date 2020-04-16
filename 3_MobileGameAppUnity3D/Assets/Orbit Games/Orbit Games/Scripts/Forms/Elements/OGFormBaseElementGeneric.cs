using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OGFormBaseElementGeneric<T> : OGFormBaseElementBehaviour where T : OGForm.BaseElement 
{
    /// <summary>
    /// The setup used to 
    /// </summary>
    public T elementSetup;
    public OGFormBaseElementBehaviour parentElement;
    [Buttons("Setup","BuildSetup")]
    public ButtonsContainer debug;
    
    /// <summary>
    /// Remembers the setup and initiates the construction of this element
    /// </summary>
    public override void Setup(OGForm parentForm, OGForm.BaseElement elementSetup, OGFormBaseElementBehaviour parentElement = null)
    {
        this.parentForm = parentForm;
        this.elementSetup = (T)elementSetup;
        this.parentElement = parentElement;
        BuildSetup();
    }

}