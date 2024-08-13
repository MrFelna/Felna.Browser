using System.Diagnostics;
using System.Text;
using Felna.Browser.DocumentParsers.StreamConsumers;
using Felna.Browser.DocumentParsers.TextReferences;
using static Felna.Browser.DocumentParsers.HtmlTokens.TokenParserState;
using static Felna.Browser.DocumentParsers.TextReferences.CharacterReference;

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
    private readonly Queue<HtmlToken> _tokensToEmit = new Queue<HtmlToken>();
    private int _characterReferenceCode;

    private (bool Success, UnicodeCodePoint CodePoint) _currentCharacter = new(false, UnicodeCodePoint.ReplacementCharacter);
    private bool _reconsumeFlag;

    private bool _eofFlag;

    internal HtmlTokenGenerator(IStreamConsumer streamConsumer)
    {
        _streamConsumer = streamConsumer;
        _parserState = Data;
        _returnState = Data;
    }

    internal HtmlToken GetNextToken()
    {
        if (_eofFlag)
            return new EndOfFileToken();

        do
        {
            if (_tokensToEmit.TryDequeue(out var token))
            {
                _eofFlag = token is EndOfFileToken;
                return token;
            }
            
            var stateHandler = _parserState switch
            {
                Data => (Action)GetTokenFrom01DataState,
                TagOpen => GetTokenFrom06TagOpenState,
                EndTagOpen => GetTokenFrom07EndTagOpenState,
                TagName => GetTokenFrom08TagNameState,
                BeforeAttributeName => GetTokenFrom32BeforeAttributeNameState,
                AttributeName => GetTokenFrom33AttributeNameState,
                AfterAttributeName => GetTokenFrom34AfterAttributeNameState,
                BeforeAttributeValueState => GetTokenFrom35BeforeAttributeValueState,
                AttributeValueDoubleQuoted => GetTokenFrom36AttributeValueDoubleQuotedState,
                AttributeValueSingleQuoted => GetTokenFrom37AttributeValueSingleQuotedState,
                AttributeValueUnquoted => GetTokenFrom38AttributeValueUnquotedState,
                AfterAttributeValueQuoted => GetTokenFrom39AfterAttributeValueQuotedState,
                SelfClosingStartTag => GetTokenFrom40SelfClosingStartTagState,
                BogusComment => GetTokenFrom41BogusCommentState,
                MarkupDeclarationOpen => GetTokenFrom42MarkupDeclarationOpenState,
                CommentStart => GetTokenFrom43CommentStartState,
                CommentStartDash => GetTokenFrom44CommentStartDashState,
                Comment => GetTokenFrom45CommentState,
                CommentLessThanSign => GetTokenFrom46CommentLessThanSignState,
                CommentLessThanSignBang => GetTokenFrom47CommentLessThanSignBangState,
                CommentLessThanSignBangDash => GetTokenFrom48CommentLessThanSignBangDashState,
                CommentLessThanSignBangDashDash => GetTokenFrom49CommentLessThanSignBangDashDashState,
                CommentEndDash => GetTokenFrom50CommentEndDashState,
                CommentEnd => GetTokenFrom51CommentEndState,
                CommentEndBang => GetTokenFrom52CommentEndBangState,
                Doctype => GetTokenFrom53DoctypeState,
                BeforeDoctypeName => GetTokenFrom54BeforeDoctypeNameState,
                DoctypeName => GetTokenFrom55DoctypeNameState,
                AfterDoctypeName => GetTokenFrom56AfterDoctypeNameState,
                AfterDoctypePublicKeyword => GetTokenFrom57AfterDoctypePublicKeywordState,
                BeforeDoctypePublicIdentifier => GetTokenFrom58BeforeDoctypePublicIdentifierState,
                DoctypePublicIdentifierDoubleQuoted => GetTokenFrom59DoctypePublicIdentifierDoubleQuotedState,
                DoctypePublicIdentifierSingleQuoted => GetTokenFrom60DoctypePublicIdentifierSingleQuotedState,
                AfterDoctypePublicIdentifier => GetTokenFrom61AfterDoctypePublicIdentifierState,
                BetweenDoctypePublicAndSystemIdentifier => GetTokenFrom62BetweenDoctypePublicAndSystemIdentifiersState,
                AfterDoctypeSystemKeyword => GetTokenFrom63AfterDoctypeSystemKeywordState,
                BeforeDoctypeSystemIdentifier => GetTokenFrom64BeforeDoctypeSystemIdentifierState,
                DoctypeSystemIdentifierDoubleQuoted => GetTokenFrom65DoctypeSystemIdentifierDoubleQuotedState,
                DoctypeSystemIdentifierSingleQuoted => GetTokenFrom66DoctypeSystemIdentifierSingleQuotedState,
                AfterDoctypeSystemIdentifier => GetTokenFrom67AfterDoctypeSystemIdentifierState,
                BogusDoctype => GetTokenFrom68BogusDoctypeState,
                TokenParserState.CharacterReference => GetTokenFrom72CharacterReferenceState,
                NamedCharacterReference => GetTokenFrom73NamedCharacterReferenceState,
                AmbiguousAmpersand => GetTokenFrom74AmbiguousAmpersandState,
                NumericCharacterReference => GetTokenFrom75NumericCharacterReferenceState,
                HexadecimalCharacterReferenceStart => GetTokenFrom76HexadecimalCharacterReferenceStartState,
                DecimalCharacterReferenceStart => GetTokenFrom77DecimalCharacterReferenceStartState,
                HexadecimalCharacterReference => GetTokenFrom78HexadecimalCharacterReferenceState,
                DecimalCharacterReference => GetTokenFrom79DecimalCharacterReferenceState,
                NumericCharacterReferenceEnd => GetTokenFrom80NumericCharacterReferenceEndState,
                _ => throw new InvalidOperationException(),
            };
            
            stateHandler.Invoke();
            
        } while (true);
    }

    private void GetTokenFrom01DataState()
    {
        var (success, codePoint) = ConsumeNextInputCharacter();
        
        switch (codePoint.Value)
        {
            case Ampersand:
                SetReturnStateTo(Data);
                SwitchTo(TokenParserState.CharacterReference);
                return;
            case LessThanSign:
                SwitchTo(TagOpen);
                return;
            case Null:
                Emit(new CharacterToken { Data = codePoint.ToString() });
                return;
            case var _ when !success:
                Emit(new EndOfFileToken());
                return;
            default:
                Emit(new CharacterToken { Data = codePoint.ToString() });
                return;
        }
    }

    private void GetTokenFrom06TagOpenState()
    {
        var (success, codePoint) = ConsumeNextInputCharacter();
        
        switch (codePoint.Value)
        {
            case ExclamationMark:
                SwitchTo(MarkupDeclarationOpen);
                return;
            case Solidus:
                SwitchTo(EndTagOpen);
                return;
            case var _ when CharacterRangeReference.AsciiAlpha.Contains(codePoint):
                CreateNewTagToken();
                ReconsumeIn(TagName);
                return;
            case QuestionMark:
                CreateNewCommentToken();
                ReconsumeIn(BogusComment);
                return;
            case var _ when !success:
                Emit(new CharacterToken{ Data = LessThanSign.ToString() }, new EndOfFileToken());
                return;
            default:
                Emit(new CharacterToken { Data = LessThanSign.ToString() });
                ReconsumeIn(Data);
                return;
        }
    }
    
    private void GetTokenFrom07EndTagOpenState()
    {
        var (success, codePoint) = ConsumeNextInputCharacter();
        
        switch (codePoint.Value)
        {
            case var _ when CharacterRangeReference.AsciiAlpha.Contains(codePoint):
                CreateNewTagToken();
                _tagTokenBuilder.SetEndTag();
                ReconsumeIn(TagName);
                return;
            case GreaterThanSign:
                SwitchTo(Data);
                return;
            case var _ when !success:
                Emit(new CharacterToken { Data = LessThanSign.ToString() }, new CharacterToken { Data = Solidus.ToString() }, new EndOfFileToken());
                return;
            default:
                CreateNewCommentToken();
                ReconsumeIn(BogusComment);
                return;
        }
    }
    
    private void GetTokenFrom08TagNameState()
    {
        var (success, codePoint) = ConsumeNextInputCharacter();
        
        switch (codePoint.Value)
        {
            case CharacterTabulation:
            case LineFeed:
            case FormFeed:
            case Space:
                SwitchTo(BeforeAttributeName);
                return;
            case Solidus:
                SwitchTo(SelfClosingStartTag);
                return;
            case GreaterThanSign:
                SwitchTo(Data);
                Emit(GetTagToken());
                return;
            case var _ when CharacterRangeReference.AsciiUpperAlpha.Contains(codePoint):
                _tagTokenBuilder.AppendToTagName(codePoint.ToAsciiLower());
                return;
            case Null:
                _tagTokenBuilder.AppendToTagName(ReplacementCharacter.ToString());
                return;
            case var _ when !success:
                Emit(new EndOfFileToken());
                return;
            default:
                _tagTokenBuilder.AppendToTagName(codePoint);
                return;
        }
    }
    
    private void GetTokenFrom32BeforeAttributeNameState()
    {
        var (success, codePoint) = ConsumeNextInputCharacter();

        switch (codePoint.Value)
        {
            case CharacterTabulation:
            case LineFeed:
            case FormFeed:
            case Space:
                return; // ignore
            case Solidus:
            case GreaterThanSign:
            case var _ when !success:
                ReconsumeIn(AfterAttributeName);
                return;
            case EqualsSign:
                _tagTokenBuilder.StartNewAttribute();
                _tagTokenBuilder.AppendToAttributeName(codePoint);
                SwitchTo(AttributeName);
                return;
            default:
                _tagTokenBuilder.StartNewAttribute();
                ReconsumeIn(AttributeName);
                return;
        }
    }
    
    private void GetTokenFrom33AttributeNameState()
    {
        var (success, codePoint) = ConsumeNextInputCharacter();
        
        switch (codePoint.Value)
        {
            case CharacterTabulation:
            case LineFeed:
            case FormFeed:
            case Space:
            case Solidus:
            case GreaterThanSign:
            case var _ when !success:
                ReconsumeIn(AfterAttributeName);
                return;
            case EqualsSign:
                SwitchTo(BeforeAttributeValueState);
                return;
            case var _ when CharacterRangeReference.AsciiUpperAlpha.Contains(codePoint):
                _tagTokenBuilder.AppendToAttributeName(codePoint.ToAsciiLower());
                return;
            case Null:
                _tagTokenBuilder.AppendToAttributeName(ReplacementCharacter.ToString());
                return;
            default:
                _tagTokenBuilder.AppendToAttributeName(codePoint);
                return;
        }
    }
    
    private void GetTokenFrom34AfterAttributeNameState()
    {
        var (success, codePoint) = ConsumeNextInputCharacter();
        
        switch (codePoint.Value)
        {
            case CharacterTabulation:
            case LineFeed:
            case FormFeed:
            case Space:
                return; // ignore
            case Solidus:
                SwitchTo(SelfClosingStartTag);
                return;
            case EqualsSign:
                SwitchTo(BeforeAttributeValueState);
                return;
            case GreaterThanSign:
                SwitchTo(Data);
                Emit(GetTagToken());
                return;
            case var _ when !success:
                Emit(new EndOfFileToken());
                return;
            default:
                _tagTokenBuilder.StartNewAttribute();
                ReconsumeIn(AttributeName);
                return;
        }
    }
    
    private void GetTokenFrom35BeforeAttributeValueState()
    {
        var (_, codePoint) = ConsumeNextInputCharacter();
        
        switch (codePoint.Value)
        {
            case CharacterTabulation:
            case LineFeed:
            case FormFeed:
            case Space:
                return; // ignore
            case QuotationMark:
                SwitchTo(AttributeValueDoubleQuoted);
                return;
            case Apostrophe:
                SwitchTo(AttributeValueSingleQuoted);
                return;
            case GreaterThanSign:
                SwitchTo(Data);
                Emit(GetTagToken());
                return;
            default:
                ReconsumeIn(AttributeValueUnquoted);
                return;
        }
    }
    
    private void GetTokenFrom36AttributeValueDoubleQuotedState()
    {
        var (success, codePoint) = ConsumeNextInputCharacter();
        
        switch (codePoint.Value)
        {
            case QuotationMark:
                SwitchTo(AfterAttributeValueQuoted);
                return;
            case Ampersand:
                SetReturnStateTo(AttributeValueDoubleQuoted);
                SwitchTo(TokenParserState.CharacterReference);
                return;
            case Null:
                _tagTokenBuilder.AppendToAttributeValue(ReplacementCharacter.ToString());
                return;
            case var _ when !success:
                Emit(new EndOfFileToken());
                return;
            default:
                _tagTokenBuilder.AppendToAttributeValue(codePoint);
                return;
        }
    }
    
    private void GetTokenFrom37AttributeValueSingleQuotedState()
    {
        var (success, codePoint) = ConsumeNextInputCharacter();
        
        switch (codePoint.Value)
        {
            case Apostrophe:
                SwitchTo(AfterAttributeValueQuoted);
                return;
            case Ampersand:
                SetReturnStateTo(AttributeValueSingleQuoted);
                SwitchTo(TokenParserState.CharacterReference);
                return;
            case Null:
                _tagTokenBuilder.AppendToAttributeValue(ReplacementCharacter.ToString());
                return;
            case var _ when !success:
                Emit(new EndOfFileToken());
                return;
            default:
                _tagTokenBuilder.AppendToAttributeValue(codePoint);
                return;
        }
    }
    
    private void GetTokenFrom38AttributeValueUnquotedState()
    {
        var (success, codePoint) = ConsumeNextInputCharacter();
        
        switch (codePoint.Value)
        {
            case CharacterTabulation:
            case LineFeed:
            case FormFeed:
            case Space:
                SwitchTo(BeforeAttributeName);
                return;
            case Ampersand:
                SetReturnStateTo(AttributeValueUnquoted);
                SwitchTo(TokenParserState.CharacterReference);
                return;
            case GreaterThanSign:
                SwitchTo(Data);
                Emit(GetTagToken());
                return;
            case Null:
                _tagTokenBuilder.AppendToAttributeValue(ReplacementCharacter.ToString());
                return;
            case var _ when !success:
                Emit(new EndOfFileToken());
                return;
            default:
                _tagTokenBuilder.AppendToAttributeValue(codePoint);
                return;
        }
    }
    
    private void GetTokenFrom39AfterAttributeValueQuotedState()
    {
        var (success, codePoint) = ConsumeNextInputCharacter();
        
        switch (codePoint.Value)
        {
            case CharacterTabulation:
            case LineFeed:
            case FormFeed:
            case Space:
                SwitchTo(BeforeAttributeName);
                return;
            case Solidus:
                SwitchTo(SelfClosingStartTag);
                return;
            case GreaterThanSign:
                SwitchTo(Data);
                Emit(GetTagToken());
                return;
            case var _ when !success:
                Emit(new EndOfFileToken());
                return;
            default:
                ReconsumeIn(BeforeAttributeName);
                return;
        }
    }
    
    private void GetTokenFrom40SelfClosingStartTagState()
    {
        var (success, codePoint) = ConsumeNextInputCharacter();
        
        switch (codePoint.Value)
        {
            case GreaterThanSign:
                _tagTokenBuilder.SetSelfClosing();
                SwitchTo(Data);
                Emit(GetTagToken());
                return;
            case var _ when !success:
                Emit(new EndOfFileToken());
                return;
            default:
                ReconsumeIn(BeforeAttributeName);
                return;
        }
    }

    private void GetTokenFrom41BogusCommentState()
    {
        var (success, codePoint) = ConsumeNextInputCharacter();
        
        switch (codePoint.Value)
        {
            case GreaterThanSign:
                SwitchTo(Data);
                Emit(GetCommentToken());
                return;
            case Null:
                _commentDataBuilder.Append(ReplacementCharacter);
                return;
            case var _ when !success:
                Emit(GetCommentToken(), new EndOfFileToken());
                return;
            default:
                _commentDataBuilder.Append(codePoint.ToString());
                return;
        }
    }
    
    private void GetTokenFrom42MarkupDeclarationOpenState()
    {
        var (success, result) = LookAhead(StringReference.DoubleHyphen.Length, false);

        if (success)
        {
            if (result == StringReference.DoubleHyphen)
            {
                ConsumeLookAhead(StringReference.DoubleHyphen.Length);
                CreateNewCommentToken();
                SwitchTo(CommentStart);
                return;
            }

            (success, result) = LookAhead(StringReference.DocType.Length, false);

            if (success)
            {
                if (StringReference.AsciiCaseInsensitiveEquals(result, StringReference.DocType))
                {
                    ConsumeLookAhead(StringReference.DocType.Length);
                    SwitchTo(Doctype);
                    return;
                }
                
                if (result == "[CDATA[")
                {
                    throw new NotImplementedException();
                }
            }
        }

        CreateNewCommentToken();
        SwitchTo(BogusComment);
    }

    private void GetTokenFrom43CommentStartState()
    {
        var (_, codePoint) = ConsumeNextInputCharacter();
        
        switch (codePoint.Value)
        {
            case HyphenMinus:
                SwitchTo(CommentStartDash);
                return;
            case GreaterThanSign:
                SwitchTo(Data);
                Emit(GetCommentToken());
                return;
            default:
                ReconsumeIn(Comment);
                return;
        }
    }
    
    private void GetTokenFrom44CommentStartDashState()
    {
        var (success, codePoint) = ConsumeNextInputCharacter();
        
        switch (codePoint.Value)
        {
            case HyphenMinus:
                SwitchTo(CommentEnd);
                return;
            case GreaterThanSign:
                SwitchTo(Data);
                Emit(GetCommentToken());
                return;
            case var _ when !success:
                Emit(GetCommentToken(), new EndOfFileToken());
                return;
            default:
                _commentDataBuilder.Append(HyphenMinus);
                ReconsumeIn(Comment);
                return;
        }
    }
    
    private void GetTokenFrom45CommentState()
    {
        var (success, codePoint) = ConsumeNextInputCharacter();
        
        switch (codePoint.Value)
        {
            case LessThanSign:
                _commentDataBuilder.Append(codePoint.ToString());
                SwitchTo(CommentLessThanSign);
                return;
            case HyphenMinus:
                SwitchTo(CommentEndDash);
                return;
            case Null:
                _commentDataBuilder.Append(ReplacementCharacter);
                return;
            case var _ when !success:
                Emit(GetCommentToken(), new EndOfFileToken());
                return;
            default:
                _commentDataBuilder.Append(codePoint.ToString());
                return;
        }
    }
    
    private void GetTokenFrom46CommentLessThanSignState()
    {
        var (_, codePoint) = ConsumeNextInputCharacter(); 
        
        switch (codePoint.Value)
        {
            case ExclamationMark:
                _commentDataBuilder.Append(codePoint.ToString());
                SwitchTo(CommentLessThanSignBang);
                return;
            case LessThanSign:
                _commentDataBuilder.Append(codePoint.ToString());
                return;
            default:
                ReconsumeIn(Comment);
                return;
        }
    }
    
    private void GetTokenFrom47CommentLessThanSignBangState()
    {
        var (_, codePoint) = ConsumeNextInputCharacter();
        
        switch (codePoint.Value)
        {
            case HyphenMinus:
                SwitchTo(CommentLessThanSignBangDash);
                return;
            default:
                ReconsumeIn(Comment);
                return;
        }
    }
    
    private void GetTokenFrom48CommentLessThanSignBangDashState()
    {
        var (_, codePoint) = ConsumeNextInputCharacter();
        
        switch (codePoint.Value)
        {
            case HyphenMinus:
                SwitchTo(CommentLessThanSignBangDashDash);
                return;
            default:
                ReconsumeIn(CommentEndDash);
                return;
        }
    }
    
    private void GetTokenFrom49CommentLessThanSignBangDashDashState()
    {
        ConsumeNextInputCharacter();
        ReconsumeIn(CommentEnd);
    }
    
    private void GetTokenFrom50CommentEndDashState()
    {
        var (success, codePoint) = ConsumeNextInputCharacter();
        
        switch (codePoint.Value)
        {
            case HyphenMinus:
                SwitchTo(CommentEnd);
                return;
            case var _ when !success:
                Emit(GetCommentToken(), new EndOfFileToken());
                return;
            default:
                _commentDataBuilder.Append(HyphenMinus);
                ReconsumeIn(Comment);
                return;
        }
    }
    
    private void GetTokenFrom51CommentEndState()
    {
        var (success, codePoint) = ConsumeNextInputCharacter();

        switch (codePoint.Value)
        {
            case GreaterThanSign:
                SwitchTo(Data);
                Emit(GetCommentToken());
                return;
            case ExclamationMark:
                SwitchTo(CommentEndBang);
                return;
            case HyphenMinus:
                _commentDataBuilder.Append(HyphenMinus);
                return;
            case var _ when !success:
                Emit(GetCommentToken(), new EndOfFileToken());
                return;
            default:
                _commentDataBuilder.Append(HyphenMinus);
                _commentDataBuilder.Append(HyphenMinus);
                ReconsumeIn(Comment);
                return;
        }
    }
    
    private void GetTokenFrom52CommentEndBangState()
    {
        var (success, codePoint) = ConsumeNextInputCharacter();
        
        switch (codePoint.Value)
        {
            case HyphenMinus:
                _commentDataBuilder.Append(HyphenMinus);
                _commentDataBuilder.Append(HyphenMinus);
                _commentDataBuilder.Append(ExclamationMark);
                SwitchTo(CommentEndDash);
                return;
            case GreaterThanSign:
                SwitchTo(Data);
                Emit(GetCommentToken());
                return;
            case var _ when !success:
                Emit(GetCommentToken(), new EndOfFileToken());
                return;
            default:
                _commentDataBuilder.Append(HyphenMinus);
                _commentDataBuilder.Append(HyphenMinus);
                _commentDataBuilder.Append(ExclamationMark);
                ReconsumeIn(Comment);
                return;
        }
    }
    
    private void GetTokenFrom53DoctypeState()
    {
        var (success, codePoint) = ConsumeNextInputCharacter();
        
        switch (codePoint.Value)
        {
            case CharacterTabulation:
            case LineFeed:
            case FormFeed:
            case Space:
                SwitchTo(BeforeDoctypeName);
                return;
            case GreaterThanSign:
                ReconsumeIn(BeforeDoctypeName);
                return;
            case var _ when !success:
                CreateNewDoctypeToken();
                _docTypeTokenBuilder.SetForceQuirks();
                Emit(GetDoctypeToken(), new EndOfFileToken());
                return;
            default:
                ReconsumeIn(BeforeDoctypeName);
                return;
        }
    }
    
    private void GetTokenFrom54BeforeDoctypeNameState()
    {
        var (success, codePoint) = ConsumeNextInputCharacter();
        
        switch (codePoint.Value)
        {
            case CharacterTabulation:
            case LineFeed:
            case FormFeed:
            case Space:
                return; // ignore
            case var _ when CharacterRangeReference.AsciiUpperAlpha.Contains(codePoint):
                CreateNewDoctypeToken();
                _docTypeTokenBuilder.AppendToName(codePoint.ToAsciiLower());
                SwitchTo(DoctypeName);
                return;
            case Null:
                CreateNewDoctypeToken();
                _docTypeTokenBuilder.AppendToName(UnicodeCodePoint.ReplacementCharacter);
                SwitchTo(DoctypeName);
                return;
            case GreaterThanSign:
                CreateNewDoctypeToken();
                _docTypeTokenBuilder.SetForceQuirks();
                SwitchTo(Data);
                Emit(GetDoctypeToken());
                return;
            case var _ when !success:
                CreateNewDoctypeToken();
                _docTypeTokenBuilder.SetForceQuirks();
                Emit(GetDoctypeToken(), new EndOfFileToken());
                return;
            default:
                CreateNewDoctypeToken();
                _docTypeTokenBuilder.AppendToName(codePoint);
                SwitchTo(DoctypeName);
                return;
        }
    }
    
    private void GetTokenFrom55DoctypeNameState()
    {
        var (success, codePoint) = ConsumeNextInputCharacter();
        
        switch (codePoint.Value)
        {
            case CharacterTabulation:
            case LineFeed:
            case FormFeed:
            case Space:
                SwitchTo(AfterDoctypeName);
                return;
            case GreaterThanSign:
                SwitchTo(Data);
                Emit(GetDoctypeToken());
                return;
            case var _ when CharacterRangeReference.AsciiUpperAlpha.Contains(codePoint):
                _docTypeTokenBuilder.AppendToName(codePoint.ToAsciiLower());
                return;
            case Null:
                _docTypeTokenBuilder.AppendToName(UnicodeCodePoint.ReplacementCharacter);
                return;
            case var _ when !success:
                _docTypeTokenBuilder.SetForceQuirks();
                Emit(GetDoctypeToken(), new EndOfFileToken());
                return;
            default:
                _docTypeTokenBuilder.AppendToName(codePoint);
                return;
        }
    }
    
    private void GetTokenFrom56AfterDoctypeNameState()
    {
        var (success, codePoint) = ConsumeNextInputCharacter();
        
        switch (codePoint.Value)
        {
            case CharacterTabulation:
            case LineFeed:
            case FormFeed:
            case Space:
                return; // ignore
            case GreaterThanSign:
                SwitchTo(Data);
                Emit(GetDoctypeToken());
                return;
            case var _ when !success:
                _docTypeTokenBuilder.SetForceQuirks();
                Emit(GetDoctypeToken(), new EndOfFileToken());
                return;
            default:
                var (lookAheadSuccess,  result) = LookAhead(StringReference.Public.Length, true);
                if (lookAheadSuccess)
                {
                    if (StringReference.AsciiCaseInsensitiveEquals(result, StringReference.Public))
                    {
                        ConsumeLookAhead(StringReference.Public.Length - 1); // current already consumed
                        SwitchTo(AfterDoctypePublicKeyword);
                        return;
                    }
                    
                    if (StringReference.AsciiCaseInsensitiveEquals(result, StringReference.System))
                    {
                        ConsumeLookAhead(StringReference.System.Length - 1); // current already consumed
                        SwitchTo(AfterDoctypeSystemKeyword);
                        return;
                    }
                }

                _docTypeTokenBuilder.SetForceQuirks();
                SwitchTo(BogusDoctype);
                return;
        }
    }
    
    private void GetTokenFrom57AfterDoctypePublicKeywordState()
    {
        var (success, codePoint) = ConsumeNextInputCharacter();
        
        switch (codePoint.Value)
        {
            case CharacterTabulation:
            case LineFeed:
            case FormFeed:
            case Space:
                SwitchTo(BeforeDoctypePublicIdentifier);
                return;
            case QuotationMark:
                _docTypeTokenBuilder.SetPublicIdentifierPresent();
                SwitchTo(DoctypePublicIdentifierDoubleQuoted);
                return;
            case Apostrophe:
                _docTypeTokenBuilder.SetPublicIdentifierPresent();
                SwitchTo(DoctypePublicIdentifierSingleQuoted);
                return;
            case GreaterThanSign:
                _docTypeTokenBuilder.SetForceQuirks();
                SwitchTo(Data);
                Emit(GetDoctypeToken());
                return;
            case var _ when !success:
                _docTypeTokenBuilder.SetForceQuirks();
                Emit(GetDoctypeToken(), new EndOfFileToken());
                return;
            default:
                _docTypeTokenBuilder.SetForceQuirks();
                ReconsumeIn(BogusDoctype);
                return;
        }
    }
    
    private void GetTokenFrom58BeforeDoctypePublicIdentifierState()
    {
        var (success, codePoint) = ConsumeNextInputCharacter();
        
        switch (codePoint.Value)
        {
            case CharacterTabulation:
            case LineFeed:
            case FormFeed:
            case Space:
                return; // ignore
            case QuotationMark:
                _docTypeTokenBuilder.SetPublicIdentifierPresent();
                SwitchTo(DoctypePublicIdentifierDoubleQuoted);
                return;
            case Apostrophe:
                _docTypeTokenBuilder.SetPublicIdentifierPresent();
                SwitchTo(DoctypePublicIdentifierSingleQuoted);
                return;
            case GreaterThanSign:
                _docTypeTokenBuilder.SetForceQuirks();
                SwitchTo(Data);
                Emit(GetDoctypeToken());
                return;
            case var _ when !success:
                _docTypeTokenBuilder.SetForceQuirks();
                Emit(GetDoctypeToken(), new EndOfFileToken());
                return;
            default:
                _docTypeTokenBuilder.SetForceQuirks();
                ReconsumeIn(BogusDoctype);
                return;
        }
    }
    
    private void GetTokenFrom59DoctypePublicIdentifierDoubleQuotedState()
    {
        var (success, codePoint) = ConsumeNextInputCharacter();
        
        switch (codePoint.Value)
        {
            case QuotationMark:
                SwitchTo(AfterDoctypePublicIdentifier);
                return;
            case Null:
                _docTypeTokenBuilder.AppendToPublicIdentifier(UnicodeCodePoint.ReplacementCharacter);
                return;
            case GreaterThanSign:
                _docTypeTokenBuilder.SetForceQuirks();
                SwitchTo(Data);
                Emit(GetDoctypeToken());
                return;
            case var _ when !success:
                _docTypeTokenBuilder.SetForceQuirks();
                Emit(GetDoctypeToken(), new EndOfFileToken());
                return;
            default:
                _docTypeTokenBuilder.AppendToPublicIdentifier(codePoint);
                return;
        }
    }
    
    private void GetTokenFrom60DoctypePublicIdentifierSingleQuotedState()
    {
        var (success, codePoint) = ConsumeNextInputCharacter();
        
        switch (codePoint.Value)
        {
            case Apostrophe:
                SwitchTo(AfterDoctypePublicIdentifier);
                return;
            case Null:
                _docTypeTokenBuilder.AppendToPublicIdentifier(UnicodeCodePoint.ReplacementCharacter);
                return;
            case GreaterThanSign:
                _docTypeTokenBuilder.SetForceQuirks();
                SwitchTo(Data);
                Emit(GetDoctypeToken());
                return;
            case var _ when !success:
                _docTypeTokenBuilder.SetForceQuirks();
                Emit(GetDoctypeToken(), new EndOfFileToken());
                return;
            default:
                _docTypeTokenBuilder.AppendToPublicIdentifier(codePoint);
                return;
        }
    }
    
    private void GetTokenFrom61AfterDoctypePublicIdentifierState()
    {
        var (success, codePoint) = ConsumeNextInputCharacter();
        
        switch (codePoint.Value)
        {
            case CharacterTabulation:
            case LineFeed:
            case FormFeed:
            case Space:
                SwitchTo(BetweenDoctypePublicAndSystemIdentifier);
                return;
            case GreaterThanSign:
                SwitchTo(Data);
                Emit(GetDoctypeToken());
                return;
            case QuotationMark:
                _docTypeTokenBuilder.SetSystemIdentifierPresent();
                SwitchTo(DoctypeSystemIdentifierDoubleQuoted);
                return;
            case Apostrophe:
                _docTypeTokenBuilder.SetSystemIdentifierPresent();
                SwitchTo(DoctypeSystemIdentifierSingleQuoted);
                return;
            case var _ when !success:
                _docTypeTokenBuilder.SetForceQuirks();
                Emit(GetDoctypeToken(), new EndOfFileToken());
                return;
            default:
                _docTypeTokenBuilder.SetForceQuirks();
                ReconsumeIn(BogusDoctype);
                return;
        }
    }
    
    private void GetTokenFrom62BetweenDoctypePublicAndSystemIdentifiersState()
    {
        var (success, codePoint) = ConsumeNextInputCharacter();
        
        switch (codePoint.Value)
        {
            case CharacterTabulation:
            case LineFeed:
            case FormFeed:
            case Space:
                return; // ignore
            case GreaterThanSign:
                SwitchTo(Data);
                Emit(GetDoctypeToken());
                return;
            case QuotationMark:
                _docTypeTokenBuilder.SetSystemIdentifierPresent();
                SwitchTo(DoctypeSystemIdentifierDoubleQuoted);
                return;
            case Apostrophe:
                _docTypeTokenBuilder.SetSystemIdentifierPresent();
                SwitchTo(DoctypeSystemIdentifierSingleQuoted);
                return;
            case var _ when !success:
                _docTypeTokenBuilder.SetForceQuirks();
                Emit(GetDoctypeToken(), new EndOfFileToken());
                return;
            default:
                _docTypeTokenBuilder.SetForceQuirks();
                ReconsumeIn(BogusDoctype);
                return;
        }
    }
    
    private void GetTokenFrom63AfterDoctypeSystemKeywordState()
    {
        var (success, codePoint) = ConsumeNextInputCharacter();
        
        switch (codePoint.Value)
        {
            case CharacterTabulation:
            case LineFeed:
            case FormFeed:
            case Space:
                SwitchTo(BeforeDoctypeSystemIdentifier);
                return;
            case QuotationMark:
                _docTypeTokenBuilder.SetSystemIdentifierPresent();
                SwitchTo(DoctypeSystemIdentifierDoubleQuoted);
                return;
            case Apostrophe:
                _docTypeTokenBuilder.SetSystemIdentifierPresent();
                SwitchTo(DoctypeSystemIdentifierSingleQuoted);
                return;
            case GreaterThanSign:
                _docTypeTokenBuilder.SetForceQuirks();
                SwitchTo(Data);
                Emit(GetDoctypeToken());
                return;
            case var _ when !success:
                _docTypeTokenBuilder.SetForceQuirks();
                Emit(GetDoctypeToken(), new EndOfFileToken());
                return;
            default:
                _docTypeTokenBuilder.SetForceQuirks();
                ReconsumeIn(BogusDoctype);
                return;
        }
    }
    
    private void GetTokenFrom64BeforeDoctypeSystemIdentifierState()
    {
        var (success, codePoint) = ConsumeNextInputCharacter();
        
        switch (codePoint.Value)
        {
            case CharacterTabulation:
            case LineFeed:
            case FormFeed:
            case Space:
                return; // ignore
            case QuotationMark:
                _docTypeTokenBuilder.SetSystemIdentifierPresent();
                SwitchTo(DoctypeSystemIdentifierDoubleQuoted);
                return;
            case Apostrophe:
                _docTypeTokenBuilder.SetSystemIdentifierPresent();
                SwitchTo(DoctypeSystemIdentifierSingleQuoted);
                return;
            case GreaterThanSign:
                _docTypeTokenBuilder.SetForceQuirks();
                SwitchTo(Data);
                Emit(GetDoctypeToken());
                return;
            case var _ when !success:
                _docTypeTokenBuilder.SetForceQuirks();
                Emit(GetDoctypeToken(), new EndOfFileToken());
                return;
            default:
                _docTypeTokenBuilder.SetForceQuirks();
                ReconsumeIn(BogusDoctype);
                return;
        }
    }
    
    private void GetTokenFrom65DoctypeSystemIdentifierDoubleQuotedState()
    {
        var (success, codePoint) = ConsumeNextInputCharacter();
        
        switch (codePoint.Value)
        {
            case QuotationMark:
                SwitchTo(AfterDoctypeSystemIdentifier);
                return;
            case Null:
                _docTypeTokenBuilder.AppendToSystemIdentifier(UnicodeCodePoint.ReplacementCharacter);
                return;
            case GreaterThanSign:
                _docTypeTokenBuilder.SetForceQuirks();
                SwitchTo(Data);
                Emit(GetDoctypeToken());
                return;
            case var _ when !success:
                _docTypeTokenBuilder.SetForceQuirks();
                Emit(GetDoctypeToken(), new EndOfFileToken());
                return;
            default:
                _docTypeTokenBuilder.AppendToSystemIdentifier(codePoint);
                return;
        }
    }
    
    private void GetTokenFrom66DoctypeSystemIdentifierSingleQuotedState()
    {
        var (success, codePoint) = ConsumeNextInputCharacter();
        
        switch (codePoint.Value)
        {
            case Apostrophe:
                SwitchTo(AfterDoctypeSystemIdentifier);
                return;
            case Null:
                _docTypeTokenBuilder.AppendToSystemIdentifier(UnicodeCodePoint.ReplacementCharacter);
                return;
            case GreaterThanSign:
                _docTypeTokenBuilder.SetForceQuirks();
                SwitchTo(Data);
                Emit(GetDoctypeToken());
                return;
            case var _ when !success:
                _docTypeTokenBuilder.SetForceQuirks();
                Emit(GetDoctypeToken(), new EndOfFileToken());
                return;
            default:
                _docTypeTokenBuilder.AppendToSystemIdentifier(codePoint);
                return;
        }
    }
    
    private void GetTokenFrom67AfterDoctypeSystemIdentifierState()
    {
        var (success, codePoint) = ConsumeNextInputCharacter();
        
        switch (codePoint.Value)
        {
            case CharacterTabulation:
            case LineFeed:
            case FormFeed:
            case Space:
                return; // ignore
            case GreaterThanSign:
                SwitchTo(Data);
                Emit(GetDoctypeToken());
                return;
            case var _ when !success:
                _docTypeTokenBuilder.SetForceQuirks();
                Emit(GetDoctypeToken(), new EndOfFileToken());
                return;
            default:
                ReconsumeIn(BogusDoctype);
                return;
        }
    }
    
    private void GetTokenFrom68BogusDoctypeState()
    {
        var (success, codePoint) = ConsumeNextInputCharacter();

        switch (codePoint.Value)
        {
            case GreaterThanSign:
                SwitchTo(Data);
                Emit(GetDoctypeToken());
                return;
            case Null:
                return; // ignore
            case var _ when !success:
                Emit(GetDoctypeToken(), new EndOfFileToken());
                return;
            default:
                return; // ignore
        }
    }

    private void GetTokenFrom72CharacterReferenceState()
    {
        _temporaryBuffer = new List<UnicodeCodePoint>();
        _temporaryBuffer.Add(new UnicodeCodePoint(Ampersand));
        
        var (_, codePoint) = ConsumeNextInputCharacter();

        switch (codePoint.Value)
        {
            case var _ when CharacterRangeReference.AsciiAlphaNumeric.Contains(codePoint):
                ReconsumeIn(NamedCharacterReference);
                return;
            case NumberSign:
                _temporaryBuffer.Add(codePoint);
                SwitchTo(NumericCharacterReference);
                return;
            default:
                FlushCodePointsConsumedAsACharacterReference();
                ReconsumeIn(_returnState);
                return;
        }
    }

    private void GetTokenFrom73NamedCharacterReferenceState()
    {
        var partialName = new string(new[] { Ampersand });
        var matchingNames = NamedEntityReference.Entities.Keys.Where(k => k.StartsWith(partialName, StringComparison.Ordinal)).ToArray();
        string? match = null;
        
        while (matchingNames.Length > 1)
        {
            var (success, result) = LookAhead(partialName.Length, true);
            if (!success)
            {
                matchingNames = Array.Empty<string>();
                break;
            }

            partialName = Ampersand + result;
            matchingNames = NamedEntityReference.Entities.Keys.Where(k => k.StartsWith(partialName, StringComparison.Ordinal)).ToArray();
            if (matchingNames.Contains(partialName))
            {
                match = partialName; // We've matched one reference, store in case further characters match nothing
            }
                
        }

        if (matchingNames.Length == 1 && (match is null || match != matchingNames[0]))
        {
            var possibleMatch = matchingNames[0];
            var (success, result) = LookAhead(possibleMatch.Length - 1, true);
            if (success && possibleMatch == Ampersand + result)
            {
                match = possibleMatch;
            }
        }

        if (match is not null)
        {
            var historical = false;
            if (ConsumedAsPartOfAnAttribute() && match[^1] != SemiColon)
            {
                var (success, result) = LookAhead(match.Length, true);
                if (success)
                {
                    var nextInputChar = result[^1];
                    historical = nextInputChar == EqualsSign || CharacterRangeReference.AsciiAlphaNumeric.Contains(new UnicodeCodePoint(nextInputChar));
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

            ConsumeLookAhead(match.Length - 1); 
            
            SwitchTo(_returnState);
        }
        else
        {
            SwitchTo(AmbiguousAmpersand);
        }

        FlushCodePointsConsumedAsACharacterReference();
    }

    private void GetTokenFrom74AmbiguousAmpersandState()
    {
        var (_, codePoint) = ConsumeNextInputCharacter();

        switch (codePoint.Value)
        {
            case var _ when CharacterRangeReference.AsciiAlphaNumeric.Contains(codePoint):
                if (ConsumedAsPartOfAnAttribute())
                    _tagTokenBuilder.AppendToAttributeValue(codePoint);
                else 
                    Emit(new CharacterToken { Data = codePoint.ToString() });
                return;
            case SemiColon:
                ReconsumeIn(_returnState);
                return;
            default:
                ReconsumeIn(_returnState);
                return;
        }
    }
    
    private void GetTokenFrom75NumericCharacterReferenceState()
    {
        _characterReferenceCode = 0;
        
        var (_, codePoint) = ConsumeNextInputCharacter();

        switch (codePoint.Value)
        {
            case LowerCaseX:
            case UpperCaseX:
                _temporaryBuffer.Add(codePoint);
                SwitchTo(HexadecimalCharacterReferenceStart);
                return;
            default:
                ReconsumeIn(DecimalCharacterReferenceStart);
                return;
        }
    }
    
    private void GetTokenFrom76HexadecimalCharacterReferenceStartState()
    {
        var (_, codePoint) = ConsumeNextInputCharacter();

        switch (codePoint.Value)
        {
            case var _ when CharacterRangeReference.AsciiHex.Contains(codePoint):
                ReconsumeIn(HexadecimalCharacterReference);
                return;
            default:
                FlushCodePointsConsumedAsACharacterReference();
                ReconsumeIn(_returnState);
                return;
        }
    }
    
    private void GetTokenFrom77DecimalCharacterReferenceStartState()
    {
        var (_, codePoint) = ConsumeNextInputCharacter();
        
        switch (codePoint.Value)
        {
            case var _ when CharacterRangeReference.AsciiDigit.Contains(codePoint):
                ReconsumeIn(DecimalCharacterReference);
                return;
            default:
                FlushCodePointsConsumedAsACharacterReference();
                ReconsumeIn(_returnState);
                return;
        }
    }
    
    private void GetTokenFrom78HexadecimalCharacterReferenceState()
    {
        var (_, codePoint) = ConsumeNextInputCharacter();

        switch (codePoint.Value)
        {
            case var _ when CharacterRangeReference.AsciiDigit.Contains(codePoint):
                ShiftAndIncrementCharacterReferenceCode(16, codePoint.Value - 0x30);
                return;
            case var _ when CharacterRangeReference.AsciiUpperHexLetter.Contains(codePoint):
                ShiftAndIncrementCharacterReferenceCode(16, codePoint.Value - 0x37);
                return;
            case var _ when CharacterRangeReference.AsciiLowerHexLetter.Contains(codePoint):
                ShiftAndIncrementCharacterReferenceCode(16, codePoint.Value - 0x57);
                return;
            case SemiColon:
                SwitchTo(NumericCharacterReferenceEnd);
                return;
            default:
                ReconsumeIn(NumericCharacterReferenceEnd);
                return;
        }
    }
    
    private void GetTokenFrom79DecimalCharacterReferenceState()
    {
        var (_, codePoint) = ConsumeNextInputCharacter();

        switch (codePoint.Value)
        {
            case var _ when CharacterRangeReference.AsciiDigit.Contains(codePoint):
                ShiftAndIncrementCharacterReferenceCode(10, codePoint.Value - 0x30);
                return;
            case SemiColon:
                SwitchTo(NumericCharacterReferenceEnd);
                return;
            default:
                ReconsumeIn(NumericCharacterReferenceEnd);
                return;
        }
    }
    
    private void GetTokenFrom80NumericCharacterReferenceEndState()
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
            _characterReferenceCode = ReplacementCharacter;
        } 
        else if (_characterReferenceCode > UnicodeCodePoint.MaxCodePoint)
        {
            _characterReferenceCode = ReplacementCharacter;
        }
        else if (IsCharacterReferenceCodeSurrogate())
        {
            _characterReferenceCode = ReplacementCharacter;
        }
        else if (charRefCodeMap.TryGetValue(_characterReferenceCode, out var replacementCodePoint))
        {
            _characterReferenceCode = replacementCodePoint;
        }

        _temporaryBuffer = new List<UnicodeCodePoint> { new UnicodeCodePoint(_characterReferenceCode) };
        FlushCodePointsConsumedAsACharacterReference();
        SwitchTo(_returnState);
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
            var tokens = _temporaryBuffer.Select(c => new CharacterToken { Data = c.ToString() }).Cast<HtmlToken>().ToArray();
            Emit(tokens);
        }
    }

    private bool ConsumedAsPartOfAnAttribute()
    {
        return _returnState is AttributeValueDoubleQuoted or AttributeValueSingleQuoted or AttributeValueUnquoted;
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

    private void Emit(params HtmlToken[] tokens)
    {
        // ReSharper disable once ForCanBeConvertedToForeach - absolutely ensure in order
        for (var i = 0; i < tokens.Length; i++)
            _tokensToEmit.Enqueue(tokens[i]);
    }

    private void SwitchTo(TokenParserState state) => _parserState = state;

    private void SetReturnStateTo(TokenParserState state) => _returnState = state;

    private void ReconsumeIn(TokenParserState state)
    {
        _reconsumeFlag = true;
        SwitchTo(state);
    }

    private (bool Success, UnicodeCodePoint codePoint) ConsumeNextInputCharacter()
    {
        if (_reconsumeFlag)
        {
            _reconsumeFlag = false;
            return _currentCharacter;
        }
        
        _currentCharacter = _streamConsumer.TryGetCurrentCodePoint();
        _streamConsumer.ConsumeCodePoint();

        return _currentCharacter;
    }

    private (bool Success, string Result) LookAhead(int codePointCount, bool includeCurrent)
    {
        var success = false;
        var result = "";
        if (codePointCount < 1)
            return (success, result);
        
        if (includeCurrent)
            codePointCount--;

        if (codePointCount > 0)
        {
            (success, result) = _streamConsumer.LookAhead(codePointCount);
            if (success && includeCurrent)
                result = _currentCharacter.CodePoint + result;
        }
        else
        {
            (success, result) = (_currentCharacter.Success, _currentCharacter.CodePoint.ToString());
        }
        
        return (success, result);
    }

    private void ConsumeLookAhead(int codePointCount)
    {
        if (_reconsumeFlag && codePointCount > 0)
        {
            codePointCount--;
            _reconsumeFlag = false;
        }
        if (codePointCount > 0)
            _streamConsumer.ConsumeCodePoint(codePointCount);
    }
}