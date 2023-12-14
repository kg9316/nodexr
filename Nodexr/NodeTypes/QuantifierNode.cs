﻿namespace Nodexr.NodeTypes;
using static Nodexr.NodeTypes.IQuantifiableNode;
using Nodexr.Utils;
using Nodexr.Core;
using Nodexr.NodeInputs;
using Nodexr.Nodes;

public class QuantifierNode : RegexNodeViewModelBase, IQuantifiableNode
{
    public override string Title => "Quantifier";

    public override string NodeInfo => "Inserts a quantifier to set the minimum and maximum number " +
        "of 'repeats' for the inputted node. Leave the 'max' option blank to allow unlimited repeats." +
        "\n'Greedy' and 'Lazy' search type will attempt to match as many or as few times as possible respectively." +
        "\nThe .NET Regex engine does not support possessive quantifiers, so they are automatically replaced " +
        "by atomic groups (which are functionally identical).";

    [NodeProperty]
    public InputProcedural InputContents { get; } = new InputProcedural()
    {
        Title = "Input",
        Description = "The node or set of nodes that will be matched the chosen number of times.",
    };

    [NodeProperty]
    public InputDropdown<Reps> InputCount { get; } = new InputDropdown<Reps>(displayNamesExcludingOne)
    {
        Title = "Repetitions:",
        Description = "The number of times to match the input.",
        Value = Reps.OneOrMore,
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

    [NodeProperty]
    public InputDropdown<SearchMode> InputSearchType { get; } = new InputDropdown<SearchMode>()
    {
        Title = "Search type:",
        Description = "Changes the way that the Regex engine tries to match the repetition."
    };

    public enum SearchMode
    {
        Greedy,
        Lazy,
        Possessive,
    }

    private static readonly Dictionary<Reps, string> displayNamesExcludingOne = new()
    {
        { Reps.ZeroOrMore, "Zero or more" },
        { Reps.OneOrMore, "One or more" },
        { Reps.ZeroOrOne, "Zero or one" },
        { Reps.Number, "Number" },
        { Reps.Range, "Range" }
    };

    public QuantifierNode()
    {
        InputNumber.Enabled = () => InputCount.Value == Reps.Number;
        InputRange.Enabled = () => InputCount.Value == Reps.Range;
        InputSearchType.Enabled = () => InputCount.Value != Reps.Number;
    }

    protected override NodeResultBuilder GetValue()
    {
        var builder = new NodeResultBuilder(InputContents.Value);

        //Simplify IntegerNode if needed
        if (InputContents.ConnectedNode is IntegerNode)
            builder.StripNonCaptureGroup();

        string suffix = "";
        string prefix = "";

        //Surround with non-capturing group if necessary
        if (InputContents.ConnectedNode is RegexNodeViewModelBase _node
            && RequiresGroupToQuantify(_node))
        {
            prefix += "(?:";
            suffix += ")";
        }

        //Add quantifier
        suffix += GetSuffix(this);

        //Add modifier
        if (InputCount.Value != Reps.Number)
        {
            if (InputSearchType.Value == SearchMode.Lazy)
            {
                suffix += "?";
            }
            else if (InputSearchType.Value == SearchMode.Possessive)
            {
                suffix += ")";
                prefix = "(?>" + prefix;
            }
        }

        builder.Prepend(prefix, this);
        builder.Append(suffix, this);
        return builder;
    }

    /// <summary>
    /// Check whether the given node needs a non-capturing group before it can be quantified.
    /// </summary>
    internal static bool RequiresGroupToQuantify(RegexNodeViewModelBase val)
    {
        if (val is null) throw new ArgumentNullException(nameof(val));

        //Any chain of 2 or more nodes will always need to be wrapped in a group to quantify properly.
        if (val.PreviousNode is not null)
            return true;

        //TODO: refactor using polymorphism
        //All Concat, Quantifier, Decimal, Optional and List nodes also need to be wrapped in a group to quantify properly.
        if (val is ConcatNode
            || val is QuantifierNode
            || val is DecimalNode
            || val is IntegerNode
            || val is OptionalNode
            || val is ListNode
            || val is RecursionNode
        )
        {
            return true;
        }

        if (val is TextNode && !val.CachedOutput.Expression.IsSingleRegexChar())
            return true;

        return false;
    }
}
