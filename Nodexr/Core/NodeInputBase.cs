﻿namespace Nodexr.Core;

public interface INodeInput
{
    string? Title { get; set; }
    event Action ValueChanged;
    Func<bool> Enabled { get; }
    string? Description { get; set; }
    Vector2 Pos { get; set; }
    int Height { get; }
    bool TrySetValue(object? value);
}

public interface INodeInput<TValue>
{
    public TValue Value { get; set; }
}

public abstract class NodeInputBase : INodeInput
{
    public string? Title { get; set; }

    /// <summary>
    /// The description for this input. Displayed as a tooltip for most types of inputs.
    /// </summary>
    public string? Description { get; set; }

    public event Action? ValueChanged;

    public Vector2 Pos { get; set; }

    public Func<bool> Enabled { get; set; } = () => true;

    public virtual int Height { get; } = 32;

    protected virtual void OnValueChanged()
    {
        ValueChanged?.Invoke();
    }

    public abstract bool TrySetValue(object? value);
}

public abstract class NodeInputBase<TValue> : NodeInputBase, INodeInput<TValue>
{
    public abstract TValue Value { get; set; }

    public override bool TrySetValue(object? value)
    {
        if (value is TValue safeValue)
        {
            Value = safeValue;
            return true;
        }
        return false;
    }

}
