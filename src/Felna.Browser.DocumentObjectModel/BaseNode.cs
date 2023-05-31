namespace Felna.Browser.DocumentObjectModel;

public abstract class BaseNode : IDocumentNode
{
    public IDocumentNode? ParentNode { get; }
    public IDocumentNodeList ChildNodes { get; }
    public abstract string NodeName { get; }
}