using System.Collections.Generic;

namespace Felna.Browser.DocumentObjectModel;

public abstract class BaseNode : IDocumentNode
{
    protected IList<IDocumentNode> ChildNodeList { get; } = new List<IDocumentNode>();
    
    public IDocumentNode? ParentNode { get; init; }
    
    public IReadOnlyList<IDocumentNode> ChildNodes => ChildNodeList.AsReadOnly();
    
    public abstract string NodeName { get; }

    public void AppendNode(IDocumentNode node)
    {
        ChildNodeList.Add(node);
    }
}