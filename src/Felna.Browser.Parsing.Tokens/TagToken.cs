namespace Felna.Browser.Parsing.Tokens;

public sealed class TagToken : HtmlToken
{
    public required bool IsEndTag { get; init; }
    
    public bool SelfClosing { get; set; }

    public string Name { get; set; } = string.Empty;
}