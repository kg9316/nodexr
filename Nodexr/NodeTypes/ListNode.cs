﻿namespace Nodexr.NodeTypes;
using Nodexr.Utils;
using Nodexr.Core;
using Nodexr.NodeInputs;
using Nodexr.Nodes;

public class ListNode : RegexNodeViewModelBase
{
    public override string Title => "List";

    public override string NodeInfo => "Matches a list of similar items, with a separator between each one." +
        "\n\nWarning: this node is marked as 'Experimental' because it will not be preserved " +
        "after using the 'Create Link' or 'Edit' buttons.";

    [NodeProperty]
    public InputProcedural InputContents { get; } = new InputProcedural()
    {
        Title = "List Item",
        Description = "The node or set of nodes to match multiple times.",
    };

    [NodeProperty]
    public InputString InputSeparator { get; } = new InputString(",")
    {
        Title = "Separator:",
        Description = "The separator character or string to match between each list item.",
    };

    [NodeProperty]
    public InputRange InputListLength { get; } = new InputRange(1, null)
    {
        Title = "List Length:",
        Description = "The number of items in the list.",
        MinValue = 0,
        AutoClearMax = true,
    };

    [NodeProperty]
    public InputCheckbox InputAllowWhitespace { get; } = new InputCheckbox(true)
    {
        Title = "Allow Whitespace",
        Description = "If checked, whitespace is allowed between each list item (after the separator).",
    };

    [NodeProperty]
    public InputCheckbox InputAllowRegex { get; } = new InputCheckbox(false)
    {
        Title = "Regex in Separator",
        Description = "If checked, the separator will be interpreted as a full regular expression, instead of text.",
    };

    [NodeProperty]
    public InputCheckbox InputLazyQuantifier { get; } = new InputCheckbox(false)
    {
        Title = "Lazy Quantifier",
        Description = "If checked, the expression will match as few items as possible.",
    };

    private const string SeparatorCharsToEscape = "()[]{}$^?.+*|";

    protected override NodeResultBuilder GetValue()
    {
        var builder = new NodeResultBuilder(InputContents.Value);

        string whitespace = InputAllowWhitespace.Checked ? "\\s*?" : "";

        string separator = InputSeparator.Value;
        if (!InputAllowRegex.Checked)
            separator = separator.EscapeCharacters(SeparatorCharsToEscape);

        int minReps = InputListLength.Min ?? 0;
        int? maxReps = InputListLength.Max;
        string quantifier = (min: minReps, max: maxReps) switch
        {
            (0, 2) => "?",
            (1, 2) => "?",
            (0, null) => "*",
            (1, null) => "*",
            (2, null) => "+",
            (0, int max) => $"{{{0},{max - 1}}}",
            var range when range.min == range.max => $"{{{range.min - 1}}}",
            var range => $"{{{range.min - 1},{range.max - 1}}}"
        };
        if (InputLazyQuantifier.Checked)
        {
            quantifier += "?";
        }

        string suffix = ")" + quantifier;

        if (minReps <= 0)
        {
            builder.Prepend("(?:", this);
            suffix += ")?";
        }

        builder.Append("(?:" + separator + whitespace, this);
        builder.Append(InputContents.Value);
        builder.Append(suffix, this);

        return builder;
    }
}
