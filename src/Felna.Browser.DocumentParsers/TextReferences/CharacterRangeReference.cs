namespace Felna.Browser.DocumentParsers.TextReferences;

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

    internal static readonly ICharacterRange AsciiUpperHexLetter = new ContinuousCharacterRange
    {
        LowCharInclusive = CharacterReference.UpperCaseA,
        HighCharInclusive = CharacterReference.UpperCaseF,
    };

    internal static readonly ICharacterRange AsciiLowerHexLetter = new ContinuousCharacterRange
    {
        LowCharInclusive = CharacterReference.LowerCaseA,
        HighCharInclusive = CharacterReference.LowerCaseF,
    };

    internal static readonly ICharacterRange AsciiHex = new CharacterRange
    {
        SubRanges = new ICharacterRange[]
        {
            AsciiUpperHexLetter,
            AsciiLowerHexLetter,
            AsciiDigit,
        },
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
        Chars = new int[]
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
    bool Contains(UnicodeCodePoint c);

    bool Contains(int c);
}

internal class ContinuousCharacterRange : ICharacterRange
{
    internal required int LowCharInclusive { get; init; }
    
    internal required int HighCharInclusive { get; init; }
    
    public bool Contains(int c)
    {
        return LowCharInclusive <= c && c <= HighCharInclusive;
    }

    public bool Contains(UnicodeCodePoint c) => Contains(c.Value);
}

internal class CharacterRange : ICharacterRange
{
    internal required ICharacterRange[] SubRanges { get; init; }

    public bool Contains(int c)
    {
        return SubRanges.Any(r => r.Contains(c));
    }
    
    public bool Contains(UnicodeCodePoint c)
    {
        return SubRanges.Any(r => r.Contains(c));
    }
}

internal class IndividualCharacterRange : ICharacterRange
{
    internal required int[] Chars { get; init; }

    public bool Contains(int c) => Chars.Contains(c);
    
    public bool Contains(UnicodeCodePoint u) => Chars.Any(c => c == u.Value);
}