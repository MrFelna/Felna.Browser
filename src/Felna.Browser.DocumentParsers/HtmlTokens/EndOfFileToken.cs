namespace Felna.Browser.DocumentParsers.HtmlTokens;

internal sealed class EndOfFileToken : HtmlToken
{
    internal override bool AreValueEqual(HtmlToken other)
    {
        return other is EndOfFileToken;
    }
}