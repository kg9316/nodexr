﻿namespace Nodexr.NodeTypes;
using Nodexr.Core;
using Nodexr.NodeInputs;
using Nodexr.Nodes;

public class LookaroundNode : RegexNodeViewModelBase
{
    public override string Title => "Lookaround";
    public override string NodeInfo => "Converts the input node into a lookahead or lookbehind.";

    [NodeProperty]
    public InputProcedural Input { get; } = new InputProcedural() { Title = "Contents" };
    [NodeProperty]
    public InputDropdown<Types> InputGroupType { get; } = new InputDropdown<Types>(groupTypeDisplyNames)
    { Title = "Type:" };

    public enum Types
    {
        lookahead,
        lookbehind,
        lookaheadNeg,
        lookbehindNeg,
    }

    private static readonly Dictionary<Types, string> groupTypeDisplyNames = new()
    {
        { Types.lookahead, "Lookahead" },
        { Types.lookbehind, "Lookbehind" },
        { Types.lookaheadNeg, "Negative Lookahead" },
        { Types.lookbehindNeg, "Negative Lookbehind" },
    };

    protected override NodeResultBuilder GetValue()
    {
        var builder = new NodeResultBuilder(Input.Value);

        if (Input.ConnectedNode is OrNode)
            builder.StripNonCaptureGroup();

        string prefix = InputGroupType.Value switch
        {
            Types.lookahead => "(?=",
            Types.lookbehind => "(?<=",
            Types.lookaheadNeg => "(?!",
            Types.lookbehindNeg => "(?<!",
            _ => "",
        };
        builder.Prepend(prefix, this);
        builder.Append(")", this);
        return builder;
    }
}
