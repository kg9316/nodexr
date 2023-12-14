﻿namespace Nodexr.NodeTypes;
using Nodexr.Core;
using Nodexr.NodeInputs;

public class UnicodeNode : RegexNodeViewModelBase
{
    public override string Title => "Unicode";

    public override string NodeInfo => "Insert a unicode category, unicode block, or the hex value of a unicode/ascii character.";

    [NodeProperty]
    public InputDropdown<Modes> InputMode { get; } = new InputDropdown<Modes>(modeDisplayNames) { Title = "Mode" };
    [NodeProperty]
    public InputString InputCategory { get; } = new InputString("IsBasicLatin") { Title = "Unicode Category" };
    [NodeProperty]
    public InputString InputHexCode { get; } = new InputString("1e22") { Title = "Hex Code" };
    [NodeProperty]
    public InputCheckbox InputInvert { get; } = new InputCheckbox() { Title = "Invert" };

    public enum Modes
    {
        Category,
        Hex
    }

    private static readonly Dictionary<Modes, string> modeDisplayNames = new()
    {
        { Modes.Category, "Category/Block" },
        { Modes.Hex, "Hex Code" },
    };

    public UnicodeNode()
    {
        InputCategory.Enabled = () => InputMode.Value == Modes.Category;
        InputHexCode.Enabled = () => InputMode.Value == Modes.Hex;
    }

    protected override NodeResultBuilder GetValue()
    {
        return new NodeResultBuilder(ValueString(), this);
    }

    private string ValueString()
    {
        return InputMode.Value switch
        {
            Modes.Category => GetCategoryRegex(InputCategory.Value, InputInvert.Checked),
            Modes.Hex => GetHexCodeRegex(InputHexCode.Value, InputInvert.Checked),
            _ => "",
        };
    }

    private static string GetCategoryRegex(string input, bool invert)
    {
        if (invert)
        {
            return @"\P{" + input + "}";
        }
        else
        {
            return @"\p{" + input + "}";
        }
    }

    private static string GetHexCodeRegex(string input, bool invert)
    {
        input = input.Length > 2
            ? "\\u" + input.PadLeft(4, '0')
            : "\\x" + input.PadLeft(2, '0');

        return invert
            ? "[^" + input + "]"
            : input;
    }
}
