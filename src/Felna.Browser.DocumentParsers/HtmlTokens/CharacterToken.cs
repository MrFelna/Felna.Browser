namespace Felna.Browser.DocumentParsers.HtmlTokens;

internal sealed class CharacterToken : HtmlToken
{
    internal string? Data { get; set; }
    
    internal override bool AreValueEqual(HtmlToken other)
    {
        if (other is CharacterToken characterToken)
            return AreValueEqual(characterToken);
        return false;
    }

    internal bool AreValueEqual(CharacterToken other)
    {
        return Data == other.Data
               && TokenAttributesEqual(other);
    }
}