using Felna.Browser.DocumentParsers.TextReferences;

namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Tokenization073NamedCharacterReferenceStateTests
{
    [TestMethod]
    // TODO Match with historical condition
    // Match 
    [DataRow("&pound;", @"[{""type"":""character"",""data"":""£""}]")]
    // Otherwise
    [DataRow("&p", @"[{""type"":""character"",""data"":""&""}, {""type"":""character"",""data"":""p""}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }

    [TestMethod]
    public void EnsureAllNamedCharactersMatch()
    {
        foreach (var row in NamedEntityReference.Entities)
        {
            var html = row.Key;
            var tokens = new[] { new CharacterToken { Data = row.Value.Characters } };
            
            HtmlTokenGeneratorTestRunner.Run(html, tokens);
        }
    }

    [TestMethod]
    public void EnsureAllNamedCharactersWithFollowingCharactersMatch()
    {
        foreach (var row in NamedEntityReference.Entities)
        {
            var html = row.Key + "a";
            var tokens = new[]
            {
                new CharacterToken { Data = row.Value.Characters },
                new CharacterToken { Data = "a" },
            };
            
            HtmlTokenGeneratorTestRunner.Run(html, tokens);
        }
    }

    [TestMethod]
    public void EnsurePartialsOfAllNamedCharactersDontMatch()
    {
        foreach (var row in NamedEntityReference.Entities)
        {
            var html = "";
            for (var i = 1; i < row.Key.Length; i++)
            {
                var partial = row.Key[..i]; // first i characters
                if (NamedEntityReference.Entities.ContainsKey(partial))
                    break;
                html = partial;
            }
            
            var tokens = html.Select(c => new CharacterToken { Data = c.ToString() });
            
            HtmlTokenGeneratorTestRunner.Run(html, tokens);
        }
    }
}