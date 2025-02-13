namespace Felna.Browser.Parsing.TokenGeneration.Tests.Tests;

[TestClass]
public class Tokenization072CharacterReferenceStateTests
{
    [TestMethod]
    // ASCII alphanumeric
    [DataRow("&pound;", @"[{""type"":""character"",""data"":""£""}]")]
    // Number sign
    [DataRow("&#x00A3;", @"[{""type"":""character"",""data"":""£""}]")]
    [DataRow("&#163;", @"[{""type"":""character"",""data"":""£""}]")]
    // Anything else
    [DataRow("&", @"[{""type"":""character"",""data"":""&""}]")]
    [DataRow("&&", @"[{""type"":""character"",""data"":""&""},{""type"":""character"",""data"":""&""}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}