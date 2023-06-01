namespace Felna.Browser.DocumentObjectModel;

public class ElementNode : BaseNode
{
    public ElementNode(string? namespacePrefix, string localName)
    {
        NamespacePrefix = namespacePrefix;
        LocalName = localName;
        QualifiedName = string.IsNullOrWhiteSpace(namespacePrefix) ? localName : $"{namespacePrefix}:{localName}";
        NodeName = QualifiedName.ToUpperInvariant();
    }

    public string? NamespacePrefix { get; }
    public string LocalName { get; }
    public string QualifiedName { get; }
    public override string NodeName { get; }
}