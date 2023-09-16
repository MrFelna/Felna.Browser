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
        var (success, character) = _streamConsumer.TryGetCurrentChar();

        if (!success)
            return new DocTypeToken {ForceQuirks = true};

        while (CharacterRangeReference.TokenWhiteSpace.Contains(character))
        {
            _streamConsumer.ConsumeChar();
            (success, character) = _streamConsumer.TryGetCurrentChar();

            if (!success)
                return new DocTypeToken {ForceQuirks = true};
        }
        
        // before DOCTYPE name state
        if (character == CharacterReference.GreaterThanSign)
        {
            _streamConsumer.ConsumeChar();
            return new DocTypeToken {ForceQuirks = true};
        }
        
        // DOCTYPE name state
        var doctypeNameBuilder = new StringBuilder();
        while (true)
        {
            if (CharacterRangeReference.TokenWhiteSpace.Contains(character))
            {
                break;
            }

            if (character == CharacterReference.GreaterThanSign)
            {
                _streamConsumer.ConsumeChar();
                return new DocTypeToken {Name = doctypeNameBuilder.ToString()};
            }

            if (character == CharacterReference.Null)
                doctypeNameBuilder.Append(CharacterReference.ReplacementCharacter);
            else
                doctypeNameBuilder.Append(CharacterReference.ToAsciiLower(character));
            
            _streamConsumer.ConsumeChar();
            (success, character) = _streamConsumer.TryGetCurrentChar();

            if (!success)
                return new DocTypeToken {ForceQuirks = true, Name = doctypeNameBuilder.ToString()};
        }
        var doctypeName = doctypeNameBuilder.ToString();

        // after DOCTYPE name state
        while (CharacterRangeReference.TokenWhiteSpace.Contains(character))
        {
            _streamConsumer.ConsumeChar();
            (success, character) = _streamConsumer.TryGetCurrentChar();

            if (!success)
                return new DocTypeToken {ForceQuirks = true, Name = doctypeName};
        }
        
        if (character == CharacterReference.GreaterThanSign)
        {
            _streamConsumer.ConsumeChar();
            return new DocTypeToken {Name = doctypeName};
        }

        (success, var result) = _streamConsumer.LookAhead(StringReference.System.Length);

        string? publicIdentifier = null;
        string? systemIdentifier = null;

        var expectPublicIdentifier = success && StringReference.AsciiCaseInsensitiveEquals(result, StringReference.Public);
        var expectSystemIdentifier = success && StringReference.AsciiCaseInsensitiveEquals(result, StringReference.System);
        var expectSystemAfterPublicIdentifier = false;

        if (!expectPublicIdentifier && !expectSystemIdentifier)
        {
            return GetBogusDoctypeToken(name: doctypeName, forceQuirks: true);
        }
        
        if (expectPublicIdentifier)
        {
            _streamConsumer.ConsumeChar(StringReference.System.Length);
            
            (success, character) = _streamConsumer.TryGetCurrentChar();

            if (!success)
                return new DocTypeToken {Name = doctypeName, ForceQuirks = true};
            
            // after DOCTYPE public keyword state
            while (CharacterRangeReference.TokenWhiteSpace.Contains(character))
            {
                _streamConsumer.ConsumeChar();
                (success, character) = _streamConsumer.TryGetCurrentChar();

                if (!success)
                    return new DocTypeToken {Name = doctypeName, ForceQuirks = true};
            }

            switch (character)
            {
                case CharacterReference.GreaterThanSign:
                    _streamConsumer.ConsumeChar();
                    return new DocTypeToken {Name = doctypeName, ForceQuirks = true};
                case CharacterReference.QuotationMark or CharacterReference.Apostrophe:
                {
                    var publicIdQuoteChar = character;
                    publicIdentifier = "";

                    do
                    {
                        _streamConsumer.ConsumeChar();
                        (success, character) = _streamConsumer.TryGetCurrentChar();

                        if (!success)
                            return new DocTypeToken {Name = doctypeName, PublicIdentifier = publicIdentifier, ForceQuirks = true};
                        if (character == publicIdQuoteChar)
                            break;
                        if (character == CharacterReference.GreaterThanSign)
                        {
                            _streamConsumer.ConsumeChar();
                            return new DocTypeToken {Name = doctypeName, PublicIdentifier = publicIdentifier, ForceQuirks = true};
                        }

                        if (character == CharacterReference.Null)
                        {
                            publicIdentifier += CharacterReference.ReplacementCharacter;
                        }
                        else
                        {
                            publicIdentifier += character;
                        }
                    } while (true);
                
                    _streamConsumer.ConsumeChar();
                    (success, character) = _streamConsumer.TryGetCurrentChar();

                    if (!success)
                        return new DocTypeToken {Name = doctypeName, PublicIdentifier = publicIdentifier, ForceQuirks = true};
                
                    // after DOCTYPE public identifier state
                    while (CharacterRangeReference.TokenWhiteSpace.Contains(character))
                    {
                        _streamConsumer.ConsumeChar();
                        (success, character) = _streamConsumer.TryGetCurrentChar();

                        if (!success)
                            return new DocTypeToken {Name = doctypeName, PublicIdentifier = publicIdentifier, ForceQuirks = true};
                    }

                    switch (character)
                    {
                        case CharacterReference.GreaterThanSign:
                            _streamConsumer.ConsumeChar();
                            return new DocTypeToken {Name = doctypeName, PublicIdentifier = publicIdentifier};
                        case CharacterReference.QuotationMark or CharacterReference.Apostrophe:
                            expectSystemAfterPublicIdentifier = true;
                            break;
                        default:
                            return GetBogusDoctypeToken(name: doctypeName, publicIdentifier: publicIdentifier, forceQuirks: true);
                    }

                    break;
                }
                default:
                    return GetBogusDoctypeToken(name: doctypeName, forceQuirks: true);
            }
        }
        
        if (expectSystemIdentifier || expectSystemAfterPublicIdentifier)
        {
            if (expectSystemIdentifier)
            {
                _streamConsumer.ConsumeChar(StringReference.System.Length);
                (success, character) = _streamConsumer.TryGetCurrentChar();

                if (!success)
                    return new DocTypeToken {Name = doctypeName, ForceQuirks = true};
            }
            
            while (CharacterRangeReference.TokenWhiteSpace.Contains(character))
            {
                _streamConsumer.ConsumeChar();
                (success, character) = _streamConsumer.TryGetCurrentChar();

                if (!success)
                    return new DocTypeToken {Name = doctypeName, ForceQuirks = true};
            }

            switch (character)
            {
                case CharacterReference.GreaterThanSign:
                    _streamConsumer.ConsumeChar();
                    return new DocTypeToken {Name = doctypeName, ForceQuirks = true};
                case CharacterReference.QuotationMark or CharacterReference.Apostrophe:
                {
                    var systemIdQuoteChar = character;
                    systemIdentifier = "";

                    do
                    {
                        _streamConsumer.ConsumeChar();
                        (success, character) = _streamConsumer.TryGetCurrentChar();

                        if (!success)
                            return new DocTypeToken {Name = doctypeName, PublicIdentifier = publicIdentifier, SystemIdentifier = systemIdentifier, ForceQuirks = true};
                        if (character == systemIdQuoteChar)
                            break;
                        if (character == CharacterReference.GreaterThanSign)
                        {
                            _streamConsumer.ConsumeChar();
                            return new DocTypeToken {Name = doctypeName, PublicIdentifier = publicIdentifier, SystemIdentifier = systemIdentifier, ForceQuirks = true};
                        }
                        if (character == CharacterReference.Null)
                        {
                            systemIdentifier += CharacterReference.ReplacementCharacter;
                        }
                        else
                        {
                            systemIdentifier += character;
                        }
                    } while (true);

                    _streamConsumer.ConsumeChar();
                    (success, character) = _streamConsumer.TryGetCurrentChar();

                    if (!success)
                        return new DocTypeToken {Name = doctypeName, PublicIdentifier = publicIdentifier, SystemIdentifier = systemIdentifier, ForceQuirks = true};
                    
                    while (CharacterRangeReference.TokenWhiteSpace.Contains(character))
                    {
                        _streamConsumer.ConsumeChar();
                        (success, character) = _streamConsumer.TryGetCurrentChar();

                        if (!success)
                            return new DocTypeToken {Name = doctypeName, PublicIdentifier = publicIdentifier, SystemIdentifier = systemIdentifier, ForceQuirks = true};
                    }
                    break;
                }
                default:
                    return GetBogusDoctypeToken(doctypeName, forceQuirks: true);
            }
        }
        
        // bogus DOCTYPE state
        var forceQuirks = !expectSystemIdentifier && !expectPublicIdentifier;
        return GetBogusDoctypeToken(doctypeName, publicIdentifier, systemIdentifier, forceQuirks);
    }

    private HtmlToken GetBogusDoctypeToken(string? name = null, string? publicIdentifier = null, string? systemIdentifier = null, bool forceQuirks = false)
    {
        do
        {
            var (success, character) = _streamConsumer.TryGetCurrentChar();
            if (!success)
                return new DocTypeToken {Name = name, PublicIdentifier = publicIdentifier, SystemIdentifier = systemIdentifier, ForceQuirks = forceQuirks};
            _streamConsumer.ConsumeChar();
            if (character == CharacterReference.GreaterThanSign)
                return new DocTypeToken {Name = name, PublicIdentifier = publicIdentifier, SystemIdentifier = systemIdentifier, ForceQuirks = forceQuirks};
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