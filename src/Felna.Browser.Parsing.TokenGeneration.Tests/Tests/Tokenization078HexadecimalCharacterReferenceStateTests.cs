namespace Felna.Browser.Parsing.TokenGeneration.Tests.Tests;

[TestClass]
public class Tokenization078HexadecimalCharacterReferenceStateTests
{
    [TestMethod]
    // Digit
    [DataRow("&#x20;", @"[{""type"":""character"",""data"":"" ""}]")]
    // Upper Hex
    [DataRow("&#x2B;", @"[{""type"":""character"",""data"":""+""}]")]
    [DataRow("&#xA3;", @"[{""type"":""character"",""data"":""£""}]")]
    // Lower hex
    [DataRow("&#x2b;", @"[{""type"":""character"",""data"":""+""}]")]
    [DataRow("&#xa3;", @"[{""type"":""character"",""data"":""£""}]")]
    // Semi colon
    [DataRow("&#x20;", @"[{""type"":""character"",""data"":"" ""}]")]
    // Anything else
    [DataRow("&#x20", @"[{""type"":""character"",""data"":"" ""}]")]
    [DataRow("&#x20p", @"[{""type"":""character"",""data"":"" ""},{""type"":""character"",""data"":""p""}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);

        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}