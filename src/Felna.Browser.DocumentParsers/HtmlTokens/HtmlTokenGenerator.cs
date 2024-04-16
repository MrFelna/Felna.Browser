using System.Text;
using Felna.Browser.DocumentParsers.StreamConsumers;
using Felna.Browser.DocumentParsers.TextReferences;

namespace Felna.Browser.DocumentParsers.HtmlTokens;

internal class HtmlTokenGenerator
{
    private readonly IStreamConsumer _streamConsumer;
    private TokenParserState _parserState;
    
    private StringBuilder _commentDataBuilder = new StringBuilder();
    private CommentToken? _commentToken;

    private DocTypeTokenBuilder _docTypeTokenBuilder = new DocTypeTokenBuilder();
    private DocTypeToken? _docTypeToken;

    internal HtmlTokenGenerator(IStreamConsumer streamConsumer)
    {
        _streamConsumer = streamConsumer;
        _parserState = TokenParserState.Data;
    }

    internal HtmlToken GetNextToken()
    {
        HtmlToken? token = null;

        do
        {
            token = _parserState switch
            {
                TokenParserState.Data => GetTokenFrom01DataState(),
                TokenParserState.TagOpen => GetTokenFrom06TagOpenState(),
                TokenParserState.BogusComment => GetTokenFrom41BogusCommentState(),
                TokenParserState.MarkupDeclarationOpen => GetTokenFrom42MarkupDeclarationOpenState(),
                TokenParserState.CommentStart => GetTokenFrom43CommentStartState(),
                TokenParserState.CommentStartDash => GetTokenFrom44CommentStartDashState(),
                TokenParserState.Comment => GetTokenFrom45CommentState(),
                TokenParserState.CommentLessThanSign => GetTokenFrom46CommentLessThanSignState(),
                TokenParserState.CommentLessThanSignBang => GetTokenFrom47CommentLessThanSignBangState(),
                TokenParserState.CommentLessThanSignBangDash => GetTokenFrom48CommentLessThanSignBangDashState(),
                TokenParserState.CommentLessThanSignBangDashDash => GetTokenFrom49CommentLessThanSignBangDashDashState(),
                TokenParserState.CommentEndDash => GetTokenFrom50CommentEndDashState(),
                TokenParserState.CommentEnd => GetTokenFrom51CommentEndState(),
                TokenParserState.CommentEndBang => GetTokenFrom52CommentEndBangState(),
                TokenParserState.Doctype => GetTokenFrom53DoctypeState(),
                TokenParserState.BeforeDoctypeName => GetTokenFrom54BeforeDoctypeNameState(),
                TokenParserState.DoctypeName => GetTokenFrom55DoctypeNameState(),
                TokenParserState.AfterDoctypeName => GetTokenFrom56AfterDoctypeNameState(),
                TokenParserState.AfterDoctypePublicKeyword => GetTokenFrom57AfterDoctypePublicKeywordState(),
                TokenParserState.BeforeDoctypePublicIdentifier => GetTokenFrom58BeforeDoctypePublicIdentifierState(),
                TokenParserState.DoctypePublicIdentifierDoubleQuoted => GetTokenFrom59DoctypePublicIdentifierDoubleQuotedState(),
                TokenParserState.DoctypePublicIdentifierSingleQuoted => GetTokenFrom60DoctypePublicIdentifierSingleQuotedState(),
                TokenParserState.AfterDoctypePublicIdentifier => GetTokenFrom61AfterDoctypePublicIdentifierState(),
                TokenParserState.BetweenDoctypePublicAndSystemIdentifier => GetTokenFrom62BetweenDoctypePublicAndSystemIdentifiersState(),
                TokenParserState.AfterDoctypeSystemKeyword => GetTokenFrom63AfterDoctypeSystemKeywordState(),
                TokenParserState.BeforeDoctypeSystemIdentifier => GetTokenFrom64BeforeDoctypeSystemIdentifierState(),
                TokenParserState.DoctypeSystemIdentifierDoubleQuoted => GetTokenFrom65DoctypeSystemIdentifierDoubleQuotedState(),
                TokenParserState.DoctypeSystemIdentifierSingleQuoted => GetTokenFrom66DoctypeSystemIdentifierSingleQuotedState(),
                TokenParserState.AfterDoctypeSystemIdentifier => GetTokenFrom67AfterDoctypeSystemIdentifierState(),
                TokenParserState.BogusDoctype => GetTokenFrom68BogusDoctypeState(),
                _ => throw new NotImplementedException()
            };
        } while (token is null);

        return token;
    }

    private HtmlToken? GetTokenFrom01DataState()
    {
        var (success, character) = _streamConsumer.TryGetCurrentChar();

        if (!success)
            return new EndOfFileToken();
        
        switch (character)
        {
            case CharacterReference.Ampersand:
                throw new NotImplementedException();
            case CharacterReference.LessThanSign:
                _streamConsumer.ConsumeChar();
                _parserState = TokenParserState.TagOpen;
                return null;
            case CharacterReference.Null:
                throw new NotImplementedException();
            default:
                throw new NotImplementedException();
        }
    }

    private HtmlToken? GetTokenFrom06TagOpenState()
    {
        var (success, character) = _streamConsumer.TryGetCurrentChar();

        if (!success)
        {
            _parserState = TokenParserState.Data; // so the next token is EOF
            return new CharacterToken{ Data = CharacterReference.LessThanSign.ToString() };
        }
        
        switch (character)
        {
            case CharacterReference.ExclamationMark:
                _streamConsumer.ConsumeChar();
                _parserState = TokenParserState.MarkupDeclarationOpen;
                return null;
            case CharacterReference.Solidus:
                throw new NotImplementedException();
            case var _ when CharacterRangeReference.AsciiAlpha.Contains(character):
                throw new NotImplementedException();
            case CharacterReference.QuestionMark:
                CreateNewCommentToken();
                _parserState = TokenParserState.BogusComment;
                return null;
            case CharacterReference.Null:
                throw new NotImplementedException();
            default:
                throw new NotImplementedException();
        }
    }

    private HtmlToken? GetTokenFrom41BogusCommentState()
    {
        var (success, character) = _streamConsumer.TryGetCurrentChar();

        if (!success)
        {
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetCommentToken();
        }
        
        switch (character)
        {
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeChar();
                _parserState = TokenParserState.Data;
                return GetCommentToken();
            case CharacterReference.Null:
                _streamConsumer.ConsumeChar();
                _commentDataBuilder.Append(CharacterReference.ReplacementCharacter);
                return null;
            default:
                _streamConsumer.ConsumeChar();
                _commentDataBuilder.Append(character);
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom42MarkupDeclarationOpenState()
    {
        var (success, result) = _streamConsumer.LookAhead(StringReference.DoubleHyphen.Length);

        if (success)
        {
            if (result == StringReference.DoubleHyphen)
            {
                _streamConsumer.ConsumeChar(StringReference.DoubleHyphen.Length);
                CreateNewCommentToken();
                _parserState = TokenParserState.CommentStart;
                return null;
            }

            (success, result) = _streamConsumer.LookAhead(StringReference.DocType.Length);

            if (success)
            {
                if (StringReference.AsciiCaseInsensitiveEquals(result, StringReference.DocType))
                {
                    _streamConsumer.ConsumeChar(StringReference.DocType.Length);
                    _parserState = TokenParserState.Doctype;
                    return null;
                }
                
                if (result == "[CDATA[")
                {
                    throw new NotImplementedException();
                }
            }
        }

        CreateNewCommentToken();
        _parserState = TokenParserState.BogusComment;
        return null;
    }

    private HtmlToken? GetTokenFrom43CommentStartState()
    {
        var (success, character) = _streamConsumer.TryGetCurrentChar();

        if (!success)
        {
            _parserState = TokenParserState.Comment;
            return null;
        }
        
        switch (character)
        {
            case CharacterReference.HyphenMinus:
                _streamConsumer.ConsumeChar();
                _parserState = TokenParserState.CommentStartDash;
                return null;
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeChar();
                _parserState = TokenParserState.Data;
                return GetCommentToken();
            default:
                _parserState = TokenParserState.Comment;
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom44CommentStartDashState()
    {
        var (success, character) = _streamConsumer.TryGetCurrentChar();

        if (!success)
        {
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetCommentToken();
        }
        
        switch (character)
        {
            case CharacterReference.HyphenMinus:
                _streamConsumer.ConsumeChar();
                _parserState = TokenParserState.CommentEnd;
                return null;
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeChar();
                _parserState = TokenParserState.Data;
                return GetCommentToken();
            default:
                _commentDataBuilder.Append(CharacterReference.HyphenMinus);
                _parserState = TokenParserState.Comment;
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom45CommentState()
    {
        var (success, character) = _streamConsumer.TryGetCurrentChar();

        if (!success)
        {
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetCommentToken();
        }
        
        switch (character)
        {
            case CharacterReference.LessThanSign:
                _streamConsumer.ConsumeChar();
                _commentDataBuilder.Append(character);
                _parserState = TokenParserState.CommentLessThanSign;
                return null;
            case CharacterReference.HyphenMinus:
                _streamConsumer.ConsumeChar();
                _parserState = TokenParserState.CommentEndDash;
                return null;
            case CharacterReference.Null:
                _streamConsumer.ConsumeChar();
                _commentDataBuilder.Append(CharacterReference.ReplacementCharacter);
                return null;
            default:
                _streamConsumer.ConsumeChar();
                _commentDataBuilder.Append(character);
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom46CommentLessThanSignState()
    {
        var (success, character) = _streamConsumer.TryGetCurrentChar();

        if (!success)
        {
            _parserState = TokenParserState.Comment;
            return null;
        }
        
        switch (character)
        {
            case CharacterReference.ExclamationMark:
                _streamConsumer.ConsumeChar();
                _commentDataBuilder.Append(character);
                _parserState = TokenParserState.CommentLessThanSignBang;
                return null;
            case CharacterReference.LessThanSign:
                _streamConsumer.ConsumeChar();
                _commentDataBuilder.Append(character);
                return null;
            default:
                _parserState = TokenParserState.Comment;
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom47CommentLessThanSignBangState()
    {
        var (success, character) = _streamConsumer.TryGetCurrentChar();

        if (!success)
        {
            _parserState = TokenParserState.Comment;
            return null;
        }
        
        switch (character)
        {
            case CharacterReference.HyphenMinus:
                _streamConsumer.ConsumeChar();
                _parserState = TokenParserState.CommentLessThanSignBangDash;
                return null;
            default:
                _parserState = TokenParserState.Comment;
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom48CommentLessThanSignBangDashState()
    {
        var (success, character) = _streamConsumer.TryGetCurrentChar();

        if (!success)
        {
            _parserState = TokenParserState.CommentEndDash;
            return null;
        }
        
        switch (character)
        {
            case CharacterReference.HyphenMinus:
                _streamConsumer.ConsumeChar();
                _parserState = TokenParserState.CommentLessThanSignBangDashDash;
                return null;
            default:
                _parserState = TokenParserState.CommentEndDash;
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom49CommentLessThanSignBangDashDashState()
    {
        _parserState = TokenParserState.CommentEnd;
        return null;
    }
    
    private HtmlToken? GetTokenFrom50CommentEndDashState()
    {
        var (success, character) = _streamConsumer.TryGetCurrentChar();

        if (!success)
        {
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetCommentToken();
        }
        
        switch (character)
        {
            case CharacterReference.HyphenMinus:
                _streamConsumer.ConsumeChar();
                _parserState = TokenParserState.CommentEnd;
                return null;
            default:
                _commentDataBuilder.Append(CharacterReference.HyphenMinus);
                _parserState = TokenParserState.Comment;
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom51CommentEndState()
    {
        var (success, character) = _streamConsumer.TryGetCurrentChar();

        if (!success)
        {
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetCommentToken();
        }
        
        switch (character)
        {
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeChar();
                _parserState = TokenParserState.Data;
                return GetCommentToken();
            case CharacterReference.ExclamationMark:
                _streamConsumer.ConsumeChar();
                _parserState = TokenParserState.CommentEndBang;
                return null;
            case CharacterReference.HyphenMinus:
                _streamConsumer.ConsumeChar();
                _commentDataBuilder.Append(CharacterReference.HyphenMinus);
                return null;
            default:
                _commentDataBuilder.Append(CharacterReference.HyphenMinus);
                _commentDataBuilder.Append(CharacterReference.HyphenMinus);
                _parserState = TokenParserState.Comment;
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom52CommentEndBangState()
    {
        var (success, character) = _streamConsumer.TryGetCurrentChar();

        if (!success)
        {
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetCommentToken();
        }
        
        switch (character)
        {
            case CharacterReference.HyphenMinus:
                _streamConsumer.ConsumeChar();
                _commentDataBuilder.Append(CharacterReference.HyphenMinus);
                _commentDataBuilder.Append(CharacterReference.HyphenMinus);
                _commentDataBuilder.Append(CharacterReference.ExclamationMark);
                _parserState = TokenParserState.CommentEndDash;
                return null;
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeChar();
                _parserState = TokenParserState.Data;
                return GetCommentToken();
            default:
                _commentDataBuilder.Append(CharacterReference.HyphenMinus);
                _commentDataBuilder.Append(CharacterReference.HyphenMinus);
                _commentDataBuilder.Append(CharacterReference.ExclamationMark);
                _parserState = TokenParserState.Comment;
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom53DoctypeState()
    {
        var (success, character) = _streamConsumer.TryGetCurrentChar();

        if (!success)
        {
            CreateNewDoctypeToken();
            _docTypeTokenBuilder.SetForceQuirks();
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetDoctypeToken();
        }
        
        switch (character)
        {
            case CharacterReference.CharacterTabulation:
            case CharacterReference.LineFeed:
            case CharacterReference.FormFeed:
            case CharacterReference.Space:
                _streamConsumer.ConsumeChar();
                _parserState = TokenParserState.BeforeDoctypeName;
                return null;
            case CharacterReference.GreaterThanSign:
                _parserState = TokenParserState.BeforeDoctypeName;
                return null;
            default:
                _parserState = TokenParserState.BeforeDoctypeName;
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom54BeforeDoctypeNameState()
    {
        var (success, character) = _streamConsumer.TryGetCurrentChar();

        if (!success)
        {
            CreateNewDoctypeToken();
            _docTypeTokenBuilder.SetForceQuirks();
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetDoctypeToken();
        }
        
        switch (character)
        {
            case CharacterReference.CharacterTabulation:
            case CharacterReference.LineFeed:
            case CharacterReference.FormFeed:
            case CharacterReference.Space:
                _streamConsumer.ConsumeChar();
                return null;
            case var _ when CharacterRangeReference.AsciiUpperAlpha.Contains(character):
                _streamConsumer.ConsumeChar();
                CreateNewDoctypeToken();
                _docTypeTokenBuilder.AppendToName(CharacterReference.ToAsciiLower(character));
                _parserState = TokenParserState.DoctypeName;
                return null;
            case CharacterReference.Null:
                _streamConsumer.ConsumeChar();
                CreateNewDoctypeToken();
                _docTypeTokenBuilder.AppendToName(CharacterReference.ReplacementCharacter);
                _parserState = TokenParserState.DoctypeName;
                return null;
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeChar();
                CreateNewDoctypeToken();
                _docTypeTokenBuilder.SetForceQuirks();
                _parserState = TokenParserState.Data;
                return GetDoctypeToken();
            default:
                _streamConsumer.ConsumeChar();
                CreateNewDoctypeToken();
                _docTypeTokenBuilder.AppendToName(character);
                _parserState = TokenParserState.DoctypeName;
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom55DoctypeNameState()
    {
        var (success, character) = _streamConsumer.TryGetCurrentChar();

        if (!success)
        {
            _docTypeTokenBuilder.SetForceQuirks();
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetDoctypeToken();
        }
        
        switch (character)
        {
            case CharacterReference.CharacterTabulation:
            case CharacterReference.LineFeed:
            case CharacterReference.FormFeed:
            case CharacterReference.Space:
                _streamConsumer.ConsumeChar();
                _parserState = TokenParserState.AfterDoctypeName;
                return null;
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeChar();
                _parserState = TokenParserState.Data;
                return GetDoctypeToken();
            case var _ when CharacterRangeReference.AsciiUpperAlpha.Contains(character):
                _streamConsumer.ConsumeChar();
                _docTypeTokenBuilder.AppendToName(CharacterReference.ToAsciiLower(character));
                return null;
            case CharacterReference.Null:
                _streamConsumer.ConsumeChar();
                _docTypeTokenBuilder.AppendToName(CharacterReference.ReplacementCharacter);
                return null;
            default:
                _streamConsumer.ConsumeChar();
                _docTypeTokenBuilder.AppendToName(character);
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom56AfterDoctypeNameState()
    {
        var (success, character) = _streamConsumer.TryGetCurrentChar();

        if (!success)
        {
            _docTypeTokenBuilder.SetForceQuirks();
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetDoctypeToken();
        }
        
        switch (character)
        {
            case CharacterReference.CharacterTabulation:
            case CharacterReference.LineFeed:
            case CharacterReference.FormFeed:
            case CharacterReference.Space:
                _streamConsumer.ConsumeChar();
                return null;
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeChar();
                _parserState = TokenParserState.Data;
                return GetDoctypeToken();
            default:

                (success, var result) = _streamConsumer.LookAhead(StringReference.Public.Length);

                if (success)
                {
                    if (StringReference.AsciiCaseInsensitiveEquals(result, StringReference.Public))
                    {
                        _streamConsumer.ConsumeChar(StringReference.Public.Length);
                        _parserState = TokenParserState.AfterDoctypePublicKeyword;
                        return null;
                    }
                    
                    if (StringReference.AsciiCaseInsensitiveEquals(result, StringReference.System))
                    {
                        _streamConsumer.ConsumeChar(StringReference.System.Length);
                        _parserState = TokenParserState.AfterDoctypeSystemKeyword;
                        return null;
                    }
                }

                _docTypeTokenBuilder.SetForceQuirks();
                _parserState = TokenParserState.BogusDoctype;
                
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom57AfterDoctypePublicKeywordState()
    {
        var (success, character) = _streamConsumer.TryGetCurrentChar();

        if (!success)
        {
            _docTypeTokenBuilder.SetForceQuirks();
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetDoctypeToken();
        }
        
        switch (character)
        {
            case CharacterReference.CharacterTabulation:
            case CharacterReference.LineFeed:
            case CharacterReference.FormFeed:
            case CharacterReference.Space:
                _streamConsumer.ConsumeChar();
                _parserState = TokenParserState.BeforeDoctypePublicIdentifier;
                return null;
            case CharacterReference.QuotationMark:
                _streamConsumer.ConsumeChar();
                _docTypeTokenBuilder.SetPublicIdentifierPresent();
                _parserState = TokenParserState.DoctypePublicIdentifierDoubleQuoted;
                return null;
            case CharacterReference.Apostrophe:
                _streamConsumer.ConsumeChar();
                _docTypeTokenBuilder.SetPublicIdentifierPresent();
                _parserState = TokenParserState.DoctypePublicIdentifierSingleQuoted;
                return null;
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeChar();
                _docTypeTokenBuilder.SetForceQuirks();
                _parserState = TokenParserState.Data;
                return GetDoctypeToken();
            default:
                _docTypeTokenBuilder.SetForceQuirks();
                _parserState = TokenParserState.BogusDoctype;
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom58BeforeDoctypePublicIdentifierState()
    {
        var (success, character) = _streamConsumer.TryGetCurrentChar();

        if (!success)
        {
            _docTypeTokenBuilder.SetForceQuirks();
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetDoctypeToken();
        }
        
        switch (character)
        {
            case CharacterReference.CharacterTabulation:
            case CharacterReference.LineFeed:
            case CharacterReference.FormFeed:
            case CharacterReference.Space:
                _streamConsumer.ConsumeChar();
                return null;
            case CharacterReference.QuotationMark:
                _streamConsumer.ConsumeChar();
                _docTypeTokenBuilder.SetPublicIdentifierPresent();
                _parserState = TokenParserState.DoctypePublicIdentifierDoubleQuoted;
                return null;
            case CharacterReference.Apostrophe:
                _streamConsumer.ConsumeChar();
                _docTypeTokenBuilder.SetPublicIdentifierPresent();
                _parserState = TokenParserState.DoctypePublicIdentifierSingleQuoted;
                return null;
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeChar();
                _docTypeTokenBuilder.SetForceQuirks();
                _parserState = TokenParserState.Data;
                return GetDoctypeToken();
            default:
                _docTypeTokenBuilder.SetForceQuirks();
                _parserState = TokenParserState.BogusDoctype;
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom59DoctypePublicIdentifierDoubleQuotedState()
    {
        var (success, character) = _streamConsumer.TryGetCurrentChar();

        if (!success)
        {
            _docTypeTokenBuilder.SetForceQuirks();
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetDoctypeToken();
        }
        
        switch (character)
        {
            case CharacterReference.QuotationMark:
                _streamConsumer.ConsumeChar();
                _parserState = TokenParserState.AfterDoctypePublicIdentifier;
                return null;
            case CharacterReference.Null:
                _streamConsumer.ConsumeChar();
                _docTypeTokenBuilder.AppendToPublicIdentifier(CharacterReference.ReplacementCharacter);
                return null;
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeChar();
                _docTypeTokenBuilder.SetForceQuirks();
                _parserState = TokenParserState.Data;
                return GetDoctypeToken();
            default:
                _streamConsumer.ConsumeChar();
                _docTypeTokenBuilder.AppendToPublicIdentifier(character);
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom60DoctypePublicIdentifierSingleQuotedState()
    {
        var (success, character) = _streamConsumer.TryGetCurrentChar();

        if (!success)
        {
            _docTypeTokenBuilder.SetForceQuirks();
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetDoctypeToken();
        }
        
        switch (character)
        {
            case CharacterReference.Apostrophe:
                _streamConsumer.ConsumeChar();
                _parserState = TokenParserState.AfterDoctypePublicIdentifier;
                return null;
            case CharacterReference.Null:
                _streamConsumer.ConsumeChar();
                _docTypeTokenBuilder.AppendToPublicIdentifier(CharacterReference.ReplacementCharacter);
                return null;
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeChar();
                _docTypeTokenBuilder.SetForceQuirks();
                _parserState = TokenParserState.Data;
                return GetDoctypeToken();
            default:
                _streamConsumer.ConsumeChar();
                _docTypeTokenBuilder.AppendToPublicIdentifier(character);
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom61AfterDoctypePublicIdentifierState()
    {
        var (success, character) = _streamConsumer.TryGetCurrentChar();

        if (!success)
        {
            _docTypeTokenBuilder.SetForceQuirks();
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetDoctypeToken();
        }
        
        switch (character)
        {
            case CharacterReference.CharacterTabulation:
            case CharacterReference.LineFeed:
            case CharacterReference.FormFeed:
            case CharacterReference.Space:
                _streamConsumer.ConsumeChar();
                _parserState = TokenParserState.BetweenDoctypePublicAndSystemIdentifier;
                return null;
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeChar();
                _parserState = TokenParserState.Data;
                return GetDoctypeToken();
            case CharacterReference.QuotationMark:
                _streamConsumer.ConsumeChar();
                _docTypeTokenBuilder.SetSystemIdentifierPresent();
                _parserState = TokenParserState.DoctypeSystemIdentifierDoubleQuoted;
                return null;
            case CharacterReference.Apostrophe:
                _streamConsumer.ConsumeChar();
                _docTypeTokenBuilder.SetSystemIdentifierPresent();
                _parserState = TokenParserState.DoctypeSystemIdentifierSingleQuoted;
                return null;
            default:
                _docTypeTokenBuilder.SetForceQuirks();
                _parserState = TokenParserState.BogusDoctype;
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom62BetweenDoctypePublicAndSystemIdentifiersState()
    {
        var (success, character) = _streamConsumer.TryGetCurrentChar();

        if (!success)
        {
            _docTypeTokenBuilder.SetForceQuirks();
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetDoctypeToken();
        }
        
        switch (character)
        {
            case CharacterReference.CharacterTabulation:
            case CharacterReference.LineFeed:
            case CharacterReference.FormFeed:
            case CharacterReference.Space:
                _streamConsumer.ConsumeChar();
                return null;
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeChar();
                _parserState = TokenParserState.Data;
                return GetDoctypeToken();
            case CharacterReference.QuotationMark:
                _streamConsumer.ConsumeChar();
                _docTypeTokenBuilder.SetSystemIdentifierPresent();
                _parserState = TokenParserState.DoctypeSystemIdentifierDoubleQuoted;
                return null;
            case CharacterReference.Apostrophe:
                _streamConsumer.ConsumeChar();
                _docTypeTokenBuilder.SetSystemIdentifierPresent();
                _parserState = TokenParserState.DoctypeSystemIdentifierSingleQuoted;
                return null;
            default:
                _docTypeTokenBuilder.SetForceQuirks();
                _parserState = TokenParserState.BogusDoctype;
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom63AfterDoctypeSystemKeywordState()
    {
        var (success, character) = _streamConsumer.TryGetCurrentChar();

        if (!success)
        {
            _docTypeTokenBuilder.SetForceQuirks();
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetDoctypeToken();
        }
        
        switch (character)
        {
            case CharacterReference.CharacterTabulation:
            case CharacterReference.LineFeed:
            case CharacterReference.FormFeed:
            case CharacterReference.Space:
                _streamConsumer.ConsumeChar();
                _parserState = TokenParserState.BeforeDoctypeSystemIdentifier;
                return null;
            case CharacterReference.QuotationMark:
                _streamConsumer.ConsumeChar();
                _docTypeTokenBuilder.SetSystemIdentifierPresent();
                _parserState = TokenParserState.DoctypeSystemIdentifierDoubleQuoted;
                return null;
            case CharacterReference.Apostrophe:
                _streamConsumer.ConsumeChar();
                _docTypeTokenBuilder.SetSystemIdentifierPresent();
                _parserState = TokenParserState.DoctypeSystemIdentifierSingleQuoted;
                return null;
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeChar();
                _docTypeTokenBuilder.SetForceQuirks();
                _parserState = TokenParserState.Data;
                return GetDoctypeToken();
            default:
                _docTypeTokenBuilder.SetForceQuirks();
                _parserState = TokenParserState.BogusDoctype;
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom64BeforeDoctypeSystemIdentifierState()
    {
        var (success, character) = _streamConsumer.TryGetCurrentChar();

        if (!success)
        {
            _docTypeTokenBuilder.SetForceQuirks();
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetDoctypeToken();
        }
        
        switch (character)
        {
            case CharacterReference.CharacterTabulation:
            case CharacterReference.LineFeed:
            case CharacterReference.FormFeed:
            case CharacterReference.Space:
                _streamConsumer.ConsumeChar();
                return null;
            case CharacterReference.QuotationMark:
                _streamConsumer.ConsumeChar();
                _docTypeTokenBuilder.SetSystemIdentifierPresent();
                _parserState = TokenParserState.DoctypeSystemIdentifierDoubleQuoted;
                return null;
            case CharacterReference.Apostrophe:
                _streamConsumer.ConsumeChar();
                _docTypeTokenBuilder.SetSystemIdentifierPresent();
                _parserState = TokenParserState.DoctypeSystemIdentifierSingleQuoted;
                return null;
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeChar();
                _docTypeTokenBuilder.SetForceQuirks();
                _parserState = TokenParserState.Data;
                return GetDoctypeToken();
            default:
                _docTypeTokenBuilder.SetForceQuirks();
                _parserState = TokenParserState.BogusDoctype;
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom65DoctypeSystemIdentifierDoubleQuotedState()
    {
        var (success, character) = _streamConsumer.TryGetCurrentChar();

        if (!success)
        {
            _docTypeTokenBuilder.SetForceQuirks();
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetDoctypeToken();
        }
        
        switch (character)
        {
            case CharacterReference.QuotationMark:
                _streamConsumer.ConsumeChar();
                _parserState = TokenParserState.AfterDoctypeSystemIdentifier;
                return null;
            case CharacterReference.Null:
                _streamConsumer.ConsumeChar();
                _docTypeTokenBuilder.AppendToSystemIdentifier(CharacterReference.ReplacementCharacter);
                return null;
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeChar();
                _docTypeTokenBuilder.SetForceQuirks();
                _parserState = TokenParserState.Data;
                return GetDoctypeToken();
            default:
                _streamConsumer.ConsumeChar();
                _docTypeTokenBuilder.AppendToSystemIdentifier(character);
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom66DoctypeSystemIdentifierSingleQuotedState()
    {
        var (success, character) = _streamConsumer.TryGetCurrentChar();

        if (!success)
        {
            _docTypeTokenBuilder.SetForceQuirks();
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetDoctypeToken();
        }
        
        switch (character)
        {
            case CharacterReference.Apostrophe:
                _streamConsumer.ConsumeChar();
                _parserState = TokenParserState.AfterDoctypeSystemIdentifier;
                return null;
            case CharacterReference.Null:
                _streamConsumer.ConsumeChar();
                _docTypeTokenBuilder.AppendToSystemIdentifier(CharacterReference.ReplacementCharacter);
                return null;
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeChar();
                _docTypeTokenBuilder.SetForceQuirks();
                _parserState = TokenParserState.Data;
                return GetDoctypeToken();
            default:
                _streamConsumer.ConsumeChar();
                _docTypeTokenBuilder.AppendToSystemIdentifier(character);
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom67AfterDoctypeSystemIdentifierState()
    {
        var (success, character) = _streamConsumer.TryGetCurrentChar();

        if (!success)
        {
            _docTypeTokenBuilder.SetForceQuirks();
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetDoctypeToken();
        }
        
        switch (character)
        {
            case CharacterReference.CharacterTabulation:
            case CharacterReference.LineFeed:
            case CharacterReference.FormFeed:
            case CharacterReference.Space:
                _streamConsumer.ConsumeChar();
                return null;
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeChar();
                _parserState = TokenParserState.Data;
                return GetDoctypeToken();
            default:
                _parserState = TokenParserState.BogusDoctype;
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom68BogusDoctypeState()
    {
        var (success, character) = _streamConsumer.TryGetCurrentChar();

        if (!success)
        {
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetDoctypeToken();
        }
        
        switch (character)
        {
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeChar();
                _parserState = TokenParserState.Data;
                return GetDoctypeToken();
            case CharacterReference.Null:
                _streamConsumer.ConsumeChar();
                return null;
            default:
                _streamConsumer.ConsumeChar();
                return null;
        }
    }

    private void CreateNewCommentToken()
    {
        _commentDataBuilder = new StringBuilder();
        _commentToken = null;
    }

    private CommentToken GetCommentToken()
    {
        _commentToken ??= new CommentToken { Data = _commentDataBuilder.ToString() };
        return _commentToken;
    }

    private void CreateNewDoctypeToken()
    {
        _docTypeTokenBuilder = new DocTypeTokenBuilder();
        _docTypeToken = null;
    }

    private DocTypeToken GetDoctypeToken()
    {
        _docTypeToken ??= _docTypeTokenBuilder.Build();
        return _docTypeToken;
    }
    
}