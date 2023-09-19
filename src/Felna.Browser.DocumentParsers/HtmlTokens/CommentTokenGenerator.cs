using System.Text;
using Felna.Browser.DocumentParsers.StreamConsumers;
using Felna.Browser.DocumentParsers.TextReferences;

namespace Felna.Browser.DocumentParsers.HtmlTokens;

internal class CommentTokenGenerator
{   
    private readonly IStreamConsumer _streamConsumer;
    private readonly StringBuilder _commentDataBuilder;
    private HtmlToken? _commentToken;

    public CommentTokenGenerator(IStreamConsumer streamConsumer)
    {
        _streamConsumer = streamConsumer;
        _commentDataBuilder = new StringBuilder();
    }

    public HtmlToken GetBogusComment()
    {
        if (_commentToken is not null)
            return _commentToken;

        var token = GetCommentTokenInBogusState();
        _commentToken = token;
        return token;
    }
    
    private HtmlToken GetCommentTokenInBogusState()
    {
        while(true)
        {
            var (success, character) = _streamConsumer.TryGetCurrentChar();

            if (!success)
                return new CommentToken {Data = _commentDataBuilder.ToString()};
            
            switch (character)
            {
                case CharacterReference.GreaterThanSign:
                    _streamConsumer.ConsumeChar();
                    return new CommentToken { Data = _commentDataBuilder.ToString() };
                case CharacterReference.Null:
                    _streamConsumer.ConsumeChar();
                    _commentDataBuilder.Append(CharacterReference.ReplacementCharacter);
                    break;
                default:
                    _streamConsumer.ConsumeChar();
                    _commentDataBuilder.Append(character);
                    break;
            }
        }
    }
}