using System.Text;
using Felna.Browser.DocumentParsers.TextReferences;

namespace Felna.Browser.DocumentParsers.StreamConsumers;

internal class UTF8StreamConsumer : IStreamConsumer
{
    private readonly List<char> _charList = new List<char>();
    private readonly Decoder _decoder = Encoding.UTF8.GetDecoder();
    private readonly Stream _stream;
    
    private bool _sourceStreamExhausted;
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
            var (success, characters) = TryReadNextCharacter();
            if (!success)
                return false;
            _charList.AddRange(characters);
        }

        return true;
    }
    
    // https://stackoverflow.com/a/11672355
    private (bool Success, char[] characters) TryReadNextCharacter()
    {
        if (_sourceStreamExhausted)
            return (false, new [] { CharacterReference.ReplacementCharacter });
        
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
                    return (true, nextChar[..1]); // Return an array of the single char
                default:
                    return (true, nextChar); // Return the array of two chars representing a code point above \uFFFF
            }
        }

        _sourceStreamExhausted = true;

        return (false, new [] { CharacterReference.ReplacementCharacter });
    }
}