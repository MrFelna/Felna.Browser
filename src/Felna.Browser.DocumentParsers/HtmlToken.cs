namespace Felna.Browser.DocumentParsers;

internal class HtmlToken
{
    internal required HtmlTokenType TokenType { get; init; }
}

internal class HtmlTokenAttribute
{
    
}

internal enum HtmlTokenType
{
    Unknown = 0,
    DocType = 1,
    StartTag = 2,
    EndTag = 3,
    Comment = 4,
    Character = 5,
    EndOfFile = 6,
}