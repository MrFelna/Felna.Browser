using Felna.Browser.Parsing.Text;

namespace Felna.Browser.Parsing.Interfaces;

public interface IStreamConsumer
{
    (bool Success, UnicodeCodePoint CodePoint) TryGetCurrentCodePoint();
    (bool Success, string result) LookAhead(int codePointCount);
    void ConsumeCodePoint(int codePointCount = 1);
}