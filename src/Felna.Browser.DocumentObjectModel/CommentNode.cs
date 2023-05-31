namespace Felna.Browser.DocumentObjectModel;

public sealed class CommentNode : BaseNode
{
    public string Comment { get; set; } = string.Empty;
    public override string NodeName { get; } = "#comment";
}