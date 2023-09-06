namespace Felna.Browser.DocumentParsers.HtmlTokens;

internal sealed class TagToken : HtmlToken
{
    internal required bool IsEndTag { get; init; }
    
    internal bool SelfClosing { get; set; }

    internal string Name { get; set; } = string.Empty;
}