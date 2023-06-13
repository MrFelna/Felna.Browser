namespace Felna.Browser.DocumentParsers;

internal static class CharacterRangeReference
{
    internal static readonly ICharacterRange AsciiLowerAlpha = new ContinuousCharacterRange
    {
        LowCharInclusive = CharacterReference.LowerCaseA, 
        HighCharInclusive = CharacterReference.LowerCaseZ
    };

    internal static readonly ICharacterRange AsciiUpperAlpha = new ContinuousCharacterRange
    {
        LowCharInclusive = CharacterReference.UpperCaseA,
        HighCharInclusive = CharacterReference.UpperCaseZ
    };
    
    internal static readonly ICharacterRange AsciiDigit = new ContinuousCharacterRange
    {
        LowCharInclusive = CharacterReference.Digit0,
        HighCharInclusive = CharacterReference.Digit9
    };

    internal static readonly ICharacterRange AsciiAlpha = new CharacterRange
    {
        SubRanges = new ICharacterRange[]
        {
            AsciiLowerAlpha,
            AsciiUpperAlpha,
        }
    };
    
    internal static readonly ICharacterRange AsciiAlphaNumeric = new CharacterRange
    {
        SubRanges = new ICharacterRange[]
        {
            AsciiAlpha,
            AsciiDigit,
        }
    };

    internal static readonly ICharacterRange TokenWhiteSpace = new IndividualCharacterRange
    {
        Chars = new[]
        {
            CharacterReference.CharacterTabulation,
            CharacterReference.LineFeed,
            CharacterReference.FormFeed,
            CharacterReference.Space,
        }
    };
}

internal interface ICharacterRange
{
    bool Contains(char c);
}

internal class ContinuousCharacterRange : ICharacterRange
{
    internal required char LowCharInclusive { get; init; }
    
    internal required char HighCharInclusive { get; init; }

    public bool Contains(char c)
    {
        return LowCharInclusive <= c && c <= HighCharInclusive;
    }
}

internal class CharacterRange : ICharacterRange
{
    internal required ICharacterRange[] SubRanges { get; init; }

    public bool Contains(char c)
    {
        return SubRanges.Any(r => r.Contains(c));
    }
}

internal class IndividualCharacterRange : ICharacterRange
{
    internal required char[] Chars { get; init; }

    public bool Contains(char c) => Chars.Contains(c);
}