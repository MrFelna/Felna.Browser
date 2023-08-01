namespace Felna.Browser.DocumentParsers.TextReferences;

internal static class CharacterReference
{
    private const int AsciiUpperLowerDiff = LowerCaseA - UpperCaseA;
    
    internal const char Null = '\u0000'; 
    
    internal const char CharacterTabulation = '\u0009'; // \t
    
    internal const char LineFeed = '\u000A'; // \n
    
    internal const char FormFeed = '\u000C';
    
    internal const char CarriageReturn = '\u000D'; // \r
    
    internal const char Space = '\u0020';
    
    internal const char ExclamationMark = '\u0021';
    
    internal const char QuotationMark = '\u0022';
    
    internal const char Ampersand = '\u0026';
    
    internal const char Apostrophe = '\u0027';

    internal const char Digit0 = '\u0030';
    
    internal const char Digit9 = '\u0039';
    
    internal const char LessThanSign = '\u003C';
    
    internal const char GreaterThanSign = '\u003E';
    
    internal const char QuestionMark = '\u003F';
    
    internal const char UpperCaseA = '\u0041';
    
    internal const char UpperCaseZ = '\u005A';
    
    internal const char LowerCaseA = '\u0061';
    
    internal const char LowerCaseZ = '\u007A';

    internal const char ReplacementCharacter = '\ufffd';

    internal static char ToAsciiLower(char c)
    {
        if (CharacterRangeReference.AsciiUpperAlpha.Contains(c))
            return (char) (c + AsciiUpperLowerDiff);

        return c;
    }
}