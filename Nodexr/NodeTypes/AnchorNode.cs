﻿namespace Nodexr.NodeTypes;
using Nodexr.Core;
using Nodexr.NodeInputs;
using Nodexr.Nodes;

public class AnchorNode : RegexNodeViewModelBase
{
    public override string Title => "Anchor";

    public override string NodeInfo => "Inserts a start-of-line or end-of-line character. " +
        "Useful for ensuring that your regex only matches if it's at a specific position in a line." +
        "\nNote: The \"Start/End of string\" options will match the starts and ends of individual lines when in Multiline mode.";

    [NodeProperty]
    public InputDropdown<Mode> InputAnchorType { get; } = new InputDropdown<Mode>(modeDisplayNames) { Title = "Type of anchor:" };

    public static readonly Dictionary<Mode, string> modeDisplayNames = new()
    {
        { Mode.StartLine, "Start of string" },
        { Mode.EndLine, "End of string" },
        { Mode.WordBoundary, "Word boundary" },
        { Mode.NotWordBoundary, "Not word boundary" }
    };

    public enum Mode
    {
        StartLine,
        EndLine,
        WordBoundary,
        NotWordBoundary,
    }

    protected override NodeResultBuilder GetValue()
    {
        string result = InputAnchorType.Value switch
        {
            Mode.StartLine => "^",
            Mode.EndLine => "$",
            Mode.WordBoundary => "\\b",
            Mode.NotWordBoundary => "\\B",
            _ => "",
        };
        var builder = new NodeResultBuilder();
        builder.Append(result, this);
        return builder;
    }
}
