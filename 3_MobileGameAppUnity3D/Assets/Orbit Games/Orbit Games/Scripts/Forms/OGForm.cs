using GameToolkit.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OGForm : MonoBehaviour, IOGPoolEventsListener
{
    #region setups
    // ###################################
    // ------------ SETUPS -------------
    // ###################################

    public enum ButtonType
    {
        Submit, Reset, Custom
    }

    public enum ButtonStyle
    {
        Send, Accept, Cancel, Delete, Reset, Default, RandomColor, Custom
    }

    public static class Setups
    {

        [Serializable]
        public class BaseElementSetup
        {
            public Transform alternativeParent;
            public OGFormattedLocalizedText labelText;
            public OGFormattedLocalizedText descriptionText;
            public bool disabled;
        }

        [Serializable]
        public class ButtonSetup : BaseElementSetup
        {
            public ButtonType buttonType;
            public Action onPress;
            public ButtonStyle style = ButtonStyle.RandomColor;
            public Sprite overrideIcon;
            public Color? overrideColor;
        }

        [Serializable]
        public class BaseFieldSetup : BaseElementSetup
        {
            public string variableName;
            public string defaultValue = "";
        }

        [Serializable]
        public class TextFieldSetup : BaseFieldSetup
        {
            public TMPro.TMP_InputField.ContentType contentType = TMPro.TMP_InputField.ContentType.Standard;
            public OGFormattedLocalizedText hintText;
            public int characterLimit = 1024 * 32;
            public int preferredWidth = 260;
        }

        [Serializable]
        public class TextAreaSetup : TextFieldSetup
        {
            public int preferredHeight = 160;
            public TextAreaSetup()
            {
                preferredWidth = 360;
            }
        }

        [Serializable]
        public class OptionSetup : BaseElementSetup
        {
            public string optionValue;
            public int optionPoints;
        }

        [Serializable]
        public class BooleanSetup : BaseFieldSetup
        {
            public int optionPoints;
        }

        [Serializable]
        public class BaseGroupSetup<T> : BaseElementSetup, BaseElement // design flaw cheat, needed due to monobehaviour hierarchy for groups to avoid duplicate code
        {
            public List<T> subelements = new List<T>();
            public W AddElement<W>(W element) where W : T
            {
                subelements.Add(element);
                return element;
            }
        }

        [Serializable]
        public class BaseFieldGroupSetup<T> : BaseGroupSetup<T>
        {
            // design flaw cheat, needed because I can't override both the basegroupsetup AND the basefieldsetup
            public string variableName;
            public string defaultValue = "";
        }
    }
    #endregion
    #region elements
    // ###################################
    // ------------ ELEMENTS -------------
    // ###################################

    public interface BaseElement { }
    
    public class TitleElement : Setups.BaseElementSetup, BaseElement { }
    
    public class HeaderElement : Setups.BaseElementSetup, BaseElement { }
    
    public class PlainTextElement : Setups.BaseElementSetup, BaseElement
    {
        public int preferredWidth = 160;
    }
    
    public class TextFieldElement : Setups.TextFieldSetup, BaseElement { }
    
    public class TextAreaElement : Setups.TextAreaSetup, BaseElement { }
    
    public class RichTextAreaElement : Setups.TextAreaSetup, BaseElement { }
    
    public class RadioButtonsElement : Setups.BaseFieldGroupSetup<RadioButtonOptionElement>, BaseElement { }
    
    public class RadioButtonOptionElement : Setups.OptionSetup, BaseElement { }
    
    public class CheckboxesElement : Setups.BaseGroupSetup<CheckboxOptionElement>, BaseElement { }
    
    public class CheckboxOptionElement : Setups.BooleanSetup, BaseElement { }
    
    public class DropdownElement : Setups.BaseFieldGroupSetup<DropdownOptionElement>, BaseElement { }
    
    public class DropdownOptionElement : Setups.OptionSetup, BaseElement { }
    
    public class ElementsGroup : Setups.BaseGroupSetup<BaseElement>
    {
        public enum Direction { Horizontal, Vertical }
        public Direction direction = Direction.Horizontal;
    }
    
    public class ButtonElement : Setups.ButtonSetup, BaseElement { }


    #endregion
    #region data
    // #############################
    // ------------ DATA -----------
    // #############################

    [Serializable]
    public struct VarValuePair
    {
        public string name;
        public string value;

        public override string ToString()
        {
            return "" + name + " = " + value + "";
        }
    }

    [Serializable]
    public class Data
    {
        public List<VarValuePair> data = new List<VarValuePair>();
        Dictionary<string, int> varToIndex = new Dictionary<string, int>();

        public int Count
        {
            get
            {
                return data.Count;
            }
        }

        public VarValuePair this[int index]
        {
            get
            {
                return data.GetOrDefault(index);
            }
        }

        public void SetData(string name, string value)
        {
            if (!varToIndex.ContainsKey(name))
            {
                if (name == null || name.Length < 0)
                {
                    Debug.LogError("Failed to set data because variable name was empty or null.");
                }
                else
                {
                    varToIndex.Add(name, data.Count);
                    data.Add(new VarValuePair() { name = name, value = value });
                }
            }
            else
            {
                int index = varToIndex[name];
                data[index] = new VarValuePair() {
                    name = data[index].name,
                    value = value
                };
            }
        }

        public string this[string name]
        {
            get
            {
                return GetData(name);
            }
            set
            {
                SetData(name, value);
            }
        }

        public string GetData(string name)
        {
            if (varToIndex.ContainsKey(name))
            {
                return data[varToIndex[name]].value;
            }
            return null;
        }

        public void Clear()
        {
            data.Clear();
            varToIndex.Clear();
        }

        public override string ToString()
        {
            string result = "";
            foreach (var pair in data)
            {
                result += pair.ToString() + "\n";
            }
            return result;
        }
    }
    #endregion
    #region form setup
    // ##################################
    // ------------ FORM SETUP ----------
    // ##################################

    [Serializable]
    public class Setup
    {
        public List<BaseElement> elements = new List<BaseElement>();
        public Data data = new Data();

        public Action<Data> onSubmit;
        public Action onReset;

        public T AddElement<T>(T element) where T : BaseElement
        {
            elements.Add(element);
            return element;
        }

        public T AddElement<T>(T element, string value) where T : BaseElement
        {
            elements.Add(element);
            if (element is Setups.BaseFieldSetup)
            {
                var variableName = (element as Setups.BaseFieldSetup).variableName;
                if (variableName != null && variableName.Length > 0) {
                    data[variableName] = value;
                }
            }
            return element;
        }

        public void AddData(string name, string value)
        {
            data[name] = value;
        }
    }
    #endregion
    #region helpers
    // ##################################
    // ------------ HELPERS -----------
    // ##################################

    public class SetupHelper
    {
        public Setup formSetup;
        public ElementsGroup fields;
        public ElementsGroup buttons;

        public ButtonElement AddResetButton(OGFormattedLocalizedText resetLabel, ButtonStyle style = ButtonStyle.Default)
        {
            return buttons.AddElement(new ButtonElement()
            {
                buttonType = ButtonType.Reset,
                style = style,
                labelText = resetLabel
            });
        }

        public ButtonElement AddSubmitButton(OGFormattedLocalizedText resetLabel, Action onPress, ButtonStyle style = ButtonStyle.Default)
        {
            return buttons.AddElement(new ButtonElement()
            {
                buttonType = ButtonType.Submit,
                style = style,
                labelText = resetLabel,
                onPress = () =>
                {
                    if (onPress != null) onPress.Invoke();
                }
            });
        }

        public ButtonElement AddCustomButton(OGFormattedLocalizedText label, Action onPress, ButtonStyle style = ButtonStyle.Default)
        {
            return buttons.AddElement(new ButtonElement()
            {
                buttonType = ButtonType.Custom,
                style = style,
                labelText = label,
                onPress = () =>
                {
                    if (onPress != null) onPress.Invoke();
                }
            });
        }

        public TextFieldElement AddTextField(OGFormattedLocalizedText label, string variableName, OGFormattedLocalizedText hint = null, int width = 350)
        {
            return fields.AddElement(new TextFieldElement()
            {
                labelText = label,
                hintText = hint,
                variableName = variableName,
                preferredWidth = width
            });
        }

        public DropdownElement AddDropdown<T>(OGFormattedLocalizedText label, string variableName, IList<T> items, Func<T, string> toValue, Func<T, string> toName, string defaultValue = null)
        {
            var dropdownElement = fields.AddElement(new DropdownElement()
            {
                labelText = label,
                variableName = variableName,
                defaultValue = defaultValue,
            });

            foreach (var item in items)
            {
                dropdownElement.AddElement(new DropdownOptionElement
                {
                    labelText = (toName(item)),
                    optionValue = toValue(item),
                });
            }

            return dropdownElement;
        }

        public RadioButtonsElement AddRadioButtons<T>(OGFormattedLocalizedText label, string variableName, IList<T> items, Func<T, string> toValue, Func<T, string> toName, string defaultValue = null)
        {
            var radioElement = fields.AddElement(new RadioButtonsElement()
            {
                labelText = label,
                variableName = variableName,
                defaultValue = defaultValue,
            });

            foreach (var item in items)
            {
                radioElement.AddElement(new RadioButtonOptionElement
                {
                    labelText = (toName(item)),
                    optionValue = toValue(item),
                });
            }

            return radioElement;
        }

        public CheckboxesElement AddCheckboxList<T>(OGFormattedLocalizedText label, string variableName, IList<T> items, Func<T, string> toName)
        {
            var radioElement = fields.AddElement(new CheckboxesElement()
            {
                labelText = label
            });

            var index = 0;
            foreach (var item in items)
            {
                radioElement.AddElement(new CheckboxOptionElement
                {
                    labelText = (toName(item)),
                    variableName = variableName + index
                });
                index++;
            }

            return radioElement;
        }

        public TextAreaElement AddTextArea(OGFormattedLocalizedText label, string variableName, OGFormattedLocalizedText hint = null, int height = 350, int width = 350)
        {
            return fields.AddElement(new TextAreaElement()
            {
                labelText = label,
                hintText = hint,
                variableName = variableName,
                preferredWidth = width,
                preferredHeight = height
            });
        }

        public RichTextAreaElement AddRichTextArea(OGFormattedLocalizedText label, string variableName, OGFormattedLocalizedText hint = null, int height = 350, int width = 350)
        {
            return fields.AddElement(new RichTextAreaElement()
            {
                labelText = label,
                hintText = hint,
                variableName = variableName,
                preferredWidth = width,
                preferredHeight = height
            });
        }

        public HeaderElement AddHeader(OGFormattedLocalizedText label)
        {
            return fields.AddElement(new HeaderElement()
            {
                labelText = label
            });
        }

        public PlainTextElement AddPlainText(OGFormattedLocalizedText text)
        {
            return fields.AddElement(new PlainTextElement()
            {
                labelText = text
            });
        }

        public TextFieldElement AddPasswordField(OGFormattedLocalizedText label, string variableName, int width = 350)
        {
            return fields.AddElement(new TextFieldElement()
            {
                labelText = label,
                hintText = OGLocalization.EmptyText,
                contentType = TMPro.TMP_InputField.ContentType.Password,
                variableName = variableName,
                preferredWidth = width
            });
        }

        public CheckboxOptionElement AddCheckbox(OGFormattedLocalizedText label, string variableName, bool defaultValue = false)
        {
            return fields.AddElement(new CheckboxOptionElement()
            {
                labelText = label,
                variableName = variableName,
                defaultValue = defaultValue.ToString()
            });
        }
    }

    public static SetupHelper GenerateBasicForm(OGFormattedLocalizedText title, OGFormattedLocalizedText description, OGFormattedLocalizedText submitText = null, Action<Data> onSubmit = null, Transform alternativeButtonsParent = null)
    {
        SetupHelper helper = new SetupHelper();
        helper.formSetup = new Setup();
        helper.formSetup.onSubmit = onSubmit;
        if (title != null)
        {
            helper.formSetup.AddElement(new TitleElement()
            {
                labelText = title
            });
        }
        if (description != null)
        {
            helper.formSetup.AddElement(new PlainTextElement()
            {
                labelText = description
            });
        }

        helper.fields = helper.formSetup.AddElement(new ElementsGroup() { direction = ElementsGroup.Direction.Vertical });
        
        helper.buttons = helper.formSetup.AddElement(new ElementsGroup() { direction = ElementsGroup.Direction.Vertical });
        helper.buttons.alternativeParent = alternativeButtonsParent;

        if (submitText != null)
        {
            helper.buttons.AddElement(new ButtonElement()
            {
                buttonType = ButtonType.Submit,
                labelText = submitText
            });
        }

        return helper;
    }

    #endregion

    // ##################################
    // ------------ BEHAVIOUR -----------
    // ##################################

    private static int updateFrame;
    private static OGForm activeForm;
    private static int selectedIndex;
    private static Selectable selectAtEndOfFrame;
    private static bool nextSelectedChanged = false;
    private static List<OGForm> builtForms = new List<OGForm>();

    public static void SetNextSelected(Selectable selectAtEndOfFrame)
    {
        nextSelectedChanged = true;
        OGForm.selectAtEndOfFrame = selectAtEndOfFrame;
    }

    private static void DoSelectNextObject()
    {
        if (!nextSelectedChanged) return;
        nextSelectedChanged = false;

        if (selectAtEndOfFrame != null) selectAtEndOfFrame.Select();
        EventSystem.current.SetSelectedGameObject(selectAtEndOfFrame == null ? null : selectAtEndOfFrame.gameObject);
    }

    public static OGForm GetActiveForm()
    {
        return activeForm;
    }

    public static OGForm IsFormActive()
    {
        return activeForm;
    }

    public static Selectable GetActiveSelectable()
    {
        DetectActiveForm();
        if (activeForm != null)
        {
            if (selectedIndex >= 0)
            {
                return activeForm.selectables[selectedIndex];
            }
        }
        return null;
    }

    public static bool IsInputActive()
    {
        var activeSelectable = GetActiveSelectable();
        if (activeSelectable != null)
        {
            if (activeSelectable is TMPro.TMP_InputField) return true;
            if (activeSelectable is InputField) return true;
        }
        return false;
    }

    private static void DetectActiveForm()
    {
        if (updateFrame != Time.frameCount)
        {
            updateFrame = Time.frameCount;
            activeForm = null;
            selectedIndex = 0;

            var currentActiveObject = EventSystem.current.currentSelectedGameObject;
            foreach (var form in builtForms)
            {
                var index = form.selectables.FindIndex(selectable => selectable.gameObject == currentActiveObject);
                if (index >= 0)
                {
                    selectedIndex = index;
                    activeForm = form;
                    break;
                }
            }
        }
    }

    private static bool IsFormActive(OGForm active)
    {
        DetectActiveForm();
        return activeForm == active;
    }

    private List<OGFormBaseElementBehaviour> elements = new List<OGFormBaseElementBehaviour>();
    private List<Selectable> selectables = new List<Selectable>();
    private Setup formSetup = new Setup();

    void Empty()
    {
        foreach (var e in elements)
        {
            e.Remove();
        }
        elements.Clear();
        selectables.Clear();
        builtForms.RemoveAll(form => form == this);
    }

    public void OnDestroy() { Empty(); }
    public void OnRemovedToPool() { Empty(); }
    public void OnPlacedFromPool() { Empty(); }
    public void OnCreatedForPool() { Empty(); }

    public void SetupForm(SetupHelper helper, bool autoSelect = true) { SetupForm(helper.formSetup, autoSelect); }
    public void SetupForm(Setup formSetup, bool autoSelect = true)
    {
        this.formSetup = formSetup;

        Empty();

        foreach (var elementSetup in formSetup.elements)
        {
            var baseSetup = elementSetup as Setups.BaseElementSetup;
            var prefab = OGFormManager.GetInstance().GetPrefabFromElementSetup(elementSetup);
            var parent = transform;

            if (baseSetup != null && baseSetup.alternativeParent != null)
                parent = baseSetup.alternativeParent;

            var copy = OGPool.placeCopy(prefab, parent);
            copy.Setup(this, elementSetup);
            copy.PutSelectablesInList(selectables);
            elements.Add(copy);
        }

        builtForms.Add(this);

        foreach (var element in elements)
        {
            element.ReadDataFrom(formSetup.data, true);
        }

        if (!selectables.IsNullOrEmpty() && autoSelect)
        {
            SetNextSelected(selectables[0]);
        }
    }

    public void SetValue(string variableName, string value)
    {
        Data data = new Data();
        data[variableName] = value;
        SetValues(data);
    }

    public void SetValues(Data data)
    {
        foreach (var e in elements)
        {
            e.ReadDataFrom(data, resetOtherwise: false);
        }
    }

    public void ResetForm()
    {
        foreach (var e in elements)
        {
            e.ReadDataFrom(formSetup.data, resetOtherwise: true);
        }
        if (formSetup.onReset != null) formSetup.onReset.Invoke();
    }

    public Data GetData()
    {
        Data data = new Data();
        foreach (var e in elements)
        {
            e.WriteDataTo(data);
        }
        return data;
    }

    public Data SubmitForm()
    {
        var data = GetData();
        if (formSetup != null) formSetup.onSubmit(data);
        EventSystem.current.SetSelectedGameObject(null);
        return data;
    }

    private void PressButton(Selectable selectable)
    {
        if (selectable is Button)
        {
            Button btn = (Button)selectable;
            if ((selectable as Button).onClick != null)
                (selectable as Button).onClick.Invoke();

            var meta = btn.GetComponent<OGFormMetaSettings>();

            if (meta == null || !meta.keepSelectedOnPress)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }

    // logic to cycle through selectables using the tab key
    // and submitting or pressing buttons
    void Update ()
    {
        if (selectables.Count > 0 && IsFormActive(this))
        {
            bool updateSelectable = false;

            if (OGInput.IsForwardCyclePressed())
            {
                updateSelectable = true;
                selectedIndex = (selectedIndex + 1) % selectables.Count;
            } else if (OGInput.IsBackwardCyclePressed())
            {
                updateSelectable = true;
                selectedIndex = (selectedIndex + selectables.Count - 1) % selectables.Count;
            }

            if (updateSelectable)
            {
                SetNextSelected(selectables[selectedIndex]);
            }

            var selectable = selectables.GetOrDefault(selectedIndex);
            if (selectable != null)
            {
                if (selectable is Button && OGInput.WantsToPressButton())
                {
                    PressButton(selectable);
                    return;
                }

                var preventSubmit = 
                    (selectable is TMP_InputField) 
                    && (selectable as TMP_InputField).multiLine 
                    && (selectable as TMP_InputField).lineType == TMP_InputField.LineType.MultiLineNewline;

                if ((OGInput.WantsToSubmit() && !preventSubmit)
                    || OGInput.WantsToReallySubmit())
                {
                    SubmitForm();
                    return;
                }
            }
        }
    }

    private void LateUpdate()
    {
        DoSelectNextObject();
    }
}
