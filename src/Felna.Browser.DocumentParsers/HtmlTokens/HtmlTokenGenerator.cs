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
            return new CommentTokenGenerator(_streamConsumer).GetBogusComment();
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
            return new DocTypeTokenGenerator(_streamConsumer).GetDocTypeToken();
        }

        return new CommentTokenGenerator(_streamConsumer).GetBogusComment();
    }
}