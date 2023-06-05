using System.Text;

namespace Felna.Browser.DocumentParsers;

internal static class HtmlTokenGenerator
{
    internal static IEnumerable<HtmlToken> TokenizeStream(Stream stream, Decoder decoder)
    {
        return Array.Empty<HtmlToken>();
    }
}