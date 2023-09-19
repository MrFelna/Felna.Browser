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

    public HtmlToken GetComment()
    {
        if (_commentToken is not null)
            return _commentToken;

        var token = GetCommentTokenInStartState();
        _commentToken = token;
        return token;
    }

    public HtmlToken GetBogusComment()
    {
        if (_commentToken is not null)
            return _commentToken;

        var token = GetCommentTokenInBogusState();
        _commentToken = token;
        return token;
    }

    private HtmlToken ConstructCommentToken() => new CommentToken { Data = _commentDataBuilder.ToString() };
    
    private HtmlToken GetCommentTokenInBogusState()
    {
        while(true)
        {
            var (success, character) = _streamConsumer.TryGetCurrentChar();

            if (!success)
                return ConstructCommentToken();
            
            switch (character)
            {
                case CharacterReference.GreaterThanSign:
                    _streamConsumer.ConsumeChar();
                    return ConstructCommentToken();
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

    private HtmlToken GetCommentTokenInStartState()
    {
        while(true)
        {
            var (success, character) = _streamConsumer.TryGetCurrentChar();

            if (!success)
                return GetCommentTokenInCommentState();
            
            switch (character)
            {
                case CharacterReference.HyphenMinus:
                    _streamConsumer.ConsumeChar();
                    return GetCommentTokenInStartDashState();
                case CharacterReference.GreaterThanSign:
                    _streamConsumer.ConsumeChar();
                    return ConstructCommentToken();
                default:
                    return GetCommentTokenInCommentState();
            }
        }
    }

    private HtmlToken GetCommentTokenInStartDashState()
    {
        var (success, character) = _streamConsumer.TryGetCurrentChar();

        if (!success)
            return ConstructCommentToken();
        
        switch (character)
        {
            case CharacterReference.HyphenMinus:
                _streamConsumer.ConsumeChar();
                return GetCommentTokenInEndState();
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeChar();
                return ConstructCommentToken();
            default:
                _commentDataBuilder.Append(CharacterReference.HyphenMinus);
                return GetCommentTokenInCommentState();
        }
    }

    private HtmlToken GetCommentTokenInCommentState()
    {
        while(true)
        {
            var (success, character) = _streamConsumer.TryGetCurrentChar();

            if (!success)
                return ConstructCommentToken();
            
            switch (character)
            {
                case CharacterReference.LessThanSign:
                    _streamConsumer.ConsumeChar();
                    _commentDataBuilder.Append(character);
                    return GetCommentTokenInLessThanSignState();
                case CharacterReference.HyphenMinus:
                    _streamConsumer.ConsumeChar();
                    return GetCommentTokenInEndDashState();
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

    private HtmlToken GetCommentTokenInLessThanSignState()
    {
        while(true)
        {
            var (success, character) = _streamConsumer.TryGetCurrentChar();

            if (!success)
                return GetCommentTokenInCommentState();
            
            switch (character)
            {
                case CharacterReference.ExclamationMark:
                    _streamConsumer.ConsumeChar();
                    _commentDataBuilder.Append(character);
                    return GetCommentTokenInLessThanSignBangState();
                case CharacterReference.LessThanSign:
                    _streamConsumer.ConsumeChar();
                    _commentDataBuilder.Append(character);
                    break;
                default:
                    return GetCommentTokenInCommentState();
            }
        }
    }

    private HtmlToken GetCommentTokenInLessThanSignBangState()
    {
        var (success, character) = _streamConsumer.TryGetCurrentChar();

        if (!success)
            return GetCommentTokenInCommentState();
        
        switch (character)
        {
            case CharacterReference.HyphenMinus:
                _streamConsumer.ConsumeChar();
                return GetCommentTokenInLessThanSignBangDashState();
            default:
                return GetCommentTokenInCommentState();
        }
    }

    private HtmlToken GetCommentTokenInLessThanSignBangDashState()
    {
        var (success, character) = _streamConsumer.TryGetCurrentChar();

        if (!success)
            return GetCommentTokenInCommentState();
        
        switch (character)
        {
            case CharacterReference.HyphenMinus:
                _streamConsumer.ConsumeChar();
                return GetCommentTokenInLessThanSignBangDashDashState();
            default:
                return GetCommentTokenInCommentState();
        }
    }

    private HtmlToken GetCommentTokenInLessThanSignBangDashDashState()
    {
        return GetCommentTokenInEndState();
    }

    private HtmlToken GetCommentTokenInEndDashState()
    {
        var (success, character) = _streamConsumer.TryGetCurrentChar();

        if (!success)
            return ConstructCommentToken();
        
        switch (character)
        {
            case CharacterReference.HyphenMinus:
                _streamConsumer.ConsumeChar();
                return GetCommentTokenInEndState();
            default:
                _commentDataBuilder.Append(CharacterReference.HyphenMinus);
                return GetCommentTokenInCommentState();
        }
    }

    private HtmlToken GetCommentTokenInEndState()
    {
        while(true)
        {
            var (success, character) = _streamConsumer.TryGetCurrentChar();

            if (!success)
                return ConstructCommentToken();
            
            switch (character)
            {
                case CharacterReference.GreaterThanSign:
                    _streamConsumer.ConsumeChar();
                    return ConstructCommentToken();
                case CharacterReference.ExclamationMark:
                    _streamConsumer.ConsumeChar();
                    return GetCommentTokenInEndBangState();
                case CharacterReference.HyphenMinus:
                    _streamConsumer.ConsumeChar();
                    _commentDataBuilder.Append(character);
                    break;
                default:
                    _commentDataBuilder.Append(CharacterReference.HyphenMinus).Append(CharacterReference.HyphenMinus);
                    return GetCommentTokenInCommentState();
            }
        }
    }

    private HtmlToken GetCommentTokenInEndBangState()
    {
        var (success, character) = _streamConsumer.TryGetCurrentChar();

        if (!success)
            return ConstructCommentToken();
        
        switch (character)
        {
            case CharacterReference.HyphenMinus:
                _streamConsumer.ConsumeChar();
                _commentDataBuilder
                    .Append(CharacterReference.HyphenMinus)
                    .Append(CharacterReference.HyphenMinus)
                    .Append(CharacterReference.ExclamationMark);
                return GetCommentTokenInEndDashState();
            case CharacterReference.GreaterThanSign:
                _streamConsumer.ConsumeChar();
                return ConstructCommentToken();
            default:
                _streamConsumer.ConsumeChar();
                _commentDataBuilder
                    .Append(CharacterReference.HyphenMinus)
                    .Append(CharacterReference.HyphenMinus)
                    .Append(CharacterReference.ExclamationMark);
                return GetCommentTokenInCommentState();
        }
    }
}