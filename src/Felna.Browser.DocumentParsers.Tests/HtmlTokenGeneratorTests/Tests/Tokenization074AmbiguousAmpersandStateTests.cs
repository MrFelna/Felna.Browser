namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

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

[TestClass]
public class Tokenization075NumericCharacterReferenceStateTests
{
    [TestMethod]
    // X
    [DataRow("&#x20;", @"[{""type"":""character"",""data"":"" ""}]")]
    // Anything else
    [DataRow("&#", @"[{""type"":""character"",""data"":""&""},{""type"":""character"",""data"":""#""}]")]
    [DataRow("&#32;", @"[{""type"":""character"",""data"":"" ""}]")]
    [DataRow("&#p", @"[{""type"":""character"",""data"":""&""},{""type"":""character"",""data"":""#""},{""type"":""character"",""data"":""p""}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);

        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}