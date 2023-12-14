namespace Nodexr.NodeTypes;
using Nodexr.Core;
using Nodexr.NodeInputs;
using Nodexr.Nodes;

public class FlagNode : RegexNodeViewModelBase
{
    public override string Title => "Flags";
    public override string NodeInfo => "Inserts flags that change the way the Regex is interpreted.\nEach flag can either be ignored (default), applied (✓), or removed (−).\nLeave the 'Contents' input empty to have the flags apply to everything that comes after them in the Regex, or connect a node to this input so that only that portion of the Regex has the flags applied.";

    [NodeProperty]
    protected InputProcedural InputContents { get; } = new InputProcedural() { Title = "(Optional) Contents" };
    [NodeProperty]
    protected InputCheckboxNullable OptionIgnoreCase { get; } = new InputCheckboxNullable() { Title = "Case Insensitive" };
    [NodeProperty]
    protected InputCheckboxNullable OptionMultiline { get; } = new InputCheckboxNullable() { Title = "Multiline" };
    [NodeProperty]
    protected InputCheckboxNullable OptionSingleline { get; } = new InputCheckboxNullable() { Title = "Singleline" };
    [NodeProperty]
    protected InputCheckboxNullable OptionExplicitCapture { get; } = new InputCheckboxNullable() { Title = "Explicit Capture" };
    [NodeProperty]
    protected InputCheckboxNullable OptionIgnoreWhitespace { get; } = new InputCheckboxNullable() { Title = "Ignore Whitespace" };

    protected override NodeResultBuilder GetValue()
    {
        string flagsOn = "";
        flagsOn += OptionIgnoreCase.CheckedState == 1 ? "i" : "";
        flagsOn += OptionMultiline.CheckedState == 1 ? "m" : "";
        flagsOn += OptionSingleline.CheckedState == 1 ? "s" : "";
        flagsOn += OptionExplicitCapture.CheckedState == 1 ? "n" : "";
        flagsOn += OptionIgnoreWhitespace.CheckedState == 1 ? "x" : "";

        string flagsOff = "";
        flagsOff += OptionIgnoreCase.CheckedState == -1 ? "i" : "";
        flagsOff += OptionMultiline.CheckedState == -1 ? "m" : "";
        flagsOff += OptionSingleline.CheckedState == -1 ? "s" : "";
        flagsOff += OptionExplicitCapture.CheckedState == -1 ? "n" : "";
        flagsOff += OptionIgnoreWhitespace.CheckedState == -1 ? "x" : "";
        if (!string.IsNullOrEmpty(flagsOff))
        {
            flagsOff = "-" + flagsOff;
        }

        var builder = new NodeResultBuilder();
        builder.Append("(?" + flagsOn + flagsOff, this);
        if (InputContents.Connected)
        {
            builder.Append(":", this);
            builder.Append(InputContents.Value);
        }
        builder.Append(")", this);
        return builder;
    }
}
