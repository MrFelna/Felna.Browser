namespace Felna.Browser.DocumentObjectModel;

public sealed class TextNode : BaseNode
{
    public string Text { get; set; } = string.Empty;
    public override string NodeName { get; } = "#text";
}