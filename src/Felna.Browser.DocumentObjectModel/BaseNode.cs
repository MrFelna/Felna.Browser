using System.Collections.Generic;

namespace Felna.Browser.DocumentObjectModel;

public abstract class BaseNode : IDocumentNode
{
    protected List<IDocumentNode> _childNodes = new();
    
    public IDocumentNode? ParentNode { get; init; }
    
    public IReadOnlyList<IDocumentNode> ChildNodes => _childNodes.AsReadOnly();
    
    public abstract string NodeName { get; }
}