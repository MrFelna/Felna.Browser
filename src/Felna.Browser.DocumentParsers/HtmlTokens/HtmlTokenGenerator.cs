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

        return new CharacterToken {Data = new string(character, 1)};
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
        
        return new CharacterToken {Data = new string(CharacterReference.LessThanSign, 1)};
    }

    private HtmlToken GetMarkupDeclarationOpenToken()
    {
        var (success, result) = _streamConsumer.LookAhead(StringReference.DocType.Length);

        if (success && StringReference.AsciiCaseInsensitiveEquals(result, StringReference.DocType))
        {
            _streamConsumer.ConsumeChar(StringReference.DocType.Length);
            return GetDocTypeToken();
        }

        throw new NotImplementedException();
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
                throw new NotImplementedException();

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
    }
}