namespace Felna.Browser.Parsing.Text;

public static class CharacterReference
{
    private const int AsciiUpperLowerDiff = LowerCaseA - UpperCaseA;
    
    public const char Null = '\u0000'; 
    
    public const char CharacterTabulation = '\u0009'; // \t
    
    public const char LineFeed = '\u000A'; // \n
    
    public const char FormFeed = '\u000C';
    
    public const char CarriageReturn = '\u000D'; // \r
    
    public const char Space = '\u0020';
    
    public const char ExclamationMark = '\u0021';
    
    public const char QuotationMark = '\u0022';
    
    public const char NumberSign = '\u0023';
    
    public const char Ampersand = '\u0026';
    
    public const char Apostrophe = '\u0027';
    
    public const char HyphenMinus = '\u002D';

    public const char Solidus = '\u002F';

    public const char Digit0 = '\u0030';
    
    public const char Digit9 = '\u0039';
    
    public const char SemiColon = '\u003B';
    
    public const char LessThanSign = '\u003C';
    
    public const char EqualsSign = '\u003D';
    
    public const char GreaterThanSign = '\u003E';
    
    public const char QuestionMark = '\u003F';
    
    public const char UpperCaseA = '\u0041';
    
    public const char UpperCaseF = '\u0046';
    
    public const char UpperCaseX = '\u0058';
    
    public const char UpperCaseZ = '\u005A';
    
    public const char LowerCaseA = '\u0061';
    
    public const char LowerCaseF = '\u0066';
    
    public const char LowerCaseX = '\u0078';
    
    public const char LowerCaseZ = '\u007A';

    public const char ReplacementCharacter = '\ufffd';

    public static int ToAsciiLower(int c)
    {
        if (CharacterRangeReference.AsciiUpperAlpha.Contains(c))
            return (char) (c + AsciiUpperLowerDiff);

        return c;
    }
}