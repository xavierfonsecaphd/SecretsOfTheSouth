using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class OGFormBaseElementBehaviour : MonoBehaviour
{
    [Header("Dynamically defined setup")]
    /// <summary>
    /// The form this element belongs to
    /// </summary>
    public OGForm parentForm;
    /// <summary>
    /// Remembers the setup and initiates the construction of this element
    /// </summary>
    public abstract void Setup(OGForm form, OGForm.BaseElement elementSetup, OGFormBaseElementBehaviour parentElement = null);
    /// <summary>
    /// Build this form UI according to the given setup
    /// </summary>
    protected abstract void BuildSetup();
    /// <summary>
    /// Reads new data belonging to this form element and sets UI to corresponding value
    /// </summary>
    /// <param name="resetOtherwise">if this element couldn't find data for this field, should it reset back to its default value?</param>
    public abstract void ReadDataFrom(OGForm.Data data, bool resetOtherwise);
    /// <summary>
    /// Writes data obtained from data filled in by user in current form element
    /// </summary>
    public abstract void WriteDataTo(OGForm.Data data);
    /// <summary>
    /// Depending on the values in this element, adds or removes points according to its defined points value
    /// </summary>
    public abstract void CalculatePoints(ref int points);
    /// <summary>
    /// Puts all the selectables of this form element into the list of selectables
    /// </summary>
    public abstract void PutSelectablesInList(List<Selectable> selectables);
    /// <summary>
    /// Destroys this element
    /// </summary>
    public abstract void Remove();
}
