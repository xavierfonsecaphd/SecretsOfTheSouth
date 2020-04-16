using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class OGRichTextInputState
{
    public int selectionAnchorIndex = -1;
    public int selectionFocusIndex = -1;
    public string text = "";

    public OGRichTextInputState() { }

    public OGRichTextInputState(string text, int selectionAnchorIndex, int selectionFocusIndex)
    {
        this.text = text;
        this.selectionAnchorIndex = selectionAnchorIndex;
        this.selectionFocusIndex = selectionFocusIndex;
    }

    public OGRichTextInputState(TMP_InputField fromField)
    {
        text = fromField.text;
        selectionAnchorIndex = fromField.selectionStringAnchorPosition;
        selectionFocusIndex = fromField.selectionStringFocusPosition;
    }

    public OGRichTextInputState(string text)
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

public enum OGRichTextBooleanTag
{
    BOLD, ITALIC, UNDERLINE
}

public class OGRichText
{

    private class FormattedTextPiece
    {
        private Dictionary<ITagHandler, string> tagValues = new Dictionary<ITagHandler, string>();
        private List<ITagHandler> tagStack = new List<ITagHandler>();
        public int originalTextOffset = 0;
        public int localSelectionAnchorIndex = -1;
        public int localSelectionFocusIndex = -1;
        public string textPiece = "";

        public ITagHandler this[int index] { get { return tagStack[index]; } }
        public string this[ITagHandler tag] { get { return tagValues[tag]; } }
        public int Length { get { return textPiece.Length; } }
        public int OffsettedLength { get { return textPiece.Length + originalTextOffset; } }
        public int Offset { get { return originalTextOffset; } }
        public char LastCharacter { get { return (textPiece.Length > 0) ? textPiece[textPiece.Length - 1] : default(char); } }
        public char FirstCharacter { get { return (textPiece.Length > 0) ? textPiece[0] : default(char); } }

        public bool IsWhiteSpace()
        {
            return System.Text.RegularExpressions.Regex.IsMatch(textPiece, @"^\s+$");
        }

        public bool IsEmpty()
        {
            return textPiece.Length == 0;
        }

        public void SetTagValue(TagReadResult tagReadResult)
        {
            SetTagValue(tagReadResult.tag, tagReadResult.value);
        }

        public void SetTagValue(ITagHandler tag, string value)
        {
            if (value == null)
            {
                tagValues.RemoveIfContainsKey(tag);
                tagStack.RemoveAll(t => t == tag);
            }
            else
            {
                if (tagValues.ContainsKey(tag))
                {
                    tagValues[tag] = value;
                    int stateIndex = tagStack.FindIndex(t => t == tag);
                    tagStack[stateIndex] = tag;
                }
                else
                {
                    tagValues.Add(tag, value);
                    tagStack.Add(tag);
                }
            }
        }
        
        public void ClearSelection()
        {
            localSelectionAnchorIndex = -1;
            localSelectionFocusIndex = -1;
        }

        public void SetSelection(int localOffsettedSelectionAnchorIndex, int localOffsettedSelectionFocusIndex, SelectionStatus selectionStatus)
        {
            bool anchorInside = localOffsettedSelectionAnchorIndex >= 0 && localOffsettedSelectionAnchorIndex <= OffsettedLength;
            bool focusInside = localOffsettedSelectionFocusIndex >= 0 && localOffsettedSelectionFocusIndex <= OffsettedLength;

            var localSelectionAnchorIndex = anchorInside ? Mathf.Max(localOffsettedSelectionAnchorIndex - Offset, 0) : localOffsettedSelectionAnchorIndex - Offset;
            var localSelectionFocusIndex = focusInside ? Mathf.Max(localOffsettedSelectionFocusIndex - Offset, 0) : localOffsettedSelectionFocusIndex - Offset;

            if (anchorInside && focusInside)
            {
                if (!selectionStatus.anchorFound)
                {
                    selectionStatus.anchorFound = true;
                    this.localSelectionAnchorIndex = localSelectionAnchorIndex;
                }

                if (!selectionStatus.focusFound)
                {
                    selectionStatus.focusFound = true;
                    this.localSelectionFocusIndex = localSelectionFocusIndex;
                }

                return;
            }
            else if (!anchorInside && !focusInside)
            {
                return;
            }

            //bool anchorAfterFocus = localSelectionAnchorIndex > localSelectionFocusIndex;
            //bool focusAfterAnchor = localSelectionFocusIndex > localSelectionAnchorIndex;

            //bool anchorOnLeftEdge = localSelectionAnchorIndex == 0;
            //bool anchorOnRightEdge = localSelectionAnchorIndex == Length;

            //bool focusOnLeftEdge = localSelectionFocusIndex == 0;
            //bool focusOnRightEdge = localSelectionFocusIndex == Length;

            // handle the cases where selection starts from an edge but doesn't include this text
            // if (anchorOnRightEdge && focusAfterAnchor) return;
            // if (focusOnRightEdge && anchorAfterFocus) return;

            // skipping, because the reselect method loops through textpieces and doesnt go back to select the previous item
            // if (anchorOnLeftEdge && anchorAfterFocus) return;
            // if (focusOnLeftEdge && focusAfterAnchor) return;

            if (anchorInside)
            {
                if (!selectionStatus.anchorFound)
                {
                    selectionStatus.anchorFound = true;
                    this.localSelectionAnchorIndex = localSelectionAnchorIndex;
                }

                return;
            }
            else if (focusInside)
            {
                if (!selectionStatus.focusFound)
                {
                    selectionStatus.focusFound = true;
                    this.localSelectionFocusIndex = localSelectionFocusIndex;
                }

                return;
            }
            return;
        }

        public int TotalTagDepth {  get { return tagStack.Count; } }

        public bool HasTagWithValue(TagReadResult tagReadResult)
        {
            return IsTagAtState(tagReadResult.tag, tagReadResult.value);
        }

        public bool IsTagAtState(ITagHandler tag, string value)
        {
            //if (otherTagState.handler == null) return false;
            if (tagValues.ContainsKey(tag))
            {
                return value != null && value.Equals(tagValues[tag]);
            }
            else
            {
                return value == null;
            }
        }

        public FormattedTextPiece SplitAndInvalidateSelectionAndReturnCreatedTail(int splitIndex, bool offsettedIndex = true)
        {
            if (offsettedIndex)
            {
                splitIndex = Mathf.Clamp(splitIndex - Offset, 0, Length);
            }

            var clone = CloneFormatting();
            clone.textPiece = textPiece.Substring(splitIndex);
            textPiece = textPiece.Substring(0, splitIndex);

            localSelectionAnchorIndex = -1;
            localSelectionFocusIndex = -1;

            return clone;
        }

        public FormattedTextPiece CloneFormatting()
        {
            var result = new FormattedTextPiece();
            result.tagStack.AddRange(tagStack);
            result.tagValues.AddRangeUnique(tagValues);
            return result;
        }
    }

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
        public ITagHandler tag { get; private set; }
        public string value { get; private set; }
        public int tagCharacterCount { get; private set; }

        public TagReadResult(bool isFound, ITagHandler tag)
        {
            this.isFound = isFound;
            this.tag = tag;
            value = null;
            tagCharacterCount = 0;
        }

        public TagReadResult(bool isFound, ITagHandler tag, string value, int tagCharacterCount)
        {
            this.isFound = isFound;
            this.tag = tag;
            this.value = value;
            this.tagCharacterCount = tagCharacterCount;
        }
    }

    private interface ITagHandler
    {
        TagReadResult ReadTag(string richtText, int fromIndex);
        void WriteTag(TagResolvingResult result, string tagValue);
    }

    private abstract class BooleanTagHandler : ITagHandler
    {
        public abstract string OpeningTag { get; }
        public abstract string ClosingTag { get; }

        public TagReadResult ReadTag(string richtText, int fromIndex)
        {
            if (NextCharsEquals(richtText, OpeningTag, fromIndex))
            {
                return new TagReadResult(true, this, bool.TrueString, OpeningTag.Length);
            }
            else if (NextCharsEquals(richtText, ClosingTag, fromIndex))
            {
                return new TagReadResult(true, this, null, ClosingTag.Length);
            }
            return new TagReadResult(false, this);
        }

        public void WriteTag(TagResolvingResult result, string tagValue)
        {
            if (tagValue == bool.TrueString)
            {
                result.WrapText(OpeningTag, ClosingTag);
            }
        }
    }

    private class BoldHandler : BooleanTagHandler
    {
        public override string OpeningTag { get { return "<b>"; } }
        public override string ClosingTag { get { return "</b>"; } }
    }

    private class ItalicHandler : BooleanTagHandler
    {
        public override string OpeningTag{ get { return "<i>"; } }
        public override string ClosingTag { get { return "</i>"; } }
    }

    private class UnderlineHandler : BooleanTagHandler
    {
        public override string OpeningTag { get { return "<u>"; } }
        public override string ClosingTag { get { return "</u>"; } }
    }

    private static List<ITagHandler> tagHandlers = new List<ITagHandler>();
    private static CharacterWalker characterWalker;

    private static void Initialize()
    {
        if (tagHandlers.Count == 0)
        {
            characterWalker = new CharacterWalker();

            tagHandlers.Add(new BoldHandler());
            tagHandlers.Add(new ItalicHandler());
            tagHandlers.Add(new UnderlineHandler());
        }
    }

    private static ITagHandler GetTagHandler(OGRichTextBooleanTag forTag)
    {
        switch (forTag)
        {
            case OGRichTextBooleanTag.BOLD:
                return tagHandlers[0];
            case OGRichTextBooleanTag.ITALIC:
                return tagHandlers[1];
            case OGRichTextBooleanTag.UNDERLINE:
                return tagHandlers[2];
            default:
                return null;
        }
    }

    private List<FormattedTextPiece> textPieces = new List<FormattedTextPiece>();
    private int selectionAnchor = -1;
    private int selectionFocus = -1;
    private string originalText;


    public OGRichText(string richText) : this(richText, -1, -1) { }

    public OGRichText(TMP_InputField inputField) 
        : this(inputField.text, inputField.selectionStringAnchorPosition, inputField.selectionStringFocusPosition) { }

    public OGRichText(string richText, int selectionAnchor, int selectionFocus)
    {
        Initialize();

        FormattedTextPiece current = new FormattedTextPiece();
        textPieces.Add(current);

        originalText = richText;

        int currentIndex = 0;
        while (currentIndex < richText.Length)
        {
            // check if we may find a tag coming up
            if (richText[currentIndex] == '<')
            {
                TagReadResult tagReadResult = default(TagReadResult);

                // check if we match a tag
                for (int i = 0; i < tagHandlers.Count; i++)
                {
                    tagReadResult = tagHandlers[i].ReadTag(richText, currentIndex);
                    if (tagReadResult.isFound) break;
                }

                // if we matched a tag, do some work
                if (tagReadResult.isFound)
                {
                    // if we already have some text before the currently read tag, create a new fresh piece of text
                    if (current.textPiece.Length > 0)
                    {
                        current = current.CloneFormatting();
                        textPieces.Add(current);
                    }

                    // set tag value to piece
                    current.SetTagValue(tagReadResult);

                    // we are guaranteed to have no text yet in the current piece, and we read a tag, so let's offset this new piece
                    current.originalTextOffset += tagReadResult.tagCharacterCount;

                    // skip reading a single character and go to character after tag
                    currentIndex += tagReadResult.tagCharacterCount;
                    continue;
                }
            }

            // process the new character
            current.textPiece += richText[currentIndex];
            currentIndex++;
        }

        Select(selectionAnchor, selectionFocus);
    }

    private void ClearSelection()
    {
        for (int i = 0; i < textPieces.Count; i++)
        {
            textPieces[i].ClearSelection();
        }
    }

    private void Reselect()
    {
        Select(selectionAnchor, selectionFocus);
    }

    private void RedetermineSelection()
    {
        Select(selectionAnchor, selectionFocus);
    }

    private class SelectionStatus
    {
        public bool anchorFound = false;
        public bool focusFound = false;
    }

    public void Select(int selectionAnchor, int selectionFocus)
    {
        ClearSelection();

        this.selectionAnchor = selectionAnchor;
        this.selectionFocus = selectionFocus;

        var selectionStatus = new SelectionStatus();

        for (int i = 0; i < textPieces.Count; i++)
        {
            textPieces[i].SetSelection(selectionAnchor, selectionFocus, selectionStatus);
            selectionAnchor -= textPieces[i].OffsettedLength;
            selectionFocus -= textPieces[i].OffsettedLength;
        }
    }

    private class CharacterWalker
    {
        private List<FormattedTextPiece> textPieces;
        public int textPieceIndex { get; private set; }
        public int characterIndex { get; private set; }
        public int localCharacterIndex { get; private set; }
        public string character { get; private set; }

        public string Initialize(List<FormattedTextPiece> textPieces, int offsettedCharacterIndex)
        {
            this.textPieces = textPieces;
            character = DetermineCharacter(offsettedCharacterIndex, isOffsettedIndex: true);
            return character;
        }

        public string MoveCharacterIndex(int count = 1)
        {
            character = DetermineCharacter(characterIndex + count, isOffsettedIndex: false);
            return character;
        }

        private string DetermineCharacter(int offsettedCharacterIndex, bool isOffsettedIndex)
        {
            characterIndex = offsettedCharacterIndex;
            localCharacterIndex = offsettedCharacterIndex;
            var totalLength = 0;
            for (textPieceIndex = 0; textPieceIndex < textPieces.Count; textPieceIndex++)
            {
                var textPiece = textPieces[textPieceIndex];

                if (isOffsettedIndex)
                {
                    // remove only offsets, becuase we want to find the text-only non-offsetted character index of the entire richtext
                    characterIndex -= textPiece.Offset;
                    // remove offset and later also textpiece length, we want the local place for search purposes
                    localCharacterIndex -= textPiece.Offset;
                }

                if (!textPiece.IsEmpty() && localCharacterIndex < textPiece.Length)
                {
                    if (localCharacterIndex >= 0)
                    {
                        return textPiece.textPiece[localCharacterIndex].ToString();
                    }
                    else
                    {
                        characterIndex = totalLength;
                        localCharacterIndex = 0;
                        return textPiece.textPiece[0].ToString();
                    }
                }

                localCharacterIndex -= textPiece.Length;
                totalLength += textPiece.Length;
            }
            return null;
        }
    }

    private int SmartSplitAtCharacter(int offsettedCharacterIndex, bool getTail)
    {
        if (textPieces.Count == 0) return -1;
        if (textPieces.Count == 1 && textPieces[0].IsEmpty()) return 0;

        // determine the offset at the end of the richtext string
        var lastNonEmptyTextPieceIndex = textPieces.Count - 1;
        var lastNonEmptyTextPiece = textPieces[lastNonEmptyTextPieceIndex];
        var totalEndOffset = 0;
        while (lastNonEmptyTextPiece.IsEmpty())
        {
            totalEndOffset += lastNonEmptyTextPiece.Offset;
            lastNonEmptyTextPieceIndex--;
            lastNonEmptyTextPiece = textPieces[lastNonEmptyTextPieceIndex];
        }

        // if the given offsetted character index is at or larger than the available text, then just split at the end
        if (offsettedCharacterIndex >= originalText.Length - totalEndOffset)
        {
            var splitResult = lastNonEmptyTextPiece.SplitAndInvalidateSelectionAndReturnCreatedTail(lastNonEmptyTextPiece.Length, offsettedIndex: false);
            textPieces.Insert(lastNonEmptyTextPieceIndex + 1, splitResult);
            return lastNonEmptyTextPieceIndex + (getTail ? 1 : 0); // return next item if we wanted the tail of the split
        }
        else
        {
            var thisCharacter = characterWalker.Initialize(textPieces, offsettedCharacterIndex);
            var thisTextPieceIndex = characterWalker.textPieceIndex;
            var thisLocalCharacterIndex = characterWalker.localCharacterIndex;

            var previousCharacter = characterWalker.MoveCharacterIndex(-1);
            var previousTextPieceIndex = characterWalker.textPieceIndex;
            var previousLocalCharacterIndex = characterWalker.localCharacterIndex;

            if (thisCharacter == null)
            {
                Debug.LogError("Could not find the character to split on");
                return -1;
            }

            var splitLocalCharacterIndex = thisLocalCharacterIndex;
            var splitTextPieceIndex = thisTextPieceIndex;
            if (thisCharacter.IsWhiteSpace() && !previousCharacter.IsNullOrEmpty() && !previousCharacter.IsWhiteSpace())
            {
                splitTextPieceIndex = previousTextPieceIndex;
                splitLocalCharacterIndex = previousLocalCharacterIndex + 1; // + 1 because we do still want to split at the offsetted character index, which is supposed to be after the currenmt character
            }

            var textPiece = textPieces[splitTextPieceIndex];
            var splitResult = textPiece.SplitAndInvalidateSelectionAndReturnCreatedTail(splitLocalCharacterIndex, offsettedIndex: false);
            textPieces.Insert(splitTextPieceIndex + 1, splitResult);

            return splitTextPieceIndex + (getTail ? 1 : 0); // return next item if we wanted the tail of the split
        }
    }

    private Tuple<int, int> SplitAtRangeEdgesAndReturnResultingTextPieceRange(int fromOriginalIndex, int toOriginalIndex)
    {
        if (toOriginalIndex < fromOriginalIndex) OGUtils.Swap(ref fromOriginalIndex, ref toOriginalIndex);

        var firstSplit = SmartSplitAtCharacter(fromOriginalIndex, getTail: true);
        var nextSplit = SmartSplitAtCharacter(toOriginalIndex, getTail: false);

        Reselect();

        // return the resulting range
        return Tuple.Create(firstSplit, nextSplit);
    }


    private int GetTextPiecesLength(int fromIndex, int toIndex)
    {
        var total = 0;
        for (int currentIndex = fromIndex; currentIndex <= toIndex; currentIndex++)
        {
            total += textPieces[currentIndex].OffsettedLength;
        }
        return total;
    }
    

    public string ExportSelectionToRichText()
    {
        return ExportRangeToRichText(selectionAnchor, selectionFocus);
    }

    public string ExportRangeToRichText(int fromOriginalIndex, int toOriginalIndex)
    {
        var range = SplitAtRangeEdgesAndReturnResultingTextPieceRange(fromOriginalIndex, toOriginalIndex);
        return ExportTextPiecesToRichTextAndSelection(range.Item1, range.Item2 - range.Item1 + 1).text;
    }



    public void RemoveSelection()
    {
        RemoveRange(selectionAnchor, selectionFocus);
    }

    public void RemoveRange(int fromOriginalIndex, int toOriginalIndex)
    {
        var range = SplitAtRangeEdgesAndReturnResultingTextPieceRange(fromOriginalIndex, toOriginalIndex);
        RemoveTextPieces(range);
    }

    private void RemoveTextPieces(Tuple<int, int> textPiecesRange)
    {
        if (textPiecesRange == null) return;
        
        var at = textPiecesRange.Item1;
        var to = textPiecesRange.Item2;
        var count = to - at + 1;

        var beforeLength = GetTextPiecesLength(0, at - 1);
        var replacedLength = GetTextPiecesLength(at, to);

        textPieces.RemoveRange(at, count);

        HandleReplacementSelection(beforeLength, replacedLength, 0);
    }



    public void ReplaceSelection(string newText)
    {
        ReplaceRange(newText, selectionAnchor, selectionFocus);
    }

    public void ReplaceRange(string newText, int fromOriginalIndex, int toOriginalIndex)
    {
        var range = SplitAtRangeEdgesAndReturnResultingTextPieceRange(fromOriginalIndex, toOriginalIndex);
        ReplaceTextPieces(newText, range);
    }

    private void ReplaceTextPieces(string newText, Tuple<int, int> textPiecesRange)
    {
        if (textPiecesRange == null) return;

        var tempRichtText = new OGRichText(newText);

        var at = textPiecesRange.Item1;
        var to = textPiecesRange.Item2;
        var count = to - at + 1;

        var beforeLength = GetTextPiecesLength(0, at - 1);
        var replacedLength = GetTextPiecesLength(at, to);

        textPieces.RemoveRange(at, count);
        textPieces.InsertRange(at, tempRichtText.textPieces);

        HandleReplacementSelection(beforeLength, replacedLength, newText.Length);

    }

    private void HandleReplacementSelection(int beforeLength, int replacedLength, int newLength)
    {
        //var newSelectionAnchor = selectionAnchor;
        //var newSelectionFocus = selectionFocus;
        //var reversedSelection = newSelectionAnchor > newSelectionFocus;

        
        Select(beforeLength + newLength, beforeLength + newLength);
        return;

        //// reverse if needed to guarantee non-reversed order of selection points
        //if (reversedSelection) OGUtils.Swap(ref newSelectionAnchor, ref newSelectionFocus);

        //// do logic for guaranteed non-reversed order of selection points
        //if (newSelectionAnchor >= beforeLength && newSelectionAnchor <= beforeLength + replacedLength)
        //{
        //    newSelectionAnchor = beforeLength + newLength;
        //}

        //if (newSelectionFocus >= beforeLength && newSelectionFocus <= beforeLength + replacedLength)
        //{
        //    newSelectionFocus = beforeLength + newLength;
        //}

        //// reverse back
        //if (reversedSelection) OGUtils.Swap(ref newSelectionAnchor, ref newSelectionFocus);

        //Select(newSelectionAnchor, newSelectionFocus);
    }



    public bool ToggleTagOnSelection(OGRichTextBooleanTag tag)
    {
        return ToggleTagOnRange(tag, selectionAnchor, selectionFocus);
    }

    public bool ToggleTagOnRange(OGRichTextBooleanTag tag, int fromOriginalIndex, int toOriginalIndex)
    {
        var range = SplitAtRangeEdgesAndReturnResultingTextPieceRange(fromOriginalIndex, toOriginalIndex);
        return ToggleTagOnTextPieces(tag, range);
    }

    private bool ToggleTagOnTextPieces(OGRichTextBooleanTag tag, Tuple<int, int> textPiecesRange)
    {
        var enabled = IsTagFullyAtStateInTextPieces(tag, true, textPiecesRange);
        SetTagToTextPieces(tag, !enabled, textPiecesRange);
        return !enabled;
    }



    public bool IsTagFullyAppliedToSelection(OGRichTextBooleanTag tag)
    {
        return IsTagFullyAppliedToRange(tag, selectionAnchor, selectionFocus);
    }

    public bool IsTagFullyAppliedToRange(OGRichTextBooleanTag tag, int fromOriginalIndex, int toOriginalIndex)
    {
        var range = SplitAtRangeEdgesAndReturnResultingTextPieceRange(fromOriginalIndex, toOriginalIndex);
        return IsTagFullyAtStateInTextPieces(tag, true, range);
    }

    private bool IsTagFullyAtStateInTextPieces(OGRichTextBooleanTag tag, bool state, Tuple<int, int> textPiecesRange)
    {
        if (textPiecesRange == null) return false;

        // get total text length of range
        var totalLength = 0;
        for (int currentIndex = textPiecesRange.Item1; currentIndex <= textPiecesRange.Item2; currentIndex++)
        {
            totalLength += textPieces[currentIndex].Length;
        }

        // get the handler
        var tagHandler = GetTagHandler(tag);

        // loop and return false if a textpiece was not found to be at the given state
        for (int currentIndex = textPiecesRange.Item1; currentIndex <= textPiecesRange.Item2; currentIndex++)
        {
            var textPiece = textPieces[currentIndex];
            // skip empty parts if we are not an empty range
            if (totalLength != 0 && textPiece.IsEmpty() || textPiece.IsWhiteSpace()) continue;

            // check state, return if textpiece is not at given state
            if (state != textPiece.IsTagAtState(tagHandler, bool.TrueString))
            {
                return false;
            }
        }
        return true;
    }



    public bool IsTagPartiallyAppliedToSelection(OGRichTextBooleanTag tag)
    {
        return IsTagPartiallyAppliedToRange(tag, selectionAnchor, selectionFocus);
    }

    public bool IsTagPartiallyAppliedToRange(OGRichTextBooleanTag tag, int fromOriginalIndex, int toOriginalIndex)
    {
        var range = SplitAtRangeEdgesAndReturnResultingTextPieceRange(fromOriginalIndex, toOriginalIndex);
        return IsTagPartiallyAppliedToTextPieces(tag, range);
    }

    private bool IsTagPartiallyAppliedToTextPieces(OGRichTextBooleanTag tag, Tuple<int, int> textPiecesRange)
    {
        return !IsTagFullyAtStateInTextPieces(tag, false, textPiecesRange);
    }



    public void SetTagToSelection(OGRichTextBooleanTag tag, bool enabled)
    {
        SetTagToRange(tag, enabled, selectionAnchor, selectionFocus);
    }

    public void SetTagToRange(OGRichTextBooleanTag tag, bool enabled, int fromOriginalIndex, int toOriginalIndex)
    {
        var range = SplitAtRangeEdgesAndReturnResultingTextPieceRange(fromOriginalIndex, toOriginalIndex);
        SetTagToTextPieces(tag, enabled, range);
    }

    private void SetTagToTextPieces(OGRichTextBooleanTag tag, bool enabled, Tuple<int, int> textPiecesRange)
    {
        if (textPiecesRange == null) return;
        var tagHandler = GetTagHandler(tag);
        for (int currentIndex = textPiecesRange.Item1; currentIndex <= textPiecesRange.Item2; currentIndex++)
        {
            textPieces[currentIndex].SetTagValue(tagHandler, enabled ? bool.TrueString : null);
        }
    }


    public void ClearFormatting()
    {
        ClearFormattingToTextPieces(Tuple.Create(0, textPieces.Count - 1));
    }

    public void ClearFormattingToSelection()
    {
        ClearFormattingToRange(selectionAnchor, selectionFocus);
    }

    public void ClearFormattingToRange(int fromOriginalIndex, int toOriginalIndex)
    {
        var range = SplitAtRangeEdgesAndReturnResultingTextPieceRange(fromOriginalIndex, toOriginalIndex);
        ClearFormattingToTextPieces(range);
    }

    private void ClearFormattingToTextPieces(Tuple<int, int> textPiecesRange)
    {
        if (textPiecesRange == null) return;
        SetTagToTextPieces(OGRichTextBooleanTag.BOLD, false, textPiecesRange);
        SetTagToTextPieces(OGRichTextBooleanTag.ITALIC, false, textPiecesRange);
        SetTagToTextPieces(OGRichTextBooleanTag.UNDERLINE, false, textPiecesRange);
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

        public void AppendText(FormattedTextPiece formattedTextPiece)
        {
            if (formattedTextPiece.localSelectionAnchorIndex >= 0)
            {
                selectionAnchorIndex = formattedTextPiece.localSelectionAnchorIndex + richText.Length;
            }
            if (formattedTextPiece.localSelectionFocusIndex >= 0)
            {
                selectionFocusIndex = formattedTextPiece.localSelectionFocusIndex + richText.Length;
            }
            richText += formattedTextPiece.textPiece;
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

    private class TagResolvingGroupHelper
    {
        public List<FormattedTextPiece> allTextPieces;
        public ITagHandler currentGroupTag;
        public string currentGroupTagValue;
        public int currentGroupStartIndex;
        public int currentGroupDepth;
        public bool isGrouping;

        public TagResolvingGroupHelper(List<FormattedTextPiece> allTextPieces, int currentGroupDepth)
        {
            this.allTextPieces = allTextPieces;
            this.currentGroupDepth = currentGroupDepth;
            currentGroupTag = null;
            currentGroupTagValue = null;
            currentGroupStartIndex = int.MaxValue;
            isGrouping = false;
        }

        public void ResolveActiveGroup(TagResolvingResult result, int atIndex)
        {
            if (atIndex > currentGroupStartIndex && isGrouping)
            {
                ResolveTagDepthRecursively(result, allTextPieces, currentGroupTag, currentGroupTagValue, currentGroupDepth + 1, currentGroupStartIndex, atIndex - currentGroupStartIndex);
            }

            currentGroupTag = null;
            currentGroupTagValue = null;
            currentGroupStartIndex = int.MaxValue;
            isGrouping = false;
        }

        public bool IsNewGroup(ITagHandler tagHandler, string tagValue)
        {
            return !isGrouping || tagHandler != currentGroupTag || !tagValue.Equals(currentGroupTagValue);
        }

        public void StartGroup(ITagHandler tagHandler, string tagValue, int atIndex)
        {
            currentGroupTag = tagHandler;
            currentGroupTagValue = tagValue;
            currentGroupStartIndex = atIndex;
            isGrouping = true;
        }
    }

    private static void ResolveTagDepthRecursively(TagResolvingResult recursiveResult, List<FormattedTextPiece> textPieces, ITagHandler wrappingTagHandler, string wrappingTagValue, int depth, int fromIndex, int count)
    {
        if (count == 0) return;

        // initialize result
        var iterationResult = new TagResolvingResult();
        var endIndex = fromIndex + count;
        var groupHelper = new TagResolvingGroupHelper(textPieces, depth);

        // go over each textpiece in the current section of the list
        for (int currentIndex = fromIndex; currentIndex < endIndex; currentIndex++)
        {
            var formattedTextPiece = textPieces[currentIndex];

            // if we are at a depth where there is no tag anymore, just return the raw text
            // but any active group should be finalized
            if (formattedTextPiece.TotalTagDepth == depth)
            {
                groupHelper.ResolveActiveGroup(iterationResult, currentIndex);
                iterationResult.AppendText(formattedTextPiece);
            }
            // this should never be possible, depth should always be <= TotalTagDepth
            else if(formattedTextPiece.TotalTagDepth < depth)
            {
                // skip, should not event happen really
                Debug.LogError("Algorithm seems to have mistake, should not get to this point");
            }
            else
            {
                // curent group
                var currentTag = formattedTextPiece[depth];
                var currentTagValue = formattedTextPiece[currentTag];

                // new grouping?
                if (groupHelper.IsNewGroup(currentTag, currentTagValue))
                {
                    groupHelper.ResolveActiveGroup(iterationResult, currentIndex);
                    groupHelper.StartGroup(currentTag, currentTagValue, currentIndex);
                }
                else
                {
                    // we are looping over the current group, so just move on to the next one
                }
            }
        }

        // finalize the last grouping, if there was any
        groupHelper.ResolveActiveGroup(iterationResult, endIndex);

        // write the wrapping tag
        if (wrappingTagHandler != null && wrappingTagValue != null)
        {
            wrappingTagHandler.WriteTag(iterationResult, wrappingTagValue);
        }

        // add to the iteration result
        recursiveResult.AppendResult(iterationResult);
    }

    private OGRichTextInputState ExportTextPiecesToRichTextAndSelection(int fromIndex, int count)
    {
        var result = new TagResolvingResult();
        ResolveTagDepthRecursively(result, textPieces, null, null, 0, fromIndex, count);
        return new OGRichTextInputState()
        {
            text = result.richText,
            selectionAnchorIndex = result.selectionAnchorIndex,
            selectionFocusIndex = result.selectionFocusIndex
        };
    }

    private OGRichTextInputState ExportTextPiecesToTextAndSelection(int fromIndex, int toIndex)
    {
        var result = new OGRichTextInputState();

        for (int currentIndex = fromIndex; currentIndex <= toIndex; currentIndex++)
        {
            var textPiece = textPieces[currentIndex];

            if (textPiece.localSelectionAnchorIndex >= 0)
                result.selectionAnchorIndex = result.text.Length + textPiece.localSelectionAnchorIndex;

            if (textPiece.localSelectionFocusIndex >= 0)
                result.selectionFocusIndex = result.text.Length + textPiece.localSelectionFocusIndex;

            result.text += textPiece.textPiece;
        }
        
        return result;
    }

    public OGRichTextInputState ExportToRichText()
    {
        return ExportTextPiecesToRichTextAndSelection(0, textPieces.Count);
    }
}