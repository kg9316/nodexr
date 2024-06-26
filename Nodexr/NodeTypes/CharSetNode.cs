﻿namespace Nodexr.NodeTypes;
using Nodexr.Core;
using static Nodexr.NodeTypes.IQuantifiableNode;
using Nodexr.NodeInputs;
using Nodexr.Nodes;

public class CharSetNode : RegexNodeViewModelBase, IQuantifiableNode
{
    public override string Title => "Character Set";

    public override string NodeInfo => "Inserts a character class containing the characters you specify. "
        + "You can enter these the same way you would in a normal regex, including ranges (e.g. A-Z).\n"
        + "The 'Invert' option creates a negated class by adding a ^ character at the start.";

    [NodeProperty]
    public InputString InputCharacters { get; } = new InputString("a-z")
    {
        Title = "Characters:",
        Description = "The list of characters or character ranges to match."
    };

    [NodeProperty]
    public InputCheckbox InputDoInvert { get; } = new InputCheckbox(false)
    {
        Title = "Invert",
        Description = "Match everything except the specified characters."
    };

    [NodeProperty]
    public InputDropdown<Reps> InputCount { get; } = new InputDropdown<Reps>(displayNames)
    {
        Title = "Repetitions:",
        Description = "Apply a quantifier to this node."
    };

    [NodeProperty]
    public InputNumber InputNumber { get; } = new InputNumber(0, min: 0) { Title = "Amount:" };

    [NodeProperty]
    public InputRange InputRange { get; } = new InputRange(0, 1)
    {
        Title = "Amount:",
        Description = "The amount of repetitions to allow. Leave the maximum field blank to allow unlimited repetitions.",
        MinValue = 0,
        AutoClearMax = true,
    };

    public CharSetNode()
    {
        SetupInputEnables();
    }

    private void SetupInputEnables()
    {
        InputNumber.Enabled = () => InputCount.Value == Reps.Number;
        InputRange.Enabled = () => InputCount.Value == Reps.Range;
    }

    protected override NodeResultBuilder GetValue()
    {
        string charSet = InputCharacters.GetValue();

        string prefix = InputDoInvert.Checked ? "^" : "";
        string result = "[" + prefix + charSet + "]";

        string suffix = GetSuffix(this);

        var builder = new NodeResultBuilder();
        builder.Append(result, this);
        builder.Append(suffix, this);
        return builder;
    }
}
