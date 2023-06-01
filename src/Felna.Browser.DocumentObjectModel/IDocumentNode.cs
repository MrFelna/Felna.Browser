using System.Collections.Generic;

namespace Felna.Browser.DocumentObjectModel;

public interface IDocumentNode
{
    IDocumentNode? ParentNode { get; init; }
    IReadOnlyList<IDocumentNode> ChildNodes { get; }
    public string NodeName { get; }
}