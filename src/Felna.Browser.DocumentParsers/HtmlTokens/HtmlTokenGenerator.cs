using System.Text;
using Felna.Browser.DocumentParsers.StreamConsumers;
using Felna.Browser.DocumentParsers.TextReferences;

namespace Felna.Browser.DocumentParsers.HtmlTokens;

internal class HtmlTokenGenerator
{
    private readonly IStreamConsumer _streamConsumer;
    private TokenParserState _parserState;
    private TokenParserState _returnState;
    
    private StringBuilder _commentDataBuilder = new StringBuilder();
    private CommentToken? _commentToken;

    private DocTypeTokenBuilder _docTypeTokenBuilder = new DocTypeTokenBuilder();
    private DocTypeToken? _docTypeToken;

    private TagTokenBuilder _tagTokenBuilder = new TagTokenBuilder();
    private TagToken? _tagToken;

    private List<UnicodeCodePoint> _temporaryBuffer = new List<UnicodeCodePoint>();
    private readonly List<CharacterToken> _flushedCharacterTokens = new List<CharacterToken>();
    private int _characterReferenceCode;

    internal HtmlTokenGenerator(IStreamConsumer streamConsumer)
    {
        _streamConsumer = streamConsumer;
        _parserState = TokenParserState.Data;
        _returnState = TokenParserState.Data;
    }

    internal HtmlToken GetNextToken()
    {
        HtmlToken? token = null;

        do
        {
            if (_flushedCharacterTokens.Any())
            {
                var characterToken = _flushedCharacterTokens.First();
                _flushedCharacterTokens.Remove(characterToken);
                return characterToken;
            }
            
            token = _parserState switch
            {
                TokenParserState.Data => GetTokenFrom01DataState(),
                TokenParserState.TagOpen => GetTokenFrom06TagOpenState(),
                TokenParserState.EndTagOpen => GetTokenFrom07EndTagOpenState(),
                TokenParserState.TagName => GetTokenFrom08TagNameState(),
                TokenParserState.BeforeAttributeName => GetTokenFrom32BeforeAttributeNameState(),
                TokenParserState.AttributeName => GetTokenFrom33AttributeNameState(),
                TokenParserState.AfterAttributeName => GetTokenFrom34AfterAttributeNameState(),
                TokenParserState.BeforeAttributeValueState => GetTokenFrom35BeforeAttributeValueState(),
                TokenParserState.AttributeValueDoubleQuoted => GetTokenFrom36AttributeValueDoubleQuotedState(),
                TokenParserState.AttributeValueSingleQuoted => GetTokenFrom37AttributeValueSingleQuotedState(),
                TokenParserState.AttributeValueUnquoted => GetTokenFrom38AttributeValueUnquotedState(),
                TokenParserState.AfterAttributeValueQuoted => GetTokenFrom39AfterAttributeValueQuotedState(),
                TokenParserState.SelfClosingStartTag => GetTokenFrom40SelfClosingStartTagState(),
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
                TokenParserState.CharacterReference => GetTokenFrom72CharacterReferenceState(),
                TokenParserState.NamedCharacterReference => GetTokenFrom73NamedCharacterReferenceState(),
                TokenParserState.AmbiguousAmpersand => GetTokenFrom74AmbiguousAmpersandState(),
                TokenParserState.NumericCharacterReference => GetTokenFrom75NumericCharacterReferenceState(),
                TokenParserState.HexadecimalCharacterReferenceStart => GetTokenFrom76HexadecimalCharacterReferenceStartState(),
                TokenParserState.DecimalCharacterReferenceStart => GetTokenFrom77DecimalCharacterReferenceStartState(),
                TokenParserState.HexadecimalCharacterReference => GetTokenFrom78HexadecimalCharacterReferenceState(),
                TokenParserState.DecimalCharacterReference => GetTokenFrom79DecimalCharacterReferenceState(),
                TokenParserState.NumericCharacterReferenceEnd => GetTokenFrom80NumericCharacterReferenceEndState(),
                _ => throw new NotImplementedException()
            };
        } while (token is null);

        return token;
    }

    private HtmlToken? GetTokenFrom01DataState()
    {
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
            return new EndOfFileToken();
        
        switch (codePoint.Value)
        {
            case CharacterReference.Ampersand:
                _streamConsumer.ConsumeCodePoint();
                _returnState = TokenParserState.Data;
                _parserState = TokenParserState.CharacterReference;
                return null;
            case CharacterReference.LessThanSign:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.TagOpen;
                return null;
            case CharacterReference.Null:
                _streamConsumer.ConsumeCodePoint();
                return new CharacterToken { Data = codePoint.ToString() };
            default:
                _streamConsumer.ConsumeCodePoint();
                return new CharacterToken { Data = codePoint.ToString() };
        }
    }

    private HtmlToken? GetTokenFrom06TagOpenState()
    {
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            _parserState = TokenParserState.Data; // so the next token is EOF
            return new CharacterToken{ Data = CharacterReference.LessThanSign.ToString() };
        }
        
        switch (codePoint.Value)
        {
            case CharacterReference.ExclamationMark:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.MarkupDeclarationOpen;
                return null;
            case CharacterReference.Solidus:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.EndTagOpen;
                return null;
            case var _ when CharacterRangeReference.AsciiAlpha.Contains(codePoint):
                CreateNewTagToken();
                _parserState = TokenParserState.TagName;
                return null;
            case CharacterReference.QuestionMark:
                CreateNewCommentToken();
                _parserState = TokenParserState.BogusComment;
                return null;
            default:
                _parserState = TokenParserState.Data;
                return new CharacterToken { Data = CharacterReference.LessThanSign.ToString() };
        }
    }
    
    private HtmlToken? GetTokenFrom07EndTagOpenState()
    {
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            _flushedCharacterTokens.Add(new CharacterToken{ Data = CharacterReference.Solidus.ToString() });
            _parserState = TokenParserState.Data; // so the final token is EOF
            return new CharacterToken{ Data = CharacterReference.LessThanSign.ToString() };
        }
        
        switch (codePoint.Value)
        {
            case var _ when CharacterRangeReference.AsciiAlpha.Contains(codePoint):
                CreateNewTagToken();
                _tagTokenBuilder.SetEndTag();
                _parserState = TokenParserState.TagName;
                return null;
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.Data;
                return null;
            default:
                CreateNewCommentToken();
                _parserState = TokenParserState.BogusComment;
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom08TagNameState()
    {
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            _parserState = TokenParserState.Data; // so the next token is EOF
            return null;
        }
        
        switch (codePoint.Value)
        {
            case CharacterReference.CharacterTabulation:
            case CharacterReference.LineFeed:
            case CharacterReference.FormFeed:
            case CharacterReference.Space:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.BeforeAttributeName;
                return null;
            case CharacterReference.Solidus:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.SelfClosingStartTag;
                return null;
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.Data;
                return GetTagToken();
            case var _ when CharacterRangeReference.AsciiUpperAlpha.Contains(codePoint):
                _streamConsumer.ConsumeCodePoint();
                _tagTokenBuilder.AppendToTagName(codePoint.ToAsciiLower());
                return null;
            case CharacterReference.Null:
                _streamConsumer.ConsumeCodePoint();
                _tagTokenBuilder.AppendToTagName(CharacterReference.ReplacementCharacter.ToString());
                return null;
            default:
                _streamConsumer.ConsumeCodePoint();
                _tagTokenBuilder.AppendToTagName(codePoint);
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom32BeforeAttributeNameState()
    {
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            _parserState = TokenParserState.AfterAttributeName;
            return null;
        }
        
        switch (codePoint.Value)
        {
            case CharacterReference.CharacterTabulation:
            case CharacterReference.LineFeed:
            case CharacterReference.FormFeed:
            case CharacterReference.Space:
                _streamConsumer.ConsumeCodePoint();
                return null;
            case CharacterReference.Solidus:
            case CharacterReference.GreaterThanSign:
                _parserState = TokenParserState.AfterAttributeName;
                return null;
            case CharacterReference.EqualsSign:
                _streamConsumer.ConsumeCodePoint();
                _tagTokenBuilder.StartNewAttribute();
                _tagTokenBuilder.AppendToTagName(codePoint);
                _parserState = TokenParserState.AttributeName;
                return null;
            default:
                _tagTokenBuilder.StartNewAttribute();
                _parserState = TokenParserState.AttributeName;
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom33AttributeNameState()
    {
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            _parserState = TokenParserState.AfterAttributeName;
            return null;
        }
        
        switch (codePoint.Value)
        {
            case CharacterReference.CharacterTabulation:
            case CharacterReference.LineFeed:
            case CharacterReference.FormFeed:
            case CharacterReference.Space:
            case CharacterReference.Solidus:
            case CharacterReference.GreaterThanSign:
                _parserState = TokenParserState.AfterAttributeName;
                return null;
            case CharacterReference.EqualsSign:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.BeforeAttributeValueState;
                return null;
            case var _ when CharacterRangeReference.AsciiUpperAlpha.Contains(codePoint):
                _streamConsumer.ConsumeCodePoint();
                _tagTokenBuilder.AppendToAttributeName(codePoint.ToAsciiLower());
                return null;
            case CharacterReference.Null:
                _streamConsumer.ConsumeCodePoint();
                _tagTokenBuilder.AppendToAttributeName(CharacterReference.ReplacementCharacter.ToString());
                return null;
            default:
                _streamConsumer.ConsumeCodePoint();
                _tagTokenBuilder.AppendToAttributeName(codePoint);
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom34AfterAttributeNameState()
    {
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            _parserState = TokenParserState.Data; // So that the next token is EOF
            return null;
        }
        
        switch (codePoint.Value)
        {
            case CharacterReference.CharacterTabulation:
            case CharacterReference.LineFeed:
            case CharacterReference.FormFeed:
            case CharacterReference.Space:
                _streamConsumer.ConsumeCodePoint();
                return null;
            case CharacterReference.Solidus:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.SelfClosingStartTag;
                return null;
            case CharacterReference.EqualsSign:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.BeforeAttributeValueState;
                return null;
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.Data;
                return GetTagToken();
            default:
                _tagTokenBuilder.StartNewAttribute();
                _parserState = TokenParserState.AttributeName;
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom35BeforeAttributeValueState()
    {
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            _parserState = TokenParserState.AttributeValueUnquoted;
            return null;
        }
        
        switch (codePoint.Value)
        {
            case CharacterReference.CharacterTabulation:
            case CharacterReference.LineFeed:
            case CharacterReference.FormFeed:
            case CharacterReference.Space:
                _streamConsumer.ConsumeCodePoint();
                return null;
            case CharacterReference.QuotationMark:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.AttributeValueDoubleQuoted;
                return null;
            case CharacterReference.Apostrophe:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.AttributeValueSingleQuoted;
                return null;
            case CharacterReference.GreaterThanSign:
                _parserState = TokenParserState.Data;
                return GetTagToken();
            default:
                _parserState = TokenParserState.AttributeValueUnquoted;
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom36AttributeValueDoubleQuotedState()
    {
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            _parserState = TokenParserState.Data; // So that next token is EOF
            return null;
        }
        
        switch (codePoint.Value)
        {
            case CharacterReference.QuotationMark:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.AfterAttributeValueQuoted;
                return null;
            case CharacterReference.Ampersand:
                _streamConsumer.ConsumeCodePoint();
                _returnState = TokenParserState.AttributeValueDoubleQuoted;
                _parserState = TokenParserState.CharacterReference;
                return null;
            case CharacterReference.Null:
                _streamConsumer.ConsumeCodePoint();
                _tagTokenBuilder.AppendToAttributeValue(CharacterReference.ReplacementCharacter.ToString());
                return null;
            default:
                _streamConsumer.ConsumeCodePoint();
                _tagTokenBuilder.AppendToAttributeValue(codePoint);
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom37AttributeValueSingleQuotedState()
    {
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            _parserState = TokenParserState.Data; // So that next token is EOF
            return null;
        }
        
        switch (codePoint.Value)
        {
            case CharacterReference.Apostrophe:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.AfterAttributeValueQuoted;
                return null;
            case CharacterReference.Ampersand:
                _streamConsumer.ConsumeCodePoint();
                _returnState = TokenParserState.AttributeValueSingleQuoted;
                _parserState = TokenParserState.CharacterReference;
                return null;
            case CharacterReference.Null:
                _streamConsumer.ConsumeCodePoint();
                _tagTokenBuilder.AppendToAttributeValue(CharacterReference.ReplacementCharacter.ToString());
                return null;
            default:
                _streamConsumer.ConsumeCodePoint();
                _tagTokenBuilder.AppendToAttributeValue(codePoint);
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom38AttributeValueUnquotedState()
    {
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            _parserState = TokenParserState.Data; // So that next token is EOF
            return null;
        }
        
        switch (codePoint.Value)
        {
            case CharacterReference.CharacterTabulation:
            case CharacterReference.LineFeed:
            case CharacterReference.FormFeed:
            case CharacterReference.Space:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.BeforeAttributeName;
                return null;
            case CharacterReference.Ampersand:
                _streamConsumer.ConsumeCodePoint();
                _returnState = TokenParserState.AttributeValueUnquoted;
                _parserState = TokenParserState.CharacterReference;
                return null;
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.Data;
                return GetTagToken();
            case CharacterReference.Null:
                _streamConsumer.ConsumeCodePoint();
                _tagTokenBuilder.AppendToAttributeValue(CharacterReference.ReplacementCharacter.ToString());
                return null;
            default:
                _streamConsumer.ConsumeCodePoint();
                _tagTokenBuilder.AppendToAttributeValue(codePoint);
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom39AfterAttributeValueQuotedState()
    {
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            _parserState = TokenParserState.Data; // So that next token is EOF
            return null;
        }
        
        switch (codePoint.Value)
        {
            case CharacterReference.CharacterTabulation:
            case CharacterReference.LineFeed:
            case CharacterReference.FormFeed:
            case CharacterReference.Space:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.BeforeAttributeName;
                return null;
            case CharacterReference.Solidus:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.SelfClosingStartTag;
                return null;
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.Data;
                return GetTagToken();
            case CharacterReference.Null:
                _streamConsumer.ConsumeCodePoint();
                _tagTokenBuilder.AppendToAttributeValue(CharacterReference.ReplacementCharacter.ToString());
                return null;
            default:
                _parserState = TokenParserState.BeforeAttributeName;
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom40SelfClosingStartTagState()
    {
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            _parserState = TokenParserState.Data; // So that next token is EOF
            return null;
        }
        
        switch (codePoint.Value)
        {
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeCodePoint();
                _tagTokenBuilder.SetSelfClosing();
                _parserState = TokenParserState.Data;
                return GetTagToken();
            default:
                _parserState = TokenParserState.BeforeAttributeName;
                return null;
        }
    }

    private HtmlToken? GetTokenFrom41BogusCommentState()
    {
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetCommentToken();
        }
        
        switch (codePoint.Value)
        {
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.Data;
                return GetCommentToken();
            case CharacterReference.Null:
                _streamConsumer.ConsumeCodePoint();
                _commentDataBuilder.Append(CharacterReference.ReplacementCharacter);
                return null;
            default:
                _streamConsumer.ConsumeCodePoint();
                _commentDataBuilder.Append(codePoint.ToString());
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
                _streamConsumer.ConsumeCodePoint(StringReference.DoubleHyphen.Length);
                CreateNewCommentToken();
                _parserState = TokenParserState.CommentStart;
                return null;
            }

            (success, result) = _streamConsumer.LookAhead(StringReference.DocType.Length);

            if (success)
            {
                if (StringReference.AsciiCaseInsensitiveEquals(result, StringReference.DocType))
                {
                    _streamConsumer.ConsumeCodePoint(StringReference.DocType.Length);
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
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            _parserState = TokenParserState.Comment;
            return null;
        }
        
        switch (codePoint.Value)
        {
            case CharacterReference.HyphenMinus:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.CommentStartDash;
                return null;
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.Data;
                return GetCommentToken();
            default:
                _parserState = TokenParserState.Comment;
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom44CommentStartDashState()
    {
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetCommentToken();
        }
        
        switch (codePoint.Value)
        {
            case CharacterReference.HyphenMinus:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.CommentEnd;
                return null;
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeCodePoint();
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
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetCommentToken();
        }
        
        switch (codePoint.Value)
        {
            case CharacterReference.LessThanSign:
                _streamConsumer.ConsumeCodePoint();
                _commentDataBuilder.Append(codePoint.ToString());
                _parserState = TokenParserState.CommentLessThanSign;
                return null;
            case CharacterReference.HyphenMinus:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.CommentEndDash;
                return null;
            case CharacterReference.Null:
                _streamConsumer.ConsumeCodePoint();
                _commentDataBuilder.Append(CharacterReference.ReplacementCharacter);
                return null;
            default:
                _streamConsumer.ConsumeCodePoint();
                _commentDataBuilder.Append(codePoint.ToString());
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom46CommentLessThanSignState()
    {
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            _parserState = TokenParserState.Comment;
            return null;
        }
        
        switch (codePoint.Value)
        {
            case CharacterReference.ExclamationMark:
                _streamConsumer.ConsumeCodePoint();
                _commentDataBuilder.Append(codePoint.ToString());
                _parserState = TokenParserState.CommentLessThanSignBang;
                return null;
            case CharacterReference.LessThanSign:
                _streamConsumer.ConsumeCodePoint();
                _commentDataBuilder.Append(codePoint.ToString());
                return null;
            default:
                _parserState = TokenParserState.Comment;
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom47CommentLessThanSignBangState()
    {
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            _parserState = TokenParserState.Comment;
            return null;
        }
        
        switch (codePoint.Value)
        {
            case CharacterReference.HyphenMinus:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.CommentLessThanSignBangDash;
                return null;
            default:
                _parserState = TokenParserState.Comment;
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom48CommentLessThanSignBangDashState()
    {
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            _parserState = TokenParserState.CommentEndDash;
            return null;
        }
        
        switch (codePoint.Value)
        {
            case CharacterReference.HyphenMinus:
                _streamConsumer.ConsumeCodePoint();
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
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetCommentToken();
        }
        
        switch (codePoint.Value)
        {
            case CharacterReference.HyphenMinus:
                _streamConsumer.ConsumeCodePoint();
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
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetCommentToken();
        }
        
        switch (codePoint.Value)
        {
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.Data;
                return GetCommentToken();
            case CharacterReference.ExclamationMark:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.CommentEndBang;
                return null;
            case CharacterReference.HyphenMinus:
                _streamConsumer.ConsumeCodePoint();
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
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetCommentToken();
        }
        
        switch (codePoint.Value)
        {
            case CharacterReference.HyphenMinus:
                _streamConsumer.ConsumeCodePoint();
                _commentDataBuilder.Append(CharacterReference.HyphenMinus);
                _commentDataBuilder.Append(CharacterReference.HyphenMinus);
                _commentDataBuilder.Append(CharacterReference.ExclamationMark);
                _parserState = TokenParserState.CommentEndDash;
                return null;
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeCodePoint();
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
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            CreateNewDoctypeToken();
            _docTypeTokenBuilder.SetForceQuirks();
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetDoctypeToken();
        }
        
        switch (codePoint.Value)
        {
            case CharacterReference.CharacterTabulation:
            case CharacterReference.LineFeed:
            case CharacterReference.FormFeed:
            case CharacterReference.Space:
                _streamConsumer.ConsumeCodePoint();
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
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            CreateNewDoctypeToken();
            _docTypeTokenBuilder.SetForceQuirks();
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetDoctypeToken();
        }
        
        switch (codePoint.Value)
        {
            case CharacterReference.CharacterTabulation:
            case CharacterReference.LineFeed:
            case CharacterReference.FormFeed:
            case CharacterReference.Space:
                _streamConsumer.ConsumeCodePoint();
                return null;
            case var _ when CharacterRangeReference.AsciiUpperAlpha.Contains(codePoint):
                _streamConsumer.ConsumeCodePoint();
                CreateNewDoctypeToken();
                _docTypeTokenBuilder.AppendToName(codePoint.ToAsciiLower());
                _parserState = TokenParserState.DoctypeName;
                return null;
            case CharacterReference.Null:
                _streamConsumer.ConsumeCodePoint();
                CreateNewDoctypeToken();
                _docTypeTokenBuilder.AppendToName(UnicodeCodePoint.ReplacementCharacter);
                _parserState = TokenParserState.DoctypeName;
                return null;
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeCodePoint();
                CreateNewDoctypeToken();
                _docTypeTokenBuilder.SetForceQuirks();
                _parserState = TokenParserState.Data;
                return GetDoctypeToken();
            default:
                _streamConsumer.ConsumeCodePoint();
                CreateNewDoctypeToken();
                _docTypeTokenBuilder.AppendToName(codePoint);
                _parserState = TokenParserState.DoctypeName;
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom55DoctypeNameState()
    {
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            _docTypeTokenBuilder.SetForceQuirks();
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetDoctypeToken();
        }
        
        switch (codePoint.Value)
        {
            case CharacterReference.CharacterTabulation:
            case CharacterReference.LineFeed:
            case CharacterReference.FormFeed:
            case CharacterReference.Space:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.AfterDoctypeName;
                return null;
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.Data;
                return GetDoctypeToken();
            case var _ when CharacterRangeReference.AsciiUpperAlpha.Contains(codePoint):
                _streamConsumer.ConsumeCodePoint();
                _docTypeTokenBuilder.AppendToName(codePoint.ToAsciiLower());
                return null;
            case CharacterReference.Null:
                _streamConsumer.ConsumeCodePoint();
                _docTypeTokenBuilder.AppendToName(UnicodeCodePoint.ReplacementCharacter);
                return null;
            default:
                _streamConsumer.ConsumeCodePoint();
                _docTypeTokenBuilder.AppendToName(codePoint);
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom56AfterDoctypeNameState()
    {
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            _docTypeTokenBuilder.SetForceQuirks();
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetDoctypeToken();
        }
        
        switch (codePoint.Value)
        {
            case CharacterReference.CharacterTabulation:
            case CharacterReference.LineFeed:
            case CharacterReference.FormFeed:
            case CharacterReference.Space:
                _streamConsumer.ConsumeCodePoint();
                return null;
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.Data;
                return GetDoctypeToken();
            default:

                (success, var result) = _streamConsumer.LookAhead(StringReference.Public.Length);

                if (success)
                {
                    if (StringReference.AsciiCaseInsensitiveEquals(result, StringReference.Public))
                    {
                        _streamConsumer.ConsumeCodePoint(StringReference.Public.Length);
                        _parserState = TokenParserState.AfterDoctypePublicKeyword;
                        return null;
                    }
                    
                    if (StringReference.AsciiCaseInsensitiveEquals(result, StringReference.System))
                    {
                        _streamConsumer.ConsumeCodePoint(StringReference.System.Length);
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
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            _docTypeTokenBuilder.SetForceQuirks();
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetDoctypeToken();
        }
        
        switch (codePoint.Value)
        {
            case CharacterReference.CharacterTabulation:
            case CharacterReference.LineFeed:
            case CharacterReference.FormFeed:
            case CharacterReference.Space:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.BeforeDoctypePublicIdentifier;
                return null;
            case CharacterReference.QuotationMark:
                _streamConsumer.ConsumeCodePoint();
                _docTypeTokenBuilder.SetPublicIdentifierPresent();
                _parserState = TokenParserState.DoctypePublicIdentifierDoubleQuoted;
                return null;
            case CharacterReference.Apostrophe:
                _streamConsumer.ConsumeCodePoint();
                _docTypeTokenBuilder.SetPublicIdentifierPresent();
                _parserState = TokenParserState.DoctypePublicIdentifierSingleQuoted;
                return null;
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeCodePoint();
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
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            _docTypeTokenBuilder.SetForceQuirks();
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetDoctypeToken();
        }
        
        switch (codePoint.Value)
        {
            case CharacterReference.CharacterTabulation:
            case CharacterReference.LineFeed:
            case CharacterReference.FormFeed:
            case CharacterReference.Space:
                _streamConsumer.ConsumeCodePoint();
                return null;
            case CharacterReference.QuotationMark:
                _streamConsumer.ConsumeCodePoint();
                _docTypeTokenBuilder.SetPublicIdentifierPresent();
                _parserState = TokenParserState.DoctypePublicIdentifierDoubleQuoted;
                return null;
            case CharacterReference.Apostrophe:
                _streamConsumer.ConsumeCodePoint();
                _docTypeTokenBuilder.SetPublicIdentifierPresent();
                _parserState = TokenParserState.DoctypePublicIdentifierSingleQuoted;
                return null;
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeCodePoint();
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
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            _docTypeTokenBuilder.SetForceQuirks();
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetDoctypeToken();
        }
        
        switch (codePoint.Value)
        {
            case CharacterReference.QuotationMark:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.AfterDoctypePublicIdentifier;
                return null;
            case CharacterReference.Null:
                _streamConsumer.ConsumeCodePoint();
                _docTypeTokenBuilder.AppendToPublicIdentifier(UnicodeCodePoint.ReplacementCharacter);
                return null;
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeCodePoint();
                _docTypeTokenBuilder.SetForceQuirks();
                _parserState = TokenParserState.Data;
                return GetDoctypeToken();
            default:
                _streamConsumer.ConsumeCodePoint();
                _docTypeTokenBuilder.AppendToPublicIdentifier(codePoint);
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom60DoctypePublicIdentifierSingleQuotedState()
    {
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            _docTypeTokenBuilder.SetForceQuirks();
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetDoctypeToken();
        }
        
        switch (codePoint.Value)
        {
            case CharacterReference.Apostrophe:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.AfterDoctypePublicIdentifier;
                return null;
            case CharacterReference.Null:
                _streamConsumer.ConsumeCodePoint();
                _docTypeTokenBuilder.AppendToPublicIdentifier(UnicodeCodePoint.ReplacementCharacter);
                return null;
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeCodePoint();
                _docTypeTokenBuilder.SetForceQuirks();
                _parserState = TokenParserState.Data;
                return GetDoctypeToken();
            default:
                _streamConsumer.ConsumeCodePoint();
                _docTypeTokenBuilder.AppendToPublicIdentifier(codePoint);
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom61AfterDoctypePublicIdentifierState()
    {
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            _docTypeTokenBuilder.SetForceQuirks();
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetDoctypeToken();
        }
        
        switch (codePoint.Value)
        {
            case CharacterReference.CharacterTabulation:
            case CharacterReference.LineFeed:
            case CharacterReference.FormFeed:
            case CharacterReference.Space:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.BetweenDoctypePublicAndSystemIdentifier;
                return null;
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.Data;
                return GetDoctypeToken();
            case CharacterReference.QuotationMark:
                _streamConsumer.ConsumeCodePoint();
                _docTypeTokenBuilder.SetSystemIdentifierPresent();
                _parserState = TokenParserState.DoctypeSystemIdentifierDoubleQuoted;
                return null;
            case CharacterReference.Apostrophe:
                _streamConsumer.ConsumeCodePoint();
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
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            _docTypeTokenBuilder.SetForceQuirks();
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetDoctypeToken();
        }
        
        switch (codePoint.Value)
        {
            case CharacterReference.CharacterTabulation:
            case CharacterReference.LineFeed:
            case CharacterReference.FormFeed:
            case CharacterReference.Space:
                _streamConsumer.ConsumeCodePoint();
                return null;
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.Data;
                return GetDoctypeToken();
            case CharacterReference.QuotationMark:
                _streamConsumer.ConsumeCodePoint();
                _docTypeTokenBuilder.SetSystemIdentifierPresent();
                _parserState = TokenParserState.DoctypeSystemIdentifierDoubleQuoted;
                return null;
            case CharacterReference.Apostrophe:
                _streamConsumer.ConsumeCodePoint();
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
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            _docTypeTokenBuilder.SetForceQuirks();
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetDoctypeToken();
        }
        
        switch (codePoint.Value)
        {
            case CharacterReference.CharacterTabulation:
            case CharacterReference.LineFeed:
            case CharacterReference.FormFeed:
            case CharacterReference.Space:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.BeforeDoctypeSystemIdentifier;
                return null;
            case CharacterReference.QuotationMark:
                _streamConsumer.ConsumeCodePoint();
                _docTypeTokenBuilder.SetSystemIdentifierPresent();
                _parserState = TokenParserState.DoctypeSystemIdentifierDoubleQuoted;
                return null;
            case CharacterReference.Apostrophe:
                _streamConsumer.ConsumeCodePoint();
                _docTypeTokenBuilder.SetSystemIdentifierPresent();
                _parserState = TokenParserState.DoctypeSystemIdentifierSingleQuoted;
                return null;
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeCodePoint();
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
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            _docTypeTokenBuilder.SetForceQuirks();
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetDoctypeToken();
        }
        
        switch (codePoint.Value)
        {
            case CharacterReference.CharacterTabulation:
            case CharacterReference.LineFeed:
            case CharacterReference.FormFeed:
            case CharacterReference.Space:
                _streamConsumer.ConsumeCodePoint();
                return null;
            case CharacterReference.QuotationMark:
                _streamConsumer.ConsumeCodePoint();
                _docTypeTokenBuilder.SetSystemIdentifierPresent();
                _parserState = TokenParserState.DoctypeSystemIdentifierDoubleQuoted;
                return null;
            case CharacterReference.Apostrophe:
                _streamConsumer.ConsumeCodePoint();
                _docTypeTokenBuilder.SetSystemIdentifierPresent();
                _parserState = TokenParserState.DoctypeSystemIdentifierSingleQuoted;
                return null;
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeCodePoint();
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
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            _docTypeTokenBuilder.SetForceQuirks();
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetDoctypeToken();
        }
        
        switch (codePoint.Value)
        {
            case CharacterReference.QuotationMark:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.AfterDoctypeSystemIdentifier;
                return null;
            case CharacterReference.Null:
                _streamConsumer.ConsumeCodePoint();
                _docTypeTokenBuilder.AppendToSystemIdentifier(UnicodeCodePoint.ReplacementCharacter);
                return null;
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeCodePoint();
                _docTypeTokenBuilder.SetForceQuirks();
                _parserState = TokenParserState.Data;
                return GetDoctypeToken();
            default:
                _streamConsumer.ConsumeCodePoint();
                _docTypeTokenBuilder.AppendToSystemIdentifier(codePoint);
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom66DoctypeSystemIdentifierSingleQuotedState()
    {
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            _docTypeTokenBuilder.SetForceQuirks();
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetDoctypeToken();
        }
        
        switch (codePoint.Value)
        {
            case CharacterReference.Apostrophe:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.AfterDoctypeSystemIdentifier;
                return null;
            case CharacterReference.Null:
                _streamConsumer.ConsumeCodePoint();
                _docTypeTokenBuilder.AppendToSystemIdentifier(UnicodeCodePoint.ReplacementCharacter);
                return null;
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeCodePoint();
                _docTypeTokenBuilder.SetForceQuirks();
                _parserState = TokenParserState.Data;
                return GetDoctypeToken();
            default:
                _streamConsumer.ConsumeCodePoint();
                _docTypeTokenBuilder.AppendToSystemIdentifier(codePoint);
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom67AfterDoctypeSystemIdentifierState()
    {
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            _docTypeTokenBuilder.SetForceQuirks();
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetDoctypeToken();
        }
        
        switch (codePoint.Value)
        {
            case CharacterReference.CharacterTabulation:
            case CharacterReference.LineFeed:
            case CharacterReference.FormFeed:
            case CharacterReference.Space:
                _streamConsumer.ConsumeCodePoint();
                return null;
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.Data;
                return GetDoctypeToken();
            default:
                _parserState = TokenParserState.BogusDoctype;
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom68BogusDoctypeState()
    {
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            _parserState = TokenParserState.Data; // so the next token is EOF
            return GetDoctypeToken();
        }
        
        switch (codePoint.Value)
        {
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.Data;
                return GetDoctypeToken();
            case CharacterReference.Null:
                _streamConsumer.ConsumeCodePoint();
                return null;
            default:
                _streamConsumer.ConsumeCodePoint();
                return null;
        }
    }

    private HtmlToken? GetTokenFrom72CharacterReferenceState()
    {
        _temporaryBuffer = new List<UnicodeCodePoint>();
        _temporaryBuffer.Add(new UnicodeCodePoint(CharacterReference.Ampersand));
        
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            FlushCodePointsConsumedAsACharacterReference();
            _parserState = _returnState;
            return null;
        }

        switch (codePoint.Value)
        {
            case var _ when CharacterRangeReference.AsciiAlphaNumeric.Contains(codePoint):
                _parserState = TokenParserState.NamedCharacterReference;
                return null;
            case CharacterReference.NumberSign:
                _streamConsumer.ConsumeCodePoint();
                _temporaryBuffer.Add(codePoint);
                _parserState = TokenParserState.NumericCharacterReference;
                return null;
            default:
                FlushCodePointsConsumedAsACharacterReference();
                _parserState = _returnState;
                return null;
        }
    }

    private HtmlToken? GetTokenFrom73NamedCharacterReferenceState()
    {
        var partialName = new string(new[] { CharacterReference.Ampersand });
        var matchingNames = NamedEntityReference.Entities.Keys.Where(k => k.StartsWith(partialName, StringComparison.Ordinal)).ToArray();
        string? match = null;
        
        while (matchingNames.Length > 1)
        {
            var (success, result) = _streamConsumer.LookAhead(partialName.Length);
            if (!success)
            {
                matchingNames = Array.Empty<string>();
                break;
            }

            partialName = CharacterReference.Ampersand + result;
            matchingNames = NamedEntityReference.Entities.Keys.Where(k => k.StartsWith(partialName, StringComparison.Ordinal)).ToArray();
            if (matchingNames.Contains(partialName))
            {
                match = partialName; // We've matched one reference, store in case further characters match nothing
            }
                
        }

        if (matchingNames.Length == 1 && (match is null || match != matchingNames[0]))
        {
            var possibleMatch = matchingNames[0];
            var (success, result) = _streamConsumer.LookAhead(possibleMatch.Length - 1);
            if (success && possibleMatch == CharacterReference.Ampersand + result)
            {
                match = possibleMatch;
            }
        }

        if (match is not null)
        {
            var historical = false;
            if (ConsumedAsPartOfAnAttribute() && match[^1] != CharacterReference.SemiColon)
            {
                var (success, result) = _streamConsumer.LookAhead(match.Length);
                if (success)
                {
                    var nextInputChar = result[^1];
                    historical = nextInputChar == CharacterReference.EqualsSign || CharacterRangeReference.AsciiAlphaNumeric.Contains(new UnicodeCodePoint(nextInputChar));
                }
            }

            if (historical)
            {
                _temporaryBuffer = UnicodeCodePoint.ConvertFromString(match).ToList();
            }
            else
            {
                _temporaryBuffer = NamedEntityReference.Entities[match].CodePoints.Select(c => new UnicodeCodePoint(c)).ToList();
            }
            
            _streamConsumer.ConsumeCodePoint(match.Length - 1);
            
            _parserState = _returnState;
        }
        else
        {
            _parserState = TokenParserState.AmbiguousAmpersand;
        }

        FlushCodePointsConsumedAsACharacterReference();
        return null;
    }

    private HtmlToken? GetTokenFrom74AmbiguousAmpersandState()
    {
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            _parserState = _returnState;
            return null;
        }

        switch (codePoint.Value)
        {
            case var _ when CharacterRangeReference.AsciiAlphaNumeric.Contains(codePoint):
                _streamConsumer.ConsumeCodePoint();
                
                if (ConsumedAsPartOfAnAttribute())
                {
                    _tagTokenBuilder.AppendToAttributeValue(codePoint);
                    return null;
                }

                return new CharacterToken { Data = codePoint.ToString() };
            case CharacterReference.SemiColon:
                _parserState = _returnState;
                return null;
            default:
                _parserState = _returnState;
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom75NumericCharacterReferenceState()
    {
        _characterReferenceCode = 0;
        
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            _parserState = TokenParserState.DecimalCharacterReferenceStart;
            return null;
        }

        switch (codePoint.Value)
        {
            case CharacterReference.LowerCaseX:
            case CharacterReference.UpperCaseX:
                _streamConsumer.ConsumeCodePoint();
                _temporaryBuffer.Add(codePoint);
                _parserState = TokenParserState.HexadecimalCharacterReferenceStart;
                return null;
            default:
                _parserState = TokenParserState.DecimalCharacterReferenceStart;
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom76HexadecimalCharacterReferenceStartState()
    {
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            FlushCodePointsConsumedAsACharacterReference();
            _parserState = _returnState;
            return null;
        }

        switch (codePoint.Value)
        {
            case var _ when CharacterRangeReference.AsciiHex.Contains(codePoint):
                _parserState = TokenParserState.HexadecimalCharacterReference;
                return null;
            default:
                FlushCodePointsConsumedAsACharacterReference();
                _parserState = _returnState;
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom77DecimalCharacterReferenceStartState()
    {
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            FlushCodePointsConsumedAsACharacterReference();
            _parserState = _returnState;
            return null;
        }

        switch (codePoint.Value)
        {
            case var _ when CharacterRangeReference.AsciiDigit.Contains(codePoint):
                _parserState = TokenParserState.DecimalCharacterReference;
                return null;
            default:
                FlushCodePointsConsumedAsACharacterReference();
                _parserState = _returnState;
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom78HexadecimalCharacterReferenceState()
    {
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            _parserState = TokenParserState.NumericCharacterReferenceEnd;
            return null;
        }

        switch (codePoint.Value)
        {
            case var _ when CharacterRangeReference.AsciiDigit.Contains(codePoint):
                _streamConsumer.ConsumeCodePoint();
                ShiftAndIncrementCharacterReferenceCode(16, codePoint.Value - 0x30);
                return null;
            case var _ when CharacterRangeReference.AsciiUpperHexLetter.Contains(codePoint):
                _streamConsumer.ConsumeCodePoint();
                ShiftAndIncrementCharacterReferenceCode(16, codePoint.Value - 0x37);
                return null;
            case var _ when CharacterRangeReference.AsciiLowerHexLetter.Contains(codePoint):
                _streamConsumer.ConsumeCodePoint();
                ShiftAndIncrementCharacterReferenceCode(16, codePoint.Value - 0x57);
                return null;
            case CharacterReference.SemiColon:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.NumericCharacterReferenceEnd;
                return null;
            default:
                _parserState = TokenParserState.NumericCharacterReferenceEnd;
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom79DecimalCharacterReferenceState()
    {
        var (success, codePoint) = _streamConsumer.TryGetCurrentCodePoint();

        if (!success)
        {
            _parserState = TokenParserState.NumericCharacterReferenceEnd;
            return null;
        }

        switch (codePoint.Value)
        {
            case var _ when CharacterRangeReference.AsciiDigit.Contains(codePoint):
                _streamConsumer.ConsumeCodePoint();
                ShiftAndIncrementCharacterReferenceCode(10, codePoint.Value - 0x30);
                return null;
            case CharacterReference.SemiColon:
                _streamConsumer.ConsumeCodePoint();
                _parserState = TokenParserState.NumericCharacterReferenceEnd;
                return null;
            default:
                _parserState = TokenParserState.NumericCharacterReferenceEnd;
                return null;
        }
    }
    
    private HtmlToken? GetTokenFrom80NumericCharacterReferenceEndState()
    {
        var charRefCodeMap = new Dictionary<int, int>
        {
            { 0x80, 0x20AC },
            { 0x82, 0x201A },
            { 0x83, 0x0192 },
            { 0x84, 0x201E },
            { 0x85, 0x2026 },
            { 0x86, 0x2020 },
            { 0x87, 0x2021 },
            { 0x88, 0x02C6 },
            { 0x89, 0x2030 },
            { 0x8A, 0x0160 },
            { 0x8B, 0x2039 },
            { 0x8C, 0x0152 },
            { 0x8E, 0x017D },
            { 0x91, 0x2018 },
            { 0x92, 0x2019 },
            { 0x93, 0x201C },
            { 0x94, 0x201D },
            { 0x95, 0x2022 },
            { 0x96, 0x2013 },
            { 0x97, 0x2014 },
            { 0x98, 0x02DC },
            { 0x99, 0x2122 },
            { 0x9A, 0x0161 },
            { 0x9B, 0x203A },
            { 0x9C, 0x0153 },
            { 0x9E, 0x017E },
            { 0x9F, 0x0178 },
        };
        if (_characterReferenceCode <= 0)
        {
            _characterReferenceCode = CharacterReference.ReplacementCharacter;
        } 
        else if (_characterReferenceCode > UnicodeCodePoint.MaxCodePoint)
        {
            _characterReferenceCode = CharacterReference.ReplacementCharacter;
        }
        else if (IsCharacterReferenceCodeSurrogate())
        {
            _characterReferenceCode = CharacterReference.ReplacementCharacter;
        }
        else if (charRefCodeMap.TryGetValue(_characterReferenceCode, out var replacementCodePoint))
        {
            _characterReferenceCode = replacementCodePoint;
        }

        _temporaryBuffer = new List<UnicodeCodePoint> { new UnicodeCodePoint(_characterReferenceCode) };
        FlushCodePointsConsumedAsACharacterReference();
        _parserState = _returnState;
        
        return null;
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
    
    private void CreateNewTagToken()
    {
        _tagTokenBuilder = new TagTokenBuilder();
        _tagToken = null;
    }

    private TagToken GetTagToken()
    {
        _tagToken ??= _tagTokenBuilder.Build();
        return _tagToken;
    }

    private void FlushCodePointsConsumedAsACharacterReference()
    {
        if (ConsumedAsPartOfAnAttribute())
        {
            _tagTokenBuilder.AppendToAttributeValue(UnicodeCodePoint.ConvertToString(_temporaryBuffer));
        }
        else
        {
            var tokens = _temporaryBuffer.Select(c => new CharacterToken { Data = c.ToString() });
            _flushedCharacterTokens.AddRange(tokens);
        }
    }

    private bool ConsumedAsPartOfAnAttribute()
    {
        return _returnState is TokenParserState.AttributeValueDoubleQuoted or TokenParserState.AttributeValueSingleQuoted or TokenParserState.AttributeValueUnquoted;
    }

    private void ShiftAndIncrementCharacterReferenceCode(int shift, int increment)
    {
        if (_characterReferenceCode <= UnicodeCodePoint.MaxCodePoint)
        {
            _characterReferenceCode *= shift;
            _characterReferenceCode += increment;
        }
    }

    private bool IsCharacterReferenceCodeSurrogate()
    {
        return _characterReferenceCode is >= 0xD800 and <= 0xDFFF;
    }
}