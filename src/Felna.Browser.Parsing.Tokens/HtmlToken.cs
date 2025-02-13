namespace Felna.Browser.Parsing.Tokens;

public abstract class HtmlToken
{
    public List<HtmlTokenAttribute> TokenAttributes { get; } = new List<HtmlTokenAttribute>();
}