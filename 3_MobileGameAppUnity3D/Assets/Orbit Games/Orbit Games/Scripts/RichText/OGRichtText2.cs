using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[System.Serializable]
public class OGRichText2InputState
{
    public int selectionAnchorIndex = -1;
    public int selectionFocusIndex = -1;
    public string text = "";

    public OGRichText2InputState() { }

    public OGRichText2InputState(string text, int selectionAnchorIndex, int selectionFocusIndex)
    {
        this.text = text;
        this.selectionAnchorIndex = selectionAnchorIndex;
        this.selectionFocusIndex = selectionFocusIndex;
    }

    public OGRichText2InputState(TMP_InputField fromField)
    {
        text = fromField.text;
        selectionAnchorIndex = fromField.selectionStringAnchorPosition;
        selectionFocusIndex = fromField.selectionStringFocusPosition;
    }

    public OGRichText2InputState(string text)
    {
        this.text = text;
        selectionAnchorIndex = text.Length - 1;
        selectionFocusIndex = text.Length - 1;
    }

    public void Apply(TMP_InputField toField)
    {
        toField.text = "";
        toField.text = text;
        toField.selectionStringAnchorPosition = selectionAnchorIndex;
        toField.selectionStringFocusPosition = selectionFocusIndex;
    }
}

public class OGRichtText2 : MonoBehaviour {

    private static bool NextCharsEquals(string fullText, string toMatch, int fromIndex)
    {
        if (fullText.Length < fromIndex + toMatch.Length) return false;
        for (int i = 0; i < toMatch.Length; i++)
        {
            if (toMatch[i] != fullText[fromIndex + i])
            {
                return false;
            }
        }
        return true;
    }

    private struct TagReadResult
    {
        public bool isFound { get; private set; }
        public ITagReader tag { get; private set; }
        public string value { get; private set; }
        public int tagCharacterCount { get; private set; }

        public TagReadResult(bool isFound, ITagReader tag)
        {
            this.isFound = isFound;
            this.tag = tag;
            value = null;
            tagCharacterCount = 0;
        }

        public TagReadResult(bool isFound, ITagReader tag, string value, int tagCharacterCount)
        {
            this.isFound = isFound;
            this.tag = tag;
            this.value = value;
            this.tagCharacterCount = tagCharacterCount;
        }
    }

    private class TagResolvingResult
    {
        public int selectionAnchorIndex = -1;
        public int selectionFocusIndex = -1;
        public string richText = "";

        public void WrapText(string openingTag, string closingTag)
        {
            var previousLength = richText.Length;

            richText = openingTag + richText + closingTag;

            if (selectionAnchorIndex == 0 && selectionFocusIndex == previousLength)
            {
                selectionFocusIndex = richText.Length;
            }
            else if (selectionFocusIndex == 0 && selectionAnchorIndex == previousLength)
            {
                selectionAnchorIndex = richText.Length;
            }
            else
            {
                if (selectionAnchorIndex >= 0) selectionAnchorIndex += openingTag.Length;
                if (selectionFocusIndex >= 0) selectionFocusIndex += openingTag.Length;
            }
        }

        public void AppendText(BaseRichTextComponent richTextComponent)
        {
            if (richTextComponent.SelectionAnchor >= 0)
            {
                selectionAnchorIndex = richTextComponent.SelectionAnchor + richText.Length;
            }
            if (richTextComponent.SelectionFocus >= 0)
            {
                selectionFocusIndex = richTextComponent.SelectionFocus + richText.Length;
            }
            richText += richTextComponent.ToRichText();
        }

        public void AppendResult(TagResolvingResult result)
        {
            if (result.selectionAnchorIndex >= 0)
            {
                selectionAnchorIndex = result.selectionAnchorIndex + richText.Length;
            }
            if (result.selectionFocusIndex >= 0)
            {
                selectionFocusIndex = result.selectionFocusIndex + richText.Length;
            }
            richText += result.richText;
        }
    }

    private interface ITagReader
    {
        string GetClosingTag(string value);
        string GetOpeningTag(string value);

        TagReadResult ReadTag(string richtText, int fromIndex);
    }

    private abstract class BooleanTagReader : ITagReader
    {
        public abstract string GetClosingTag(string value);
        public abstract string GetOpeningTag(string value);

        public TagReadResult ReadTag(string richtText, int fromIndex)
        {
            if (NextCharsEquals(richtText, GetOpeningTag(bool.TrueString), fromIndex))
            {
                return new TagReadResult(true, this, bool.TrueString, GetOpeningTag(bool.TrueString).Length);
            }
            else if (NextCharsEquals(richtText, GetClosingTag(bool.TrueString), fromIndex))
            {
                return new TagReadResult(true, this, null, GetClosingTag(bool.TrueString).Length);
            }
            return new TagReadResult(false, this);
        }
    }

    private class BoldReader : BooleanTagReader
    {
        public override string GetOpeningTag(string value) { return value == bool.TrueString ? "<b>" : ""; }
        public override string GetClosingTag(string value) { return value == bool.TrueString ? "</b>" : ""; }
    }

    private class ItalicReader : BooleanTagReader
    {
        public override string GetOpeningTag(string value) { return value == bool.TrueString ? "<i>" : ""; }
        public override string GetClosingTag(string value) { return value == bool.TrueString ? "</i>" : ""; }
    }

    private class UnderlineReader : BooleanTagReader
    {
        public override string GetOpeningTag(string value) { return value == bool.TrueString ? "<u>" : ""; }
        public override string GetClosingTag(string value) { return value == bool.TrueString ? "</u>" : ""; }
    }

    private static List<ITagReader> tagReaders = new List<ITagReader>();

    private static void Initialize()
    {
        if (tagReaders.Count == 0)
        {
            tagReaders.Add(new BoldReader());
            tagReaders.Add(new ItalicReader());
            tagReaders.Add(new UnderlineReader());
        }
    }

    private static ITagReader GetTagReader(OGRichTextBooleanTag forTag)
    {
        switch (forTag)
        {
            case OGRichTextBooleanTag.BOLD:
                return tagReaders[0];
            case OGRichTextBooleanTag.ITALIC:
                return tagReaders[1];
            case OGRichTextBooleanTag.UNDERLINE:
                return tagReaders[2];
            default:
                return null;
        }
    }

    private abstract class BaseRichTextComponent
    {
        public abstract int RichTextLength { get; }
        public abstract int PlainTextLength { get; }

        public abstract int SelectionAnchor { get; protected set; }
        public abstract int SelectionFocus { get; protected set; }

        public abstract string ToPlainText();
        public abstract string ToRichText();
    }

    private class PlainText : BaseRichTextComponent
    {
        public override int SelectionAnchor { get; protected set; }
        public override int SelectionFocus { get; protected set; }

        public override int RichTextLength { get { return PlainTextLength; } }
        public override int PlainTextLength { get { return plainText.Length; } }

        private string plainText = "";

        public PlainText()
        {
            SelectionAnchor = -1;
            SelectionFocus = -1;
        }

        public override string ToPlainText()
        {
            return plainText;
        }

        public override string ToRichText()
        {
            return plainText;
        }
    }

    private abstract class BaseEncapsulatingTag: BaseRichTextComponent
    {
        public override int SelectionAnchor { get; protected set; }
        public override int SelectionFocus { get; protected set; }

        public override int RichTextLength { get { return prefix.Length + content.Sum(c => c.RichTextLength) + suffix.Length; } }
        public override int PlainTextLength { get { return content.Sum(c => c.PlainTextLength); } }

        public BaseEncapsulatingTag()
        {
            SelectionAnchor = -1;
            SelectionFocus = -1;
        }

        public override string ToPlainText()
        {
            return string.Join("", content.Select(i => i.ToPlainText()).ToArray());
        }

        public override string ToRichText()
        {
            return prefix + string.Join("", content.Select(i => i.ToRichText()).ToArray()) + suffix;
        }

        private string prefix = "";
        protected List<BaseRichTextComponent> content = new List<BaseRichTextComponent>();
        private string suffix = "";

        protected abstract string GetPrefix(string value);
        protected abstract string GetSuffix(string value);
    }
}
