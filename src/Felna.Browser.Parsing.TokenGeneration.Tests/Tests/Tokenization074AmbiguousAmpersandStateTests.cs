namespace Felna.Browser.Parsing.TokenGeneration.Tests.Tests;

[TestClass]
public class Tokenization074AmbiguousAmpersandStateTests
{
    [TestMethod]
    // TODO Consumed as part of an attribute
    // ASCII alphanumeric
    [DataRow("&p1", @"[{""type"":""character"",""data"":""&""},{""type"":""character"",""data"":""p""},{""type"":""character"",""data"":""1""}]")]
    // Semi colon
    [DataRow("&p;", @"[{""type"":""character"",""data"":""&""},{""type"":""character"",""data"":""p""},{""type"":""character"",""data"":"";""}]")]
    // Anything else
    [DataRow("&p&", @"[{""type"":""character"",""data"":""&""},{""type"":""character"",""data"":""p""},{""type"":""character"",""data"":""&""}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);

        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}