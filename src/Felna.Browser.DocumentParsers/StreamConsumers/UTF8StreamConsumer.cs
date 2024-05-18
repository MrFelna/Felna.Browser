using System.Text;
using Felna.Browser.DocumentParsers.TextReferences;

namespace Felna.Browser.DocumentParsers.StreamConsumers;

internal class UTF8StreamConsumer : IStreamConsumer
{
    private readonly List<UnicodeCodePoint> _codePointList = new List<UnicodeCodePoint>();
    private readonly Decoder _decoder = Encoding.UTF8.GetDecoder();
    private readonly Stream _stream;
    
    private bool _sourceStreamExhausted;
    private int _currentCodePointIndex;

    internal UTF8StreamConsumer(Stream stream)
    {
        _stream = stream;
    }

    public (bool Success, UnicodeCodePoint CodePoint) TryGetCurrentCodePoint()
    {
        if (!ReadUntil(_currentCodePointIndex))
            return (false, UnicodeCodePoint.ReplacementCharacter);
            
        return (true, _codePointList[_currentCodePointIndex]);
    }

    public (bool Success, string result) LookAhead(int codePointCount)
    {
        if (codePointCount < 1)
            throw new ArgumentOutOfRangeException(nameof(codePointCount), "Code point count must be at least 1");
        
        if (!ReadUntil(_currentCodePointIndex + codePointCount - 1))
            return (false, string.Empty);

        var result = UnicodeCodePoint.ConvertToString(_codePointList.GetRange(_currentCodePointIndex, codePointCount));

        return (true, result);
    }

    public void ConsumeCodePoint(int codePointCount = 1)
    {
        if (codePointCount < 1)
            throw new ArgumentOutOfRangeException(nameof(codePointCount), "Code point count must be at least 1");

        _currentCodePointIndex += codePointCount;
    }

    private bool ReadUntil(int codePointIndex)
    {
        while (_codePointList.Count <= codePointIndex)
        {
            var (success, codePoint) = TryReadNextCodePoint();
            if (!success)
                return false;
            _codePointList.Add(codePoint);
        }

        return true;
    }
    
    // https://stackoverflow.com/a/11672355
    private (bool Success, UnicodeCodePoint codePoint) TryReadNextCodePoint()
    {
        if (_sourceStreamExhausted)
            return (false, UnicodeCodePoint.ReplacementCharacter);
        
        int byteAsInt;
        var nextChar = new char[2]; // need two to handle code points above \uFFFF

        while ((byteAsInt = _stream.ReadByte()) != -1)
        {
            var charCount = _decoder.GetChars(new[] {(byte) byteAsInt}, 0, 1, nextChar, 0);
            switch (charCount)
            {
                case 0:
                    continue;
                case 1:
                    return (true, new UnicodeCodePoint(nextChar[0])); // Return the single char
                default:
                    // Convert two UTF16 chars representing a code point above \uFFFF to UTF32
                    return (true, new UnicodeCodePoint(nextChar[0], nextChar[1]));
            }
        }

        _sourceStreamExhausted = true;

        return (false, UnicodeCodePoint.ReplacementCharacter);
    }
}