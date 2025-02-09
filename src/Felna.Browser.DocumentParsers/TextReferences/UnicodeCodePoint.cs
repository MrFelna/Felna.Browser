namespace Felna.Browser.DocumentParsers.TextReferences;

public readonly struct UnicodeCodePoint : IEquatable<UnicodeCodePoint>
{
    public UnicodeCodePoint(char highSurrogate, char lowSurrogate) : this(char.ConvertToUtf32(highSurrogate, lowSurrogate)) { }
    
    public UnicodeCodePoint(int codePoint)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(codePoint, nameof(codePoint));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(codePoint, MaxCodePoint);
        Value = codePoint;
    }
    
    // Instance properties & methods
    
    public int Value { get; }

    public UnicodeCodePoint ToAsciiLower()
    {
        return ToAsciiLower(this);
    }

    public override string ToString()
    {
        return char.ConvertFromUtf32(Value);
    }

    public bool Equals(UnicodeCodePoint other)
    {
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return obj is UnicodeCodePoint other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Value;
    }

    public static bool operator ==(UnicodeCodePoint left, UnicodeCodePoint right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(UnicodeCodePoint left, UnicodeCodePoint right)
    {
        return !left.Equals(right);
    }
    
    // Static properties & methods
    
    private const int AsciiUpperLowerDiff = 32;
    
    public const int MaxCodePoint = 0x10FFFF;

    public static UnicodeCodePoint ReplacementCharacter { get; } = new(CharacterReference.ReplacementCharacter); 

    public static UnicodeCodePoint ToAsciiLower(UnicodeCodePoint c)
    {
        if (CharacterRangeReference.AsciiUpperAlpha.Contains(c))
            return  new UnicodeCodePoint(c.Value + AsciiUpperLowerDiff);

        return c;
    }

    public static string ConvertToString(IEnumerable<UnicodeCodePoint> codePoints)
    {
        ArgumentNullException.ThrowIfNull(codePoints);
        var codePointsArray = codePoints.ToArray();
        var strings = new string[codePointsArray.Length];
        for (var i = 0; i < codePointsArray.Length; i++)
        {
            strings[i] = codePointsArray[i].ToString();
        }
        return string.Concat(strings);
    }

    public static IEnumerable<UnicodeCodePoint> ConvertFromString(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        var codePoints = new List<UnicodeCodePoint>(input.Length);
        for (var i = 0; i < input.Length; i++)
        {
            UnicodeCodePoint codePoint;
            if (char.IsSurrogatePair(input, i))
            {
                codePoint = new UnicodeCodePoint(input[i], input[i + 1]);
                i++;
            }
            else
            {
                codePoint = new UnicodeCodePoint(input[i]);
            }
            codePoints.Add(codePoint);
        }

        return codePoints;
    }
}