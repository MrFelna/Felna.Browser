using System.Text;

namespace Felna.Browser.DocumentParsers.HtmlTokens;

internal class HtmlTokenAttributeBuilder
{
    private StringBuilder _nameBuilder = new StringBuilder();
    private StringBuilder _valueBuilder = new StringBuilder();

    public void AppendToName(char c) => _nameBuilder.Append(c);
    
    public void AppendToName(string s) => _nameBuilder.Append(s);

    public void AppendToValue(char c) => _valueBuilder.Append(c);
    
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