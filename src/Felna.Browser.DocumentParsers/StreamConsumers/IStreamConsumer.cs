namespace Felna.Browser.DocumentParsers.StreamConsumers;

internal interface IStreamConsumer
{
    (bool Success, char character) TryGetCurrentChar();
    (bool Success, string result) LookAhead(int charCount);
    bool EndOfStream();
    void ConsumeChar(int charCount = 1);
}