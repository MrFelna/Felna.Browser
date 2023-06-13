namespace Felna.Browser.DocumentParsers.HtmlTokens;

internal sealed class TagToken : HtmlToken
{
    internal required bool IsEndTag { get; init; }
    
    internal bool SelfClosing { get; set; }

    internal string Name { get; set; } = string.Empty;
    
    internal override bool AreValueEqual(HtmlToken other)
    {
        if (other is TagToken tagToken)
            return AreValueEqual(tagToken);
        return false;
    }

    internal bool AreValueEqual(TagToken other)
    {
        return IsEndTag == other.IsEndTag
               && SelfClosing == other.SelfClosing
               && Name == other.Name
               && TokenAttributesEqual(other);
    }
}