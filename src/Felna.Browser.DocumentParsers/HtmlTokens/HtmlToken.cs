namespace Felna.Browser.DocumentParsers.HtmlTokens;

internal abstract class HtmlToken
{
    internal List<HtmlTokenAttribute> TokenAttributes { get; } = new List<HtmlTokenAttribute>();
}