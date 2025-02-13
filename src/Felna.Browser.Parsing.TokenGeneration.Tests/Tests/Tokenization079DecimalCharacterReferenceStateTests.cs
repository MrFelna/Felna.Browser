namespace Felna.Browser.Parsing.TokenGeneration.Tests.Tests;

[TestClass]
public class Tokenization079DecimalCharacterReferenceStateTests
{
    [TestMethod]
    // Digit
    [DataRow("&#32;", @"[{""type"":""character"",""data"":"" ""}]")]
    // Semi colon
    [DataRow("&#32;", @"[{""type"":""character"",""data"":"" ""}]")]
    // Anything else
    [DataRow("&#32", @"[{""type"":""character"",""data"":"" ""}]")]
    [DataRow("&#32p", @"[{""type"":""character"",""data"":"" ""},{""type"":""character"",""data"":""p""}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);

        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}