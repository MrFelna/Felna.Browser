namespace Felna.Browser.DocumentObjectModel;

public class ElementNode : BaseNode
{
    public string? NamespacePrefix { get; }
    public string LocalName { get; } = "div";
    public string QualifiedName { get; } = "div";
    public override string NodeName { get; } = "DIV";
}