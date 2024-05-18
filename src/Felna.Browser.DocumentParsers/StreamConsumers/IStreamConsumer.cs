using Felna.Browser.DocumentParsers.TextReferences;

namespace Felna.Browser.DocumentParsers.StreamConsumers;

internal interface IStreamConsumer
{
    (bool Success, UnicodeCodePoint CodePoint) TryGetCurrentCodePoint();
    (bool Success, string result) LookAhead(int codePointCount);
    void ConsumeCodePoint(int codePointCount = 1);
}