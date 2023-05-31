namespace Felna.Browser.DocumentObjectModel;

public interface IDocumentNode
{
    IDocumentNode? ParentNode { get; }
    IDocumentNodeList ChildNodes { get; }
    public string NodeName { get; }
}