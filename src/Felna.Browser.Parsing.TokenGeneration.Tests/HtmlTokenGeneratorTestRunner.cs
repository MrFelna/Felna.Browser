using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Felna.Browser.Parsing.TokenGeneration.Tests;

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
        var assertMessage = $"Expected {GetTokenTypeList(expected)} - Actual {GetTokenTypeList(actual)}";
        Assert.AreEqual(expected.Count, actual.Count, assertMessage);

        for (var i = 0; i < actual.Count; i++)
        {
            TestAreEqual(expected[i], actual[i]);
        }
    }

    private static void TestAreEqual(HtmlToken expectedToken, HtmlToken actualToken)
    {
        var expectedType = expectedToken.GetType();
        Assert.AreEqual(expectedType, actualToken.GetType());

        var properties = expectedType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (var property in properties.Where(p => p.Name != nameof(HtmlToken.TokenAttributes)))
        {
            Assert.AreEqual(property.GetValue(expectedToken), property.GetValue(actualToken), $"Property {property.Name} differs for {expectedType.Name}");
        }

        var expectedAttributes = expectedToken.TokenAttributes
            .OrderBy(ta => ta.Name?.ToUpperInvariant())
            .ThenBy(ta => ta.Value?.ToUpperInvariant())
            .ToList();

        var actualAttributes = expectedToken.TokenAttributes
            .OrderBy(ta => ta.Name?.ToUpperInvariant())
            .ThenBy(ta => ta.Value?.ToUpperInvariant())
            .ToList();
            
        var message = $"Expected {GetTokenAttributeList(expectedAttributes)} - Actual {GetTokenAttributeList(actualAttributes)} for {expectedType.Name}";
        Assert.AreEqual(expectedToken.TokenAttributes.Count, actualToken.TokenAttributes.Count, message);

        for (var i = 0; i < expectedAttributes.Count; i++)
        {
            Assert.AreEqual(expectedAttributes[i].Name, actualAttributes[i].Name, message);
            Assert.AreEqual(expectedAttributes[i].Value, actualAttributes[i].Value, message);
        }
    }

    private static string GetTokenTypeList(IEnumerable<HtmlToken> tokens)
    {
        return string.Join(',', tokens.Select(t => t.GetType().Name));
    }
    
    private static string GetTokenAttributeList(IEnumerable<HtmlTokenAttribute> attributes)
    {
        return string.Join(',', attributes.Select(ta => ta.Name + ":" + ta.Value));
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
                "tag" => new TagToken
                {
                    Name = jObject["name"]?.Value<string>() ?? "",
                    IsEndTag = jObject["isendtag"]?.Value<bool>() ?? false,
                    SelfClosing = jObject["selfclosing"]?.Value<bool>() ?? false,
                },
                _ => null
            };

            if (newToken is not null)
            {
                if (jObject["attributes"] is JObject attrObject)
                {
                    foreach (var property in attrObject.Properties())
                    {
                        var attribute = new HtmlTokenAttribute
                        {
                            Name = property.Name, 
                            Value = property.Value.Value<string>() ?? string.Empty,
                        };
                        newToken.TokenAttributes.Add(attribute);
                    }
                }
                tokens.Add(newToken);
            }
        }

        return tokens.AsReadOnly();
    }
}