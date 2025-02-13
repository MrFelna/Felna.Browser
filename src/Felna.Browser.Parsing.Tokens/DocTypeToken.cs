namespace Felna.Browser.Parsing.Tokens;

public sealed class DocTypeToken : HtmlToken
{
    public string? Name { get; set; }
    
    public string? PublicIdentifier { get; set; }
    
    public string? SystemIdentifier { get; set; }
    
    public bool ForceQuirks { get; set; }
}