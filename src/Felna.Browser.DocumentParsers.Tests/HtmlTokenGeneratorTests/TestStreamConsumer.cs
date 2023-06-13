using Felna.Browser.DocumentParsers.StreamConsumers;
using Felna.Browser.DocumentParsers.TextReferences;

namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests;

public class TestStreamConsumer : IStreamConsumer
{
    private readonly string _source;
    
    private int _currentCharIndex;

    public TestStreamConsumer(string source)
    {
        _source = source;
    }

    public (bool Success, char character) TryGetCurrentChar()
    {
        if (_currentCharIndex >= _source.Length)
            return (false, CharacterReference.ReplacementCharacter);

        return (true, _source[_currentCharIndex]);
    }

    public (bool Success, string result) LookAhead(int charCount)
    {
        if (charCount < 1)
            throw new ArgumentOutOfRangeException(nameof(charCount), "Char count must be at least 1");

        if (_currentCharIndex + charCount - 1 >= _source.Length)
            return (false, string.Empty);

        return (true, _source.Substring(_currentCharIndex, charCount));
    }

    public bool EndOfStream()
    {
        return _currentCharIndex >= _source.Length;
    }

    public void ConsumeChar(int charCount = 1)
    {
        if (charCount < 1)
            throw new ArgumentOutOfRangeException(nameof(charCount), "Char count must be at least 1");

        _currentCharIndex += charCount;
    }
}