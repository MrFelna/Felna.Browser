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
            var tokens = row.Value.CodePoints
                .Select(char.ConvertFromUtf32)
                .Select(s => new CharacterToken { Data = s });
            
            HtmlTokenGeneratorTestRunner.Run(html, tokens);
        }
    }

    [TestMethod]
    public void EnsureAllNamedCharactersWithFollowingCharactersMatch()
    {
        foreach (var row in NamedEntityReference.Entities)
        {
            var html = row.Key + "a";
            var tokens = row.Value.CodePoints
                .Select(char.ConvertFromUtf32)
                .Select(s => new CharacterToken { Data = s })
                .Union(new[] { new CharacterToken { Data = "a" } });
            
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
    
    [TestMethod]
    public void EnsureAllNamedCharactersNotEndingInSemiColonDontMatchInAttributeValue()
    {
        foreach (var row in NamedEntityReference.Entities)
        {
            var attr = row.Key;
            if (attr.EndsWith(';'))
                continue;

            var html = $"<p a='{attr}='>"; // realistic example: href='index.html?gt=1'

            var token = new TagToken { Name = "p", IsEndTag = false };
            token.TokenAttributes.Add(new HtmlTokenAttribute{Name = "a", Value = attr});
            
            HtmlTokenGeneratorTestRunner.Run(html, new [] { token });

            html = $"<p a='{attr}z'>"; // realistic example: value='arts&ampcrafts'

            token = new TagToken { Name = "p", IsEndTag = false };
            token.TokenAttributes.Add(new HtmlTokenAttribute{Name = "a", Value = attr + "z"});
            
            HtmlTokenGeneratorTestRunner.Run(html, new [] { token });

            html = $"<p a='{attr}Z'>"; // realistic example: value='arts&ampcrafts'

            token = new TagToken { Name = "p", IsEndTag = false };
            token.TokenAttributes.Add(new HtmlTokenAttribute{Name = "a", Value = attr + "Z"});
            
            HtmlTokenGeneratorTestRunner.Run(html, new [] { token });

            html = $"<p a='{attr}1'>"; // realistic example: value='arts&ampcrafts'

            token = new TagToken { Name = "p", IsEndTag = false };
            token.TokenAttributes.Add(new HtmlTokenAttribute{Name = "a", Value = attr + "1"});
            
            HtmlTokenGeneratorTestRunner.Run(html, new [] { token });
        }
    }
}