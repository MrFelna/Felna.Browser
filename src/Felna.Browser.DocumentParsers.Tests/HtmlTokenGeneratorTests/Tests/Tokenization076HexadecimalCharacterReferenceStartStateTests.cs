namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Tokenization076HexadecimalCharacterReferenceStartStateTests
{
    [TestMethod]
    // Hex
    [DataRow("&#x20;", @"[{""type"":""character"",""data"":"" ""}]")]
    // Anything else
    [DataRow("&#x", @"[{""type"":""character"",""data"":""&""},{""type"":""character"",""data"":""#""},{""type"":""character"",""data"":""x""}]")]
    [DataRow("&#x;", @"[{""type"":""character"",""data"":""&""},{""type"":""character"",""data"":""#""},{""type"":""character"",""data"":""x""},{""type"":""character"",""data"":"";""}]")]
    [DataRow("&#xp", @"[{""type"":""character"",""data"":""&""},{""type"":""character"",""data"":""#""},{""type"":""character"",""data"":""x""},{""type"":""character"",""data"":""p""}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);

        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}