using Newtonsoft.Json.Linq;

namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests;

public static class HtmlTokenGeneratorTestRunner
{
    
    internal static void Run(string input, IEnumerable<HtmlToken> expectedTokens)
    {
        // arrange
        var streamConsumer = new TestStreamConsumer(input);
        var tokenGenerator = new HtmlTokenGenerator(streamConsumer);
        var actual = new List<HtmlToken>();
        var expected = new List<HtmlToken>(expectedTokens)
        {
            new EndOfFileToken(),
            new EndOfFileToken()
        };

        // act
        while (actual.Count < expected.Count)
        {
            var nextToken = tokenGenerator.GetNextToken();
            actual.Add(nextToken);
        }
        
        // assert
        Assert.AreEqual(expected.Count, actual.Count);

        for (var i = 0; i < actual.Count; i++)
        {
            Assert.IsTrue(expected[i].AreValueEqual(actual[i]));
        }
    }

    internal static IEnumerable<HtmlToken> ConvertJsonToTokens(string json)
    {
        var jArray = JArray.Parse(json);
        var tokens = new List<HtmlToken>();

        foreach (var jToken in jArray)
        {
            if (jToken.Type != JTokenType.Object)
                throw new NotSupportedException();

            var jObject = (JObject) jToken;

            var type = jObject["type"];

            HtmlToken? newToken = type?.Value<string>() switch
            {
                "character" => new CharacterToken
                {
                    Data = jObject["data"]?.Value<string>(),
                },
                "comment" => new CommentToken
                {
                    Data = jObject["data"]?.Value<string>(),
                },
                "doctype" => new DocTypeToken
                {
                    Name = jObject["name"]?.Value<string>(),
                    ForceQuirks = jObject["forcequirks"]?.Value<bool>() ?? false,
                    PublicIdentifier = jObject["publicidentifier"]?.Value<string>(),
                    SystemIdentifier = jObject["systemidentifier"]?.Value<string>(),
                },
                _ => null
            };

            if (newToken is not null)
                tokens.Add(newToken);
        }

        return tokens.AsReadOnly();
    }
}