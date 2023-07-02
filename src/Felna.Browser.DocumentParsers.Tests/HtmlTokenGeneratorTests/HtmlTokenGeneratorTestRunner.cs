using Newtonsoft.Json.Linq;

namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests;

public static class HtmlTokenGeneratorTestRunner
{
    
    internal static void Run(string input, IEnumerable<HtmlToken> expectedTokens)
    {
        // arrange
        var streamConsumer = new TestStreamConsumer(input);
        var tokenGenerator = new HtmlTokenGenerator(streamConsumer);
        var tokens = new List<HtmlToken>();
        var actual = new List<HtmlToken>(expectedTokens);
        actual.Add(new EndOfFileToken());
        actual.Add(new EndOfFileToken());
        
        // act
        while (tokens.Count < actual.Count)
        {
            var nextToken = tokenGenerator.GetNextToken();
            tokens.Add(nextToken);
        }
        
        // assert
        Assert.AreEqual(actual.Count, tokens.Count);

        for (var i = 0; i < tokens.Count; i++)
        {
            Assert.IsTrue(actual[i].AreValueEqual(tokens[i]));
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
            
            switch (type?.Value<string>())
            {
                case "comment":
                    tokens.Add(new CommentToken
                    {
                        Data = jObject["data"]?.Value<string>()
                    });
                    break;
            }
        }

        return tokens.AsReadOnly();
    }
}