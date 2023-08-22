namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Tokenization006TagOpenStateTests
{
    [TestMethod]
    // TODO: Exclamation
    // TODO: Solidus
    // TODO: Alpha
    // Question mark
    [DataRow("<?", @"[{""type"":""comment"",""data"":""?""}]")]
    [DataRow("<?test>", @"[{""type"":""comment"",""data"":""?test""}]")]
    [DataRow("<?\u0000", @"[{""type"":""comment"",""data"":""?\ufffd""}]")]
    // EOF
    [DataRow("<", @"[{""type"":""character"",""data"":""<""}]")]
    // TODO: Anything else
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}