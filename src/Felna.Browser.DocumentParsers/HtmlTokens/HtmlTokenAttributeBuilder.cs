using System.Text;
using Felna.Browser.DocumentParsers.TextReferences;

namespace Felna.Browser.DocumentParsers.HtmlTokens;

internal class HtmlTokenAttributeBuilder
{
    private StringBuilder _nameBuilder = new StringBuilder();
    private StringBuilder _valueBuilder = new StringBuilder();

    public void AppendToName(UnicodeCodePoint c) => _nameBuilder.Append(c.ToString());
    
    public void AppendToName(string s) => _nameBuilder.Append(s);

    public void AppendToValue(UnicodeCodePoint c) => _valueBuilder.Append(c.ToString());
    
    public void AppendToValue(string s) => _valueBuilder.Append(s);
    
    public HtmlTokenAttribute Build()
    {
        return new HtmlTokenAttribute
        {
            Name = _nameBuilder.ToString(),
            Value = _valueBuilder.ToString(),
        };
    }
    
}