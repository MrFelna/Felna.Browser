namespace Felna.Browser.DocumentParsers.HtmlTokens;

internal sealed class DocTypeToken : HtmlToken
{
    internal string? Name { get; set; }
    
    internal string? PublicIdentifier { get; set; }
    
    internal string? SystemIdentifier { get; set; }
    
    internal bool ForceQuirks { get; set; }
    
    internal override bool AreValueEqual(HtmlToken other)
    {
        if (other is DocTypeToken docTypeToken)
            return AreValueEqual(docTypeToken);
        return false;
    }
    
    internal bool AreValueEqual(DocTypeToken other)
    {
        return Name == other.Name
               && PublicIdentifier == other.PublicIdentifier
               && SystemIdentifier == other.SystemIdentifier
               && ForceQuirks == other.ForceQuirks
               && TokenAttributesEqual(other);
    }
}