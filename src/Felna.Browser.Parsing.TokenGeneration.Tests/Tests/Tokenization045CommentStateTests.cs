namespace Felna.Browser.Parsing.TokenGeneration.Tests.Tests;

[TestClass]
public class Tokenization045CommentStateTests 
{
    [TestMethod]
    // Less-than sign
    [DataRow("<!--te<st-->", @"[{""type"":""comment"",""data"":""te<st""}]")]
    // Hyphen-minus
    [DataRow("<!--te-st-->", @"[{""type"":""comment"",""data"":""te-st""}]")]
    // NULL
    [DataRow("<!--te\u0000st-->", "[{\"type\":\"comment\",\"data\":\"te\ufffdst\"}]")]
    // EOF
    [DataRow("<!--test", @"[{""type"":""comment"",""data"":""test""}]")]
    // Anything else
    [DataRow("<!--test-->", @"[{""type"":""comment"",""data"":""test""}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}