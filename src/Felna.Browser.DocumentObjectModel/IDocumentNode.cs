namespace Felna.Browser.DocumentObjectModel;

public interface IDocumentNode
{
    IDocumentNode? ParentNode { get; init; }
    IDocumentNodeList ChildNodes { get; }
    public string NodeName { get; }
}