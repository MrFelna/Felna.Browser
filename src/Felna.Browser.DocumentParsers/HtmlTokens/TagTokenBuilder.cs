using System.Text;
using Felna.Browser.DocumentParsers.TextReferences;

namespace Felna.Browser.DocumentParsers.HtmlTokens;

internal class TagTokenBuilder
{
    private readonly StringBuilder _tagNameBuilder = new StringBuilder();
    private readonly Dictionary<string, HtmlTokenAttribute> _attributes = new Dictionary<string, HtmlTokenAttribute>();
    
    private StringBuilder _attributeNameBuilder = new StringBuilder();
    private StringBuilder _attributeValueBuilder = new StringBuilder();
    private bool _endTag;
    private bool _selfClosing;
    
    public void AppendToTagName(UnicodeCodePoint c) => AppendToTagName(c.ToString());
    
    public void AppendToTagName(string s) => _tagNameBuilder.Append(s);

    public void StartNewAttribute()
    {
        var attributeName = _attributeNameBuilder.ToString();
        var key = attributeName.ToUpperInvariant();
        if (!string.IsNullOrWhiteSpace(key) && !_attributes.ContainsKey(key))
        {
            _attributes.Add(key, new HtmlTokenAttribute{ Name = attributeName, Value = _attributeValueBuilder.ToString() });
        }

        _attributeNameBuilder = new StringBuilder();
        _attributeValueBuilder = new StringBuilder();
    }
    
    public void AppendToAttributeName(UnicodeCodePoint c) => AppendToAttributeName(c.ToString());
    
    public void AppendToAttributeName(string s) => _tagNameBuilder.Append(s);

    public void AppendToAttributeValue(UnicodeCodePoint c) => AppendToAttributeValue(c.ToString());
    
    public void AppendToAttributeValue(string s) => _tagNameBuilder.Append(s);

    public void SetEndTag() => _endTag = true;

    public void SetSelfClosing() => _selfClosing = true;

    public TagToken Build()
    {
        StartNewAttribute();
        var tagName = _tagNameBuilder.ToString();
        var tagToken = new TagToken
        {
            Name = tagName,
            IsEndTag = _endTag,
            SelfClosing = _selfClosing,
        };
        tagToken.TokenAttributes.AddRange(_attributes.Values);
        return tagToken;
    }
}