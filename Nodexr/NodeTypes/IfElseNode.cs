﻿namespace Nodexr.NodeTypes;
using Nodexr.Core;
using Nodexr.NodeInputs;
using Nodexr.Nodes;

public class IfElseNode : RegexNodeViewModelBase
{
    public override string Title => "If-Else";

    public override string NodeInfo => "Matches either of two expressions, depending on whether the 'Condition' expression has matched. " +
        "\nIf the name or number of a captured group is used as the 'Condition' expression, it will be considered to have matched if the group it references was matched.";

    [NodeProperty]
    public InputString InputCondition { get; } = new InputString("")
    {
        Title = "Condition:",
        Description = "The lookahead or group name/number that determines whether to match the first or second option."
    };

    [NodeProperty]
    public InputProcedural InputThen { get; set; } = new InputProcedural() { Title = "Match if true" };

    [NodeProperty]
    public InputProcedural InputElse { get; set; } = new InputProcedural() { Title = "Match if false" };

    protected override NodeResultBuilder GetValue()
    {
        //return null;
        var builder = new NodeResultBuilder();
        //string condition = ConditionType.DropdownValue == "Expression" ? InputCondition.GetValue() : InputGroupID.GetValue();
        string condition = InputCondition.GetValue();

        builder.Append($"(?({condition})", this);
        builder.Append(InputThen.Value);
        builder.Append("|", this);
        builder.Append(InputElse.Value);
        builder.Append(")", this);
        //return $"(?({condition}){InputThen.GetValue()}|{InputElse.GetValue()})";
        return builder;
    }
}
