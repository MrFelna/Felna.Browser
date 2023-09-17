using System.Text;

namespace Felna.Browser.DocumentParsers.HtmlTokens;

internal class DocTypeTokenBuilder
{
    private bool _publicIdentifierPresent;
    private bool _systemIdentifierPresent;
    private bool _forceQuirks;
    private StringBuilder _nameBuilder = new StringBuilder();
    private StringBuilder _publicIdentifierBuilder = new StringBuilder();
    private StringBuilder _systemIdentifierBuilder = new StringBuilder();
    
    public DocTypeTokenBuilder SetForceQuirks()
    {
        _forceQuirks = true;
        return this;
    }

    public void AppendToName(char c) => _nameBuilder.Append(c);

    public void AppendToPublicIdentifier(char c) => _publicIdentifierBuilder.Append(c);

    public void SetPublicIdentifierPresent() => _publicIdentifierPresent = true;

    public void AppendToSystemIdentifier(char c) => _systemIdentifierBuilder.Append(c);

    public void SetSystemIdentifierPresent() => _systemIdentifierPresent = true;

    public DocTypeToken Build()
    {
        var name = _nameBuilder.ToString();
        return new DocTypeToken
        {
            ForceQuirks = _forceQuirks,
            Name = name.Length > 0 ? name : null,
            PublicIdentifier = _publicIdentifierPresent ? _publicIdentifierBuilder.ToString() : null,
            SystemIdentifier = _systemIdentifierPresent ? _systemIdentifierBuilder.ToString() : null,
        };
    }
}