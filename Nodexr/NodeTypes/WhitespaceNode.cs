namespace Nodexr.NodeTypes;
using static Nodexr.NodeTypes.IQuantifiableNode;
using Nodexr.Core;
using Nodexr.NodeInputs;

public class WhitespaceNode : RegexNodeViewModelBase, IQuantifiableNode
{
    public override string Title => "Whitespace";

    public override string NodeInfo => "Matches any of the specified types of whitespace character." +
        "\nIf 'Invert' is checked, matches everything BUT the specified types of whitespace character.";

    [NodeProperty]
    public InputCheckbox InputInvert { get; } = new InputCheckbox(false)
    {
        Title = "Invert",
        Description = "Match everything except the specified types of whitespace.\nThis includes all non-whitespace characters."
    };

    [NodeProperty]
    public InputCheckbox InputAllWhitespace { get; } = new InputCheckbox(true) { Title = "All Whitespace" };

    [NodeProperty]
    public InputCheckbox InputSpace { get; } = new InputCheckbox(true) { Title = "Space" };

    [NodeProperty]
    public InputCheckbox InputTab { get; } = new InputCheckbox(true) { Title = "Tab" };

    [NodeProperty]
    public InputCheckbox InputCR { get; } = new InputCheckbox(true) { Title = "Newline (\\r)" };

    [NodeProperty]
    public InputCheckbox InputLF { get; } = new InputCheckbox(true) { Title = "Newline (\\n)" };

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

    public WhitespaceNode()
    {
        bool IsAllWhitespaceUnchecked() => !InputAllWhitespace.Checked;

        InputSpace.Enabled = IsAllWhitespaceUnchecked;
        InputTab.Enabled = IsAllWhitespaceUnchecked;
        InputCR.Enabled = IsAllWhitespaceUnchecked;
        InputLF.Enabled = IsAllWhitespaceUnchecked;

        InputNumber.Enabled = () => InputCount.Value == Reps.Number;
        InputRange.Enabled = () => InputCount.Value == Reps.Range;
    }

    protected override NodeResultBuilder GetValue()
    {
        string suffix = GetSuffix(this);

        var builder = new NodeResultBuilder(ValueString(), this);
        builder.Append(suffix, this);
        return builder;
    }

    private string ValueString()
    {
        bool invert = InputInvert.Checked;

        if (InputAllWhitespace.Checked)
        {
            return invert ? "\\S" : "\\s";
        }

        List<string> charsToAllow = new List<string>();

        if (InputSpace.Checked) charsToAllow.Add(" ");
        if (InputTab.Checked) charsToAllow.Add("\\t");
        if (InputCR.Checked) charsToAllow.Add("\\r");
        if (InputLF.Checked) charsToAllow.Add("\\n");

        string charsConverted = string.Concat(charsToAllow);
        if (invert)
        {
            return "[^" + charsConverted + "]";
        }
        else if (charsToAllow.Count > 1)
        {
            return "[" + charsConverted + "]";
        }
        else
        {
            return "" + charsConverted;
        }
    }
}
