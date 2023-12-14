﻿namespace Nodexr.NodeTypes;
using Nodexr.Core;
using Nodexr.NodeInputs;
using Nodexr.Nodes;

public class ConcatNode : RegexNodeViewModelBase
{
    public override string Title => "Concatenate";
    public override string NodeInfo => "Concatenates (strings together) the outputs of multiple nodes, so that they come one after another. This could also be thought of as an 'And' or 'Then' node. The resulting regex will be order-sensitive, so order your inputs from top to bottom.";

    [NodeProperty]
    protected InputCollection Inputs { get; } = new InputCollection("Item", 2);

    protected override NodeResultBuilder GetValue()
    {
        var builder = new NodeResultBuilder();
        foreach (var input in Inputs.Inputs)
        {
            builder.Append(input.Value);
        }
        return builder;
    }
}
