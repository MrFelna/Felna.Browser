using System.Text;
using Felna.Browser.DocumentObjectModel;

namespace Felna.Browser.DocumentParsers;

public sealed class HtmlParser
{
    public static DocumentNode Parse(Stream stream, Decoder decoder)
    {
        var document = new DocumentNode();

        return document;
    }
}