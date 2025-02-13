namespace Felna.Browser.Parsing.TokenGeneration.Tests.Tests;

[TestClass]
public class Tokenization077DecimalCharacterReferenceStartStateTests
{
    [TestMethod]
    // Hex
    [DataRow("&#32;", @"[{""type"":""character"",""data"":"" ""}]")]
    // Anything else
    [DataRow("&#", @"[{""type"":""character"",""data"":""&""},{""type"":""character"",""data"":""#""}]")]
    [DataRow("&#;", @"[{""type"":""character"",""data"":""&""},{""type"":""character"",""data"":""#""},{""type"":""character"",""data"":"";""}]")]
    [DataRow("&#p", @"[{""type"":""character"",""data"":""&""},{""type"":""character"",""data"":""#""},{""type"":""character"",""data"":""p""}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);

        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}