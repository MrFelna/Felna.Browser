namespace Felna.Browser.Parsing.Text;

public static class CharacterRangeReference
{
    public static readonly ICharacterRange AsciiLowerAlpha = new ContinuousCharacterRange
    {
        LowCharInclusive = CharacterReference.LowerCaseA, 
        HighCharInclusive = CharacterReference.LowerCaseZ
    };

    public static readonly ICharacterRange AsciiUpperAlpha = new ContinuousCharacterRange
    {
        LowCharInclusive = CharacterReference.UpperCaseA,
        HighCharInclusive = CharacterReference.UpperCaseZ
    };
    
    public static readonly ICharacterRange AsciiDigit = new ContinuousCharacterRange
    {
        LowCharInclusive = CharacterReference.Digit0,
        HighCharInclusive = CharacterReference.Digit9
    };

    public static readonly ICharacterRange AsciiUpperHexLetter = new ContinuousCharacterRange
    {
        LowCharInclusive = CharacterReference.UpperCaseA,
        HighCharInclusive = CharacterReference.UpperCaseF,
    };

    public static readonly ICharacterRange AsciiLowerHexLetter = new ContinuousCharacterRange
    {
        LowCharInclusive = CharacterReference.LowerCaseA,
        HighCharInclusive = CharacterReference.LowerCaseF,
    };

    public static readonly ICharacterRange AsciiHex = new CharacterRange
    {
        SubRanges = new ICharacterRange[]
        {
            AsciiUpperHexLetter,
            AsciiLowerHexLetter,
            AsciiDigit,
        },
    };

    public static readonly ICharacterRange AsciiAlpha = new CharacterRange
    {
        SubRanges = new ICharacterRange[]
        {
            AsciiLowerAlpha,
            AsciiUpperAlpha,
        }
    };
    
    public static readonly ICharacterRange AsciiAlphaNumeric = new CharacterRange
    {
        SubRanges = new ICharacterRange[]
        {
            AsciiAlpha,
            AsciiDigit,
        }
    };

    public static readonly ICharacterRange TokenWhiteSpace = new IndividualCharacterRange
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

public interface ICharacterRange
{
    bool Contains(UnicodeCodePoint c);

    bool Contains(int c);
}

public class ContinuousCharacterRange : ICharacterRange
{
    public required int LowCharInclusive { get; init; }
    
    public required int HighCharInclusive { get; init; }
    
    public bool Contains(int c)
    {
        return LowCharInclusive <= c && c <= HighCharInclusive;
    }

    public bool Contains(UnicodeCodePoint c) => Contains(c.Value);
}

public class CharacterRange : ICharacterRange
{
    public required ICharacterRange[] SubRanges { get; init; }

    public bool Contains(int c)
    {
        return SubRanges.Any(r => r.Contains(c));
    }
    
    public bool Contains(UnicodeCodePoint c)
    {
        return SubRanges.Any(r => r.Contains(c));
    }
}

public class IndividualCharacterRange : ICharacterRange
{
    public required int[] Chars { get; init; }

    public bool Contains(int c) => Chars.Contains(c);
    
    public bool Contains(UnicodeCodePoint u) => Chars.Any(c => c == u.Value);
}