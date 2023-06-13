using System.Text;
using Felna.Browser.DocumentParsers.TextReferences;

namespace Felna.Browser.DocumentParsers.StreamConsumers;

internal class UTF8StreamConsumer : IStreamConsumer
{
    private readonly List<char> _charList = new List<char>();
    private readonly Decoder _decoder = Encoding.UTF8.GetDecoder();
    private readonly Stream _stream;
    
    private bool _endOfStream;
    private int _currentCharIndex;

    internal UTF8StreamConsumer(Stream stream)
    {
        _stream = stream;
    }

    public (bool Success, char character) TryGetCurrentChar()
    {
        if (!ReadUntil(_currentCharIndex))
            return (false, CharacterReference.ReplacementCharacter);
            
        return (true, _charList[_currentCharIndex]);
    }

    public (bool Success, string result) LookAhead(int charCount)
    {
        if (charCount < 1)
            throw new ArgumentOutOfRangeException(nameof(charCount), "Char count must be at least 1");
        
        if (!ReadUntil(_currentCharIndex + charCount - 1))
            return (false, string.Empty);

        var chars = _charList.GetRange(_currentCharIndex, charCount).ToArray();

        return (true, new string(chars));
    }

    public bool EndOfStream() => _endOfStream;

    public void ConsumeChar(int charCount = 1)
    {
        if (charCount < 1)
            throw new ArgumentOutOfRangeException(nameof(charCount), "Char count must be at least 1");

        _currentCharIndex += charCount;
    }

    private bool ReadUntil(int charIndex)
    {
        while (_charList.Count <= charIndex)
        {
            var (success, character) = TryReadNextCharacter();
            if (!success)
                return false;
            _charList.Add(character);
        }

        return true;
    }
    
    // https://stackoverflow.com/a/11672355
    private (bool Success, char character) TryReadNextCharacter()
    {
        if (_endOfStream)
            return (false, CharacterReference.ReplacementCharacter);
        
        int byteAsInt;
        var nextChar = new char[1];

        while ((byteAsInt = _stream.ReadByte()) != -1)
        {
            var charCount = _decoder.GetChars(new[] {(byte) byteAsInt}, 0, 1, nextChar, 0);
            if(charCount == 0) continue;

            return (true, nextChar[0]);
        }

        _endOfStream = true;

        return (false, CharacterReference.ReplacementCharacter);
    }
}