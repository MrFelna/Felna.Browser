using System.Text;
using Felna.Browser.DocumentParsers.StreamConsumers;
using Felna.Browser.DocumentParsers.TextReferences;

namespace Felna.Browser.DocumentParsers.HtmlTokens;

internal class HtmlTokenGenerator
{
    private readonly IStreamConsumer _streamConsumer;

    internal HtmlTokenGenerator(IStreamConsumer streamConsumer)
    {
        _streamConsumer = streamConsumer;
    }

    internal HtmlToken GetNextToken() => GetDataToken();

    private HtmlToken GetDataToken()
    {
        var (success, character) = _streamConsumer.TryGetCurrentChar();
        
        if (!success)
            return new EndOfFileToken();

        if (character == CharacterReference.LessThanSign)
        {
            _streamConsumer.ConsumeChar();
            return GetTagOpenToken();
        }

        throw new NotImplementedException();
    }

    private HtmlToken GetTagOpenToken()
    {
        var (success, character) = _streamConsumer.TryGetCurrentChar();

        if (!success)
            return new CharacterToken {Data = new string(CharacterReference.LessThanSign, 1)};

        if (character == CharacterReference.ExclamationMark)
        {
            _streamConsumer.ConsumeChar();
            return GetMarkupDeclarationOpenToken();
        }

        if (character == CharacterReference.QuestionMark)
        {
            return GetBogusCommentToken();
        }

        throw new NotImplementedException();
    }

    private HtmlToken GetMarkupDeclarationOpenToken()
    {
        var (success, result) = _streamConsumer.LookAhead(2);

        if (success && result == "--")
        {
            throw new NotImplementedException("Comment start");
        }
        
        (success, result) = _streamConsumer.LookAhead(StringReference.DocType.Length);

        if (success && result == "[CDATA[")
        {
            throw new NotImplementedException("possible cdata section state");
        }

        if (success && StringReference.AsciiCaseInsensitiveEquals(result, StringReference.DocType))
        {
            _streamConsumer.ConsumeChar(StringReference.DocType.Length);
            return GetDocTypeToken();
        }

        return GetBogusCommentToken();
    }

    private HtmlToken GetDocTypeToken()
    {
        var docTypeTokenBuilder = new DocTypeTokenBuilder();
        
        var (success, character) = _streamConsumer.TryGetCurrentChar();

        if (!success)
            return docTypeTokenBuilder.SetForceQuirks().Build();

        while (CharacterRangeReference.TokenWhiteSpace.Contains(character))
        {
            _streamConsumer.ConsumeChar();
            (success, character) = _streamConsumer.TryGetCurrentChar();

            if (!success)
                return docTypeTokenBuilder.SetForceQuirks().Build();
        }
        
        // before DOCTYPE name state
        if (character == CharacterReference.GreaterThanSign)
        {
            _streamConsumer.ConsumeChar();
            return docTypeTokenBuilder.SetForceQuirks().Build();
        }
        
        // DOCTYPE name state
        while (true)
        {
            if (CharacterRangeReference.TokenWhiteSpace.Contains(character))
            {
                break;
            }

            if (character == CharacterReference.GreaterThanSign)
            {
                _streamConsumer.ConsumeChar();
                return docTypeTokenBuilder.Build();
            }

            if (character == CharacterReference.Null)
                docTypeTokenBuilder.AppendToName(CharacterReference.ReplacementCharacter);
            else
                docTypeTokenBuilder.AppendToName(CharacterReference.ToAsciiLower(character));
            
            _streamConsumer.ConsumeChar();
            (success, character) = _streamConsumer.TryGetCurrentChar();

            if (!success)
                return docTypeTokenBuilder.SetForceQuirks().Build();
        }

        // after DOCTYPE name state
        while (CharacterRangeReference.TokenWhiteSpace.Contains(character))
        {
            _streamConsumer.ConsumeChar();
            (success, character) = _streamConsumer.TryGetCurrentChar();

            if (!success)
                return docTypeTokenBuilder.SetForceQuirks().Build();
        }
        
        if (character == CharacterReference.GreaterThanSign)
        {
            _streamConsumer.ConsumeChar();
            return docTypeTokenBuilder.Build();
        }

        (success, var result) = _streamConsumer.LookAhead(StringReference.System.Length);

        var expectPublicIdentifier = success && StringReference.AsciiCaseInsensitiveEquals(result, StringReference.Public);
        var expectSystemIdentifier = success && StringReference.AsciiCaseInsensitiveEquals(result, StringReference.System);
        var expectSystemAfterPublicIdentifier = false;

        if (!expectPublicIdentifier && !expectSystemIdentifier)
        {
            return GetBogusDoctypeToken(docTypeTokenBuilder.SetForceQuirks());
        }
        
        if (expectPublicIdentifier)
        {
            _streamConsumer.ConsumeChar(StringReference.System.Length);
            
            (success, character) = _streamConsumer.TryGetCurrentChar();

            if (!success)
                return docTypeTokenBuilder.SetForceQuirks().Build();
            
            // after DOCTYPE public keyword state
            while (CharacterRangeReference.TokenWhiteSpace.Contains(character))
            {
                _streamConsumer.ConsumeChar();
                (success, character) = _streamConsumer.TryGetCurrentChar();

                if (!success)
                    return docTypeTokenBuilder.SetForceQuirks().Build();
            }

            switch (character)
            {
                case CharacterReference.GreaterThanSign:
                    _streamConsumer.ConsumeChar();
                    return docTypeTokenBuilder.SetForceQuirks().Build();
                case CharacterReference.QuotationMark or CharacterReference.Apostrophe:
                {
                    var publicIdQuoteChar = character;
                    docTypeTokenBuilder.SetPublicIdentifierPresent();

                    do
                    {
                        _streamConsumer.ConsumeChar();
                        (success, character) = _streamConsumer.TryGetCurrentChar();

                        if (!success)
                            return docTypeTokenBuilder.SetForceQuirks().Build();
                        if (character == publicIdQuoteChar)
                            break;
                        if (character == CharacterReference.GreaterThanSign)
                        {
                            _streamConsumer.ConsumeChar();
                            return docTypeTokenBuilder.SetForceQuirks().Build();
                        }

                        if (character == CharacterReference.Null)
                        {
                            docTypeTokenBuilder.AppendToPublicIdentifier(CharacterReference.ReplacementCharacter);
                        }
                        else
                        {
                            docTypeTokenBuilder.AppendToPublicIdentifier(character);
                        }
                    } while (true);
                
                    _streamConsumer.ConsumeChar();
                    (success, character) = _streamConsumer.TryGetCurrentChar();

                    if (!success)
                        return docTypeTokenBuilder.SetForceQuirks().Build();
                
                    // after DOCTYPE public identifier state
                    while (CharacterRangeReference.TokenWhiteSpace.Contains(character))
                    {
                        _streamConsumer.ConsumeChar();
                        (success, character) = _streamConsumer.TryGetCurrentChar();

                        if (!success)
                            return docTypeTokenBuilder.SetForceQuirks().Build();
                    }

                    switch (character)
                    {
                        case CharacterReference.GreaterThanSign:
                            _streamConsumer.ConsumeChar();
                            return docTypeTokenBuilder.Build();
                        case CharacterReference.QuotationMark or CharacterReference.Apostrophe:
                            expectSystemAfterPublicIdentifier = true;
                            break;
                        default:
                            return GetBogusDoctypeToken(docTypeTokenBuilder.SetForceQuirks());
                    }

                    break;
                }
                default:
                    return GetBogusDoctypeToken(docTypeTokenBuilder.SetForceQuirks());
            }
        }
        
        if (expectSystemIdentifier || expectSystemAfterPublicIdentifier)
        {
            if (expectSystemIdentifier)
            {
                _streamConsumer.ConsumeChar(StringReference.System.Length);
                (success, character) = _streamConsumer.TryGetCurrentChar();

                if (!success)
                    return docTypeTokenBuilder.SetForceQuirks().Build();
            }
            
            while (CharacterRangeReference.TokenWhiteSpace.Contains(character))
            {
                _streamConsumer.ConsumeChar();
                (success, character) = _streamConsumer.TryGetCurrentChar();

                if (!success)
                    return docTypeTokenBuilder.SetForceQuirks().Build();
            }

            switch (character)
            {
                case CharacterReference.GreaterThanSign:
                    _streamConsumer.ConsumeChar();
                    return docTypeTokenBuilder.SetForceQuirks().Build();
                case CharacterReference.QuotationMark or CharacterReference.Apostrophe:
                {
                    var systemIdQuoteChar = character;
                    docTypeTokenBuilder.SetSystemIdentifierPresent();

                    do
                    {
                        _streamConsumer.ConsumeChar();
                        (success, character) = _streamConsumer.TryGetCurrentChar();

                        if (!success)
                            return docTypeTokenBuilder.SetForceQuirks().Build();
                        if (character == systemIdQuoteChar)
                            break;
                        if (character == CharacterReference.GreaterThanSign)
                        {
                            _streamConsumer.ConsumeChar();
                            return docTypeTokenBuilder.SetForceQuirks().Build();
                        }
                        if (character == CharacterReference.Null)
                        {
                            docTypeTokenBuilder.AppendToSystemIdentifier(CharacterReference.ReplacementCharacter);
                        }
                        else
                        {
                            docTypeTokenBuilder.AppendToSystemIdentifier(character);
                        }
                    } while (true);

                    _streamConsumer.ConsumeChar();
                    (success, character) = _streamConsumer.TryGetCurrentChar();

                    if (!success)
                        return docTypeTokenBuilder.SetForceQuirks().Build();
                    
                    while (CharacterRangeReference.TokenWhiteSpace.Contains(character))
                    {
                        _streamConsumer.ConsumeChar();
                        (success, character) = _streamConsumer.TryGetCurrentChar();

                        if (!success)
                            return docTypeTokenBuilder.SetForceQuirks().Build();
                    }
                    break;
                }
                default:
                    return GetBogusDoctypeToken(docTypeTokenBuilder.SetForceQuirks());
            }
        }
        
        // bogus DOCTYPE state
        return GetBogusDoctypeToken(docTypeTokenBuilder);
    }

    private HtmlToken GetBogusDoctypeToken(DocTypeTokenBuilder docTypeTokenBuilder)
    {
        var docTypeToken = docTypeTokenBuilder.Build();
        do
        {
            var (success, character) = _streamConsumer.TryGetCurrentChar();
            if (!success)
                return docTypeToken;
            _streamConsumer.ConsumeChar();
            if (character == CharacterReference.GreaterThanSign)
                return docTypeToken;
        } while (true);
    }

    private HtmlToken GetBogusCommentToken()
    {
        var commentDataBuilder = new StringBuilder();
        while(true)
        {
            var (success, character) = _streamConsumer.TryGetCurrentChar();

            if (!success)
                return new CommentToken {Data = commentDataBuilder.ToString()};
            
            _streamConsumer.ConsumeChar();
            
            if (character == CharacterReference.GreaterThanSign)
                return new CommentToken {Data = commentDataBuilder.ToString()};

            if (character == CharacterReference.Null)
                commentDataBuilder.Append(CharacterReference.ReplacementCharacter);
            else
                commentDataBuilder.Append(character);
        }
    }
}