using Felna.Browser.DocumentParsers.StreamConsumers;
using Felna.Browser.DocumentParsers.TextReferences;

namespace Felna.Browser.DocumentParsers.HtmlTokens;

internal class DocTypeTokenGenerator
{
    private readonly IStreamConsumer _streamConsumer;
    private readonly DocTypeTokenBuilder _docTypeTokenBuilder;
    private HtmlToken? _docTypeToken;

    public DocTypeTokenGenerator(IStreamConsumer streamConsumer)
    {
        _streamConsumer = streamConsumer;
        _docTypeTokenBuilder = new DocTypeTokenBuilder();
    }

    internal HtmlToken GetDocTypeToken()
    {
        if (_docTypeToken != null)
            return _docTypeToken;
        
        var token = DocTypeTokenInDoctypeState();
        _docTypeToken = token;
        return token;
    }

    private HtmlToken DocTypeTokenInDoctypeState()
    {
        var (success, character) = _streamConsumer.TryGetCurrentChar();

        // EOF
        if (!success)
            return _docTypeTokenBuilder.SetForceQuirks().Build();

        switch (character)
        {
            case CharacterReference.CharacterTabulation:
            case CharacterReference.LineFeed:
            case CharacterReference.FormFeed:
            case CharacterReference.Space:
                _streamConsumer.ConsumeChar();
                return GetDocTypeTokenInBeforeNameState();
            case CharacterReference.GreaterThanSign:
                return GetDocTypeTokenInBeforeNameState();
            default:
                return GetDocTypeTokenInBeforeNameState();
        }
    }

    private HtmlToken GetDocTypeTokenInBeforeNameState()
    {
        while (true)
        {
            var (success, character) = _streamConsumer.TryGetCurrentChar();

            // EOF
            if (!success)
                return _docTypeTokenBuilder.SetForceQuirks().Build();

            switch (character)
            {
                case CharacterReference.CharacterTabulation:
                case CharacterReference.LineFeed:
                case CharacterReference.FormFeed:
                case CharacterReference.Space:
                    _streamConsumer.ConsumeChar();
                    break;
                case var _ when CharacterRangeReference.AsciiUpperAlpha.Contains(character):
                    _streamConsumer.ConsumeChar();
                    _docTypeTokenBuilder.AppendToName(CharacterReference.ToAsciiLower(character));
                    return GetDocTypeTokenInNameState();
                case CharacterReference.Null:
                    _streamConsumer.ConsumeChar();
                    _docTypeTokenBuilder.AppendToName(CharacterReference.ReplacementCharacter);
                    return GetDocTypeTokenInNameState();
                case CharacterReference.GreaterThanSign:
                    _streamConsumer.ConsumeChar();
                    return _docTypeTokenBuilder.SetForceQuirks().Build();
                default:
                    _streamConsumer.ConsumeChar();
                    _docTypeTokenBuilder.AppendToName(character);
                    return GetDocTypeTokenInNameState();
            }
        }
    }

    private HtmlToken GetDocTypeTokenInNameState()
    {
        while (true)
        {
            var (success, character) = _streamConsumer.TryGetCurrentChar();

            // EOF
            if (!success)
                return _docTypeTokenBuilder.SetForceQuirks().Build();

            switch (character)
            {
                case CharacterReference.CharacterTabulation:
                case CharacterReference.LineFeed:
                case CharacterReference.FormFeed:
                case CharacterReference.Space:
                    _streamConsumer.ConsumeChar();
                    return GetDocTypeTokenInAfterNameState();
                case CharacterReference.GreaterThanSign:
                    _streamConsumer.ConsumeChar();
                    return _docTypeTokenBuilder.Build();
                case var _ when CharacterRangeReference.AsciiUpperAlpha.Contains(character):
                    _streamConsumer.ConsumeChar();
                    _docTypeTokenBuilder.AppendToName(CharacterReference.ToAsciiLower(character));
                    break;
                case CharacterReference.Null:
                    _streamConsumer.ConsumeChar();
                    _docTypeTokenBuilder.AppendToName(CharacterReference.ReplacementCharacter);
                    break;
                default:
                    _streamConsumer.ConsumeChar();
                    _docTypeTokenBuilder.AppendToName(character);
                    break;
            }
        }
    }

    private HtmlToken GetDocTypeTokenInAfterNameState()
    {
        while (true)
        {
            var (success, character) = _streamConsumer.TryGetCurrentChar();

            // EOF
            if (!success)
                return _docTypeTokenBuilder.SetForceQuirks().Build();

            switch (character)
            {
                case CharacterReference.CharacterTabulation:
                case CharacterReference.LineFeed:
                case CharacterReference.FormFeed:
                case CharacterReference.Space:
                    _streamConsumer.ConsumeChar();
                    break;
                case CharacterReference.GreaterThanSign:
                    _streamConsumer.ConsumeChar();
                    return _docTypeTokenBuilder.Build();
                default:
                    (success, var result) = _streamConsumer.LookAhead(StringReference.System.Length);

                    var expectPublicIdentifier = success && StringReference.AsciiCaseInsensitiveEquals(result, StringReference.Public);
                    var expectSystemIdentifier = success && StringReference.AsciiCaseInsensitiveEquals(result, StringReference.System);
        
                    if (expectPublicIdentifier)
                    {
                        _streamConsumer.ConsumeChar(StringReference.System.Length);
                        return GetDocTypeTokenInAfterPublicKeywordState();
                    }
        
                    if (expectSystemIdentifier)
                    {
                        _streamConsumer.ConsumeChar(StringReference.System.Length);
                        return GetDocTypeTokenInAfterSystemKeywordState();
                    }

                    _docTypeTokenBuilder.SetForceQuirks();
                    return GetDocTypeTokenInBogusState();
            }
        }
    }

    private HtmlToken GetDocTypeTokenInAfterPublicKeywordState()
    {
        while (true)
        {
            var (success, character) = _streamConsumer.TryGetCurrentChar();

            // EOF
            if (!success)
                return _docTypeTokenBuilder.SetForceQuirks().Build();

            switch (character)
            {
                case CharacterReference.CharacterTabulation:
                case CharacterReference.LineFeed:
                case CharacterReference.FormFeed:
                case CharacterReference.Space:
                    _streamConsumer.ConsumeChar();
                    return GetDocTypeTokenInBeforePublicIdentifierState();
                case CharacterReference.QuotationMark:
                    _streamConsumer.ConsumeChar();
                    _docTypeTokenBuilder.SetPublicIdentifierPresent();
                    return GetDocTypeTokenInPublicIdentifierDoubleQuotedState();
                case CharacterReference.Apostrophe:
                    _streamConsumer.ConsumeChar();
                    _docTypeTokenBuilder.SetPublicIdentifierPresent();
                    return GetDocTypeTokenInPublicIdentifierSingleQuotedState();
                case CharacterReference.GreaterThanSign:
                    _streamConsumer.ConsumeChar();
                    return _docTypeTokenBuilder.SetForceQuirks().Build();
                default:
                    _docTypeTokenBuilder.SetForceQuirks();
                    return GetDocTypeTokenInBogusState();
            }
        }
    }

    private HtmlToken GetDocTypeTokenInBeforePublicIdentifierState()
    {
        while (true)
        {
            var (success, character) = _streamConsumer.TryGetCurrentChar();

            // EOF
            if (!success)
                return _docTypeTokenBuilder.SetForceQuirks().Build();

            switch (character)
            {
                case CharacterReference.CharacterTabulation:
                case CharacterReference.LineFeed:
                case CharacterReference.FormFeed:
                case CharacterReference.Space:
                    _streamConsumer.ConsumeChar();
                    break;
                case CharacterReference.QuotationMark:
                    _streamConsumer.ConsumeChar();
                    _docTypeTokenBuilder.SetPublicIdentifierPresent();
                    return GetDocTypeTokenInPublicIdentifierDoubleQuotedState();
                case CharacterReference.Apostrophe:
                    _streamConsumer.ConsumeChar();
                    _docTypeTokenBuilder.SetPublicIdentifierPresent();
                    return GetDocTypeTokenInPublicIdentifierSingleQuotedState();
                case CharacterReference.GreaterThanSign:
                    _streamConsumer.ConsumeChar();
                    return _docTypeTokenBuilder.SetForceQuirks().Build();
                default:
                    _docTypeTokenBuilder.SetForceQuirks();
                    return GetDocTypeTokenInBogusState();
            }
        }
    }

    private HtmlToken GetDocTypeTokenInPublicIdentifierDoubleQuotedState()
    {
        while (true)
        {
            var (success, character) = _streamConsumer.TryGetCurrentChar();

            // EOF
            if (!success)
                return _docTypeTokenBuilder.SetForceQuirks().Build();

            switch (character)
            {
                case CharacterReference.QuotationMark:
                    _streamConsumer.ConsumeChar();
                    return GetDocTypeTokenInAfterPublicIdentifierState();
                case CharacterReference.Null:
                    _streamConsumer.ConsumeChar();
                    _docTypeTokenBuilder.AppendToPublicIdentifier(CharacterReference.ReplacementCharacter);
                    break;
                case CharacterReference.GreaterThanSign:
                    _streamConsumer.ConsumeChar();
                    return _docTypeTokenBuilder.SetForceQuirks().Build();
                default:
                    _streamConsumer.ConsumeChar();
                    _docTypeTokenBuilder.AppendToPublicIdentifier(character);
                    break;
            }
        }
    }

    private HtmlToken GetDocTypeTokenInPublicIdentifierSingleQuotedState()
    {
        while (true)
        {
            var (success, character) = _streamConsumer.TryGetCurrentChar();

            // EOF
            if (!success)
                return _docTypeTokenBuilder.SetForceQuirks().Build();

            switch (character)
            {
                case CharacterReference.Apostrophe:
                    _streamConsumer.ConsumeChar();
                    return GetDocTypeTokenInAfterPublicIdentifierState();
                case CharacterReference.Null:
                    _streamConsumer.ConsumeChar();
                    _docTypeTokenBuilder.AppendToPublicIdentifier(CharacterReference.ReplacementCharacter);
                    break;
                case CharacterReference.GreaterThanSign:
                    _streamConsumer.ConsumeChar();
                    return _docTypeTokenBuilder.SetForceQuirks().Build();
                default:
                    _streamConsumer.ConsumeChar();
                    _docTypeTokenBuilder.AppendToPublicIdentifier(character);
                    break;
            }
        }
    }

    private HtmlToken GetDocTypeTokenInAfterPublicIdentifierState()
    {
        while (true)
        {
            var (success, character) = _streamConsumer.TryGetCurrentChar();

            // EOF
            if (!success)
                return _docTypeTokenBuilder.SetForceQuirks().Build();

            switch (character)
            {
                case CharacterReference.CharacterTabulation:
                case CharacterReference.LineFeed:
                case CharacterReference.FormFeed:
                case CharacterReference.Space:
                    _streamConsumer.ConsumeChar();
                    return GetDocTypeTokenInBetweenPublicAndSystemIdentifiersState();
                case CharacterReference.GreaterThanSign:
                    _streamConsumer.ConsumeChar();
                    return _docTypeTokenBuilder.Build();
                case CharacterReference.QuotationMark:
                    _streamConsumer.ConsumeChar();
                    _docTypeTokenBuilder.SetSystemIdentifierPresent();
                    return GetDocTypeTokenInSystemIdentifierDoubleQuotedState();
                case CharacterReference.Apostrophe:
                    _streamConsumer.ConsumeChar();
                    _docTypeTokenBuilder.SetSystemIdentifierPresent();
                    return GetDocTypeTokenInSystemIdentifierSingleQuotedState();
                default:
                    _docTypeTokenBuilder.SetForceQuirks();
                    return GetDocTypeTokenInBogusState();
            }
        }
    }

    private HtmlToken GetDocTypeTokenInBetweenPublicAndSystemIdentifiersState()
    {
        while (true)
        {
            var (success, character) = _streamConsumer.TryGetCurrentChar();

            // EOF
            if (!success)
                return _docTypeTokenBuilder.SetForceQuirks().Build();

            switch (character)
            {
                case CharacterReference.CharacterTabulation:
                case CharacterReference.LineFeed:
                case CharacterReference.FormFeed:
                case CharacterReference.Space:
                    _streamConsumer.ConsumeChar();
                    break;
                case CharacterReference.GreaterThanSign:
                    _streamConsumer.ConsumeChar();
                    return _docTypeTokenBuilder.Build();
                case CharacterReference.QuotationMark:
                    _streamConsumer.ConsumeChar();
                    _docTypeTokenBuilder.SetSystemIdentifierPresent();
                    return GetDocTypeTokenInSystemIdentifierDoubleQuotedState();
                case CharacterReference.Apostrophe:
                    _streamConsumer.ConsumeChar();
                    _docTypeTokenBuilder.SetSystemIdentifierPresent();
                    return GetDocTypeTokenInSystemIdentifierSingleQuotedState();
                default:
                    _docTypeTokenBuilder.SetForceQuirks();
                    return GetDocTypeTokenInBogusState();
            }
        }
    }

    private HtmlToken GetDocTypeTokenInAfterSystemKeywordState()
    {
        while (true)
        {
            var (success, character) = _streamConsumer.TryGetCurrentChar();

            // EOF
            if (!success)
                return _docTypeTokenBuilder.SetForceQuirks().Build();

            switch (character)
            {
                case CharacterReference.CharacterTabulation:
                case CharacterReference.LineFeed:
                case CharacterReference.FormFeed:
                case CharacterReference.Space:
                    _streamConsumer.ConsumeChar();
                    return GetDocTypeTokenInBeforeSystemIdentifierState();
                case CharacterReference.QuotationMark:
                    _streamConsumer.ConsumeChar();
                    _docTypeTokenBuilder.SetSystemIdentifierPresent();
                    return GetDocTypeTokenInSystemIdentifierDoubleQuotedState();
                case CharacterReference.Apostrophe:
                    _streamConsumer.ConsumeChar();
                    _docTypeTokenBuilder.SetSystemIdentifierPresent();
                    return GetDocTypeTokenInSystemIdentifierSingleQuotedState();
                case CharacterReference.GreaterThanSign:
                    _streamConsumer.ConsumeChar();
                    return _docTypeTokenBuilder.SetForceQuirks().Build();
                default:
                    _docTypeTokenBuilder.SetForceQuirks();
                    return GetDocTypeTokenInBogusState();
            }
        }
    }

    private HtmlToken GetDocTypeTokenInBeforeSystemIdentifierState()
    {
        while (true)
        {
            var (success, character) = _streamConsumer.TryGetCurrentChar();

            // EOF
            if (!success)
                return _docTypeTokenBuilder.SetForceQuirks().Build();

            switch (character)
            {
                case CharacterReference.CharacterTabulation:
                case CharacterReference.LineFeed:
                case CharacterReference.FormFeed:
                case CharacterReference.Space:
                    _streamConsumer.ConsumeChar();
                    break;
                case CharacterReference.QuotationMark:
                    _streamConsumer.ConsumeChar();
                    _docTypeTokenBuilder.SetSystemIdentifierPresent();
                    return GetDocTypeTokenInSystemIdentifierDoubleQuotedState();
                case CharacterReference.Apostrophe:
                    _streamConsumer.ConsumeChar();
                    _docTypeTokenBuilder.SetSystemIdentifierPresent();
                    return GetDocTypeTokenInSystemIdentifierSingleQuotedState();
                case CharacterReference.GreaterThanSign:
                    _streamConsumer.ConsumeChar();
                    return _docTypeTokenBuilder.SetForceQuirks().Build();
                default:
                    _docTypeTokenBuilder.SetForceQuirks();
                    return GetDocTypeTokenInBogusState();
            }
        }
    }

    private HtmlToken GetDocTypeTokenInSystemIdentifierDoubleQuotedState()
    {
        while (true)
        {
            var (success, character) = _streamConsumer.TryGetCurrentChar();

            // EOF
            if (!success)
                return _docTypeTokenBuilder.SetForceQuirks().Build();

            switch (character)
            {
                case CharacterReference.QuotationMark:
                    _streamConsumer.ConsumeChar();
                    return GetDocTypeTokenInAfterSystemIdentifierState();
                case CharacterReference.Null:
                    _streamConsumer.ConsumeChar();
                    _docTypeTokenBuilder.AppendToSystemIdentifier(CharacterReference.ReplacementCharacter);
                    break;
                case CharacterReference.GreaterThanSign:
                    _streamConsumer.ConsumeChar();
                    return _docTypeTokenBuilder.SetForceQuirks().Build();
                default:
                    _streamConsumer.ConsumeChar();
                    _docTypeTokenBuilder.AppendToSystemIdentifier(character);
                    break;
            }
        }
    }

    private HtmlToken GetDocTypeTokenInSystemIdentifierSingleQuotedState()
    {
        while (true)
        {
            var (success, character) = _streamConsumer.TryGetCurrentChar();

            // EOF
            if (!success)
                return _docTypeTokenBuilder.SetForceQuirks().Build();

            switch (character)
            {
                case CharacterReference.Apostrophe:
                    _streamConsumer.ConsumeChar();
                    return GetDocTypeTokenInAfterSystemIdentifierState();
                case CharacterReference.Null:
                    _streamConsumer.ConsumeChar();
                    _docTypeTokenBuilder.AppendToSystemIdentifier(CharacterReference.ReplacementCharacter);
                    break;
                case CharacterReference.GreaterThanSign:
                    _streamConsumer.ConsumeChar();
                    return _docTypeTokenBuilder.SetForceQuirks().Build();
                default:
                    _streamConsumer.ConsumeChar();
                    _docTypeTokenBuilder.AppendToSystemIdentifier(character);
                    break;
            }
        }
    }

    private HtmlToken GetDocTypeTokenInAfterSystemIdentifierState()
    {
        while (true)
        {
            var (success, character) = _streamConsumer.TryGetCurrentChar();

            // EOF
            if (!success)
                return _docTypeTokenBuilder.SetForceQuirks().Build();

            switch (character)
            {
                case CharacterReference.CharacterTabulation:
                case CharacterReference.LineFeed:
                case CharacterReference.FormFeed:
                case CharacterReference.Space:
                    _streamConsumer.ConsumeChar();
                    break;
                case CharacterReference.GreaterThanSign:
                    _streamConsumer.ConsumeChar();
                    return _docTypeTokenBuilder.Build();
                default:
                    return GetDocTypeTokenInBogusState();
            }
        }
    }

    private HtmlToken GetDocTypeTokenInBogusState()
    {
        while (true)
        {
            var (success, character) = _streamConsumer.TryGetCurrentChar();
            
            // EOF
            if (!success)
                return _docTypeTokenBuilder.Build();

            switch (character)
            {
                case CharacterReference.GreaterThanSign:
                    _streamConsumer.ConsumeChar();
                    return _docTypeTokenBuilder.Build();
                case CharacterReference.Null:
                    _streamConsumer.ConsumeChar();
                    break;
                default:
                    _streamConsumer.ConsumeChar();
                    break;
            }
        }
    }
}