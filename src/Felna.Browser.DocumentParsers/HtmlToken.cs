namespace Felna.Browser.DocumentParsers;

internal abstract class HtmlToken
{
    internal List<HtmlTokenAttribute> TokenAttributes { get; } = new List<HtmlTokenAttribute>();

    // May move these methods to tests if unused in library
    internal abstract bool AreValueEqual(HtmlToken other);

    protected bool TokenAttributesEqual(HtmlToken other)
    {
        if (TokenAttributes.Count != other.TokenAttributes.Count)
            return false;

        for (var i = 0; i < TokenAttributes.Count; i++)
        {
            var x = TokenAttributes[i];
            var y = other.TokenAttributes[i];

            if (x.Name != y.Name || x.Value != y.Value)
                return false;
        }

        return true;
    }
}

internal sealed class DocTypeToken : HtmlToken
{
    internal string? Name { get; set; }
    
    internal string? PublicIdentifier { get; set; }
    
    internal string? SystemIdentifier { get; set; }
    
    internal bool ForceQuirks { get; set; }
    
    internal override bool AreValueEqual(HtmlToken other)
    {
        if (other is DocTypeToken docTypeToken)
            return AreValueEqual(docTypeToken);
        return false;
    }
    
    internal bool AreValueEqual(DocTypeToken other)
    {
        return Name == other.Name
               && PublicIdentifier == other.PublicIdentifier
               && SystemIdentifier == other.SystemIdentifier
               && ForceQuirks == other.ForceQuirks
               && TokenAttributesEqual(other);
    }
}

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

internal sealed class EndOfFileToken : HtmlToken
{
    internal override bool AreValueEqual(HtmlToken other)
    {
        return other is EndOfFileToken;
    }
}

internal class HtmlTokenAttribute
{
    public string? Name { get; set; }
    
    public string? Value { get; set; }
}