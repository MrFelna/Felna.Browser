namespace Felna.Browser.DocumentParsers;

internal abstract class HtmlToken
{
    internal List<HtmlTokenAttribute> TokenAttributes { get; } = new List<HtmlTokenAttribute>();
}

internal sealed class DocTypeToken : HtmlToken
{
    internal string? Name { get; set; }
    
    internal string? PublicIdentifier { get; set; }
    
    internal string? SystemIdentifier { get; set; }
    
    internal bool ForceQuirks { get; set; }
}

internal sealed class TagToken : HtmlToken
{
    internal required bool IsEndTag { get; init; }
    
    internal bool SelfClosing { get; set; }

    internal string Name { get; set; } = string.Empty;
}

internal sealed class CommentToken : HtmlToken
{
    internal string? Data { get; set; }
}

internal sealed class CharacterToken : HtmlToken
{
    internal string? Data { get; set; }
}

internal sealed class EndOfFileToken : HtmlToken
{
}

internal class HtmlTokenAttribute
{
    public string? Name { get; set; }
    
    public string? Value { get; set; }
}