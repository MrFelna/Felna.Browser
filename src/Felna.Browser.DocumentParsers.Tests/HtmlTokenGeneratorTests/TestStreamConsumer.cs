namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests;

public class TestStreamConsumer : IStreamConsumer
{
    private readonly List<UnicodeCodePoint> _codePoints;
    
    private int _currentCodePointIndex;

    public TestStreamConsumer(string source)
    {
        ArgumentNullException.ThrowIfNull(source);
        _codePoints = new List<UnicodeCodePoint>(UnicodeCodePoint.ConvertFromString(source));
    }

    public (bool Success, UnicodeCodePoint CodePoint) TryGetCurrentCodePoint()
    {
        if (_currentCodePointIndex >= _codePoints.Count)
            return (false, UnicodeCodePoint.ReplacementCharacter);

        return (true, _codePoints[_currentCodePointIndex]);
    }

    public (bool Success, string result) LookAhead(int codePointCount)
    {
        if (codePointCount < 1)
            throw new ArgumentOutOfRangeException(nameof(codePointCount), "Code point count must be at least 1");

        if (_currentCodePointIndex + codePointCount - 1 >= _codePoints.Count)
            return (false, string.Empty);

        return (true, UnicodeCodePoint.ConvertToString(_codePoints.GetRange(_currentCodePointIndex, codePointCount)));
    }

    public void ConsumeCodePoint(int codePointCount = 1)
    {
        if (codePointCount < 1)
            throw new ArgumentOutOfRangeException(nameof(codePointCount), "Code point count must be at least 1");

        _currentCodePointIndex += codePointCount;
    }
}