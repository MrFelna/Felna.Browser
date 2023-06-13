namespace Felna.Browser.DocumentParsers.HtmlTokens;

internal sealed class CommentToken : HtmlToken
{
    internal string? Data { get; set; }
    
    internal override bool AreValueEqual(HtmlToken other)
    {
        if (other is CommentToken commentToken)
            return AreValueEqual(commentToken);
        return false;
    }

    internal bool AreValueEqual(CommentToken other)
    {
        return Data == other.Data
               && TokenAttributesEqual(other);
    }
}