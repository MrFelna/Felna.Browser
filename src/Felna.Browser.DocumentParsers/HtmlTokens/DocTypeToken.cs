namespace Felna.Browser.DocumentParsers.HtmlTokens;

internal sealed class DocTypeToken : HtmlToken
{
    internal string? Name { get; set; }
    
    internal string? PublicIdentifier { get; set; }
    
    internal string? SystemIdentifier { get; set; }
    
    internal bool ForceQuirks { get; set; }
}