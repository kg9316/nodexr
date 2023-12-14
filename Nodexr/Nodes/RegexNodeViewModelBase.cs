﻿namespace Nodexr.Nodes;
using Nodexr.Core;
using Nodexr.NodeInputs;

public abstract class RegexNodeViewModelBase : NodeViewModelBase<NodeResult>
{
    public InputProcedural Previous { get; } = new InputProcedural();
    public override IInputPort PrimaryInput => Previous;
    public INodeOutput<NodeResult>? PreviousNode
    {
        get => Previous.ConnectedNode;
        set => Previous.ConnectedNode = value;
    }

    private NodeResult? cachedOutput;
    public override NodeResult CachedOutput => cachedOutput!;

    public override string OutputTooltip => CachedOutput.Expression;

    public override Vector2 OutputPos => Pos + new Vector2(154, 13);

    public override event Action? OutputChanged;

    protected virtual void OnOutputChanged() => OutputChanged?.Invoke();

    protected void OnInputsChanged()
    {
        cachedOutput = GetOutput();
        OnOutputChanged();
    }

    protected RegexNodeViewModelBase()
    {
        Previous.ValueChanged += OnInputsChanged;

        foreach (var input in NodeInputs)
        {
            input.ValueChanged += OnInputsChanged;
            if (input is InputCollection inputColl)
            {
                inputColl.InputPositionsChanged += OnLayoutChanged;
            }
        }

        OnInputsChanged();
    }

    /// <inheritdoc/>
    public override void CalculateInputsPos()
    {
        //TODO: refactor using GetHeight() on each input
        Previous.Pos = new Vector2(Pos.x + 2, Pos.y + 13);
        if (IsCollapsed)
        {
            const int startHeight = 13;
            foreach (var input in NodeInputs)
            {
                switch (input)
                {
                    case InputProcedural input1:
                        input1.Pos = new Vector2(Pos.x + 2, Pos.y + startHeight);
                        break;
                    case InputCollection input1:
                        input1.Pos = new Vector2(Pos.x + 2, Pos.y + startHeight);
                        foreach (var input2 in input1.Inputs)
                        {
                            input2.Pos = new Vector2(Pos.x + 2, Pos.y + startHeight);
                        }
                        break;
                }
            }
        }
        else
        {
            int startHeight = 44;
            //TODO: Support disabled inputs
            foreach (var input in NodeInputs)
            {
                if (input is InputCollection inputColl)
                {
                    inputColl.Pos = new Vector2(Pos.x, Pos.y + startHeight);
                    foreach (var input2 in inputColl.Inputs)
                    {
                        input2.Pos = new Vector2(Pos.x, Pos.y + startHeight);
                        startHeight += input2.Height;
                    }
                    startHeight += 28;
                }
                else
                {
                    input.Pos = new Vector2(Pos.x, Pos.y + startHeight);
                    startHeight += input.Height;
                }
            }
        }
    }

    public int GetHeight()
    {
        const int baseHeight = 28;

        int inputHeight = NodeInputs
            .Where(input => input.Enabled())
            .Select(input => input.Height)
            .Sum();

        return baseHeight + inputHeight;
    }

    public override string CssName => Title.Replace(" ", "").ToLowerInvariant();
    public override string CssColor => $"var(--col-node-{CssName})";

    protected virtual NodeResult GetOutput()
    {
        var builder = GetValue() ?? new NodeResultBuilder();
        if (Previous.Value != null)
        {
            builder.Prepend(Previous.Value);
        }
        return builder.Build();
    }

    /// <summary>
    /// Get all of the inputs to the node, including the 'previous' input and the sub-inputs of any InputCollections.
    /// InputCollections themselves are not returned.
    /// </summary>
    public override IEnumerable<INodeInput> GetAllInputs()
    {
        yield return Previous;
        foreach (var input in NodeInputs)
        {
            if (input is InputCollection coll)
            {
                foreach (var subInput in coll.Inputs)
                    yield return subInput;
            }
            else
            {
                yield return input;
            }
        }
    }

    protected abstract NodeResultBuilder GetValue();
}
