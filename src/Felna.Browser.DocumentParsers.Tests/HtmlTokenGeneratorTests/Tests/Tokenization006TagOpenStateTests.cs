namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Tokenization006TagOpenStateTests
{
    [TestMethod]
    // TODO: Exclamation, Solidus, Alpha
    [DataRow("<?", @"[{""type"":""comment"",""data"":""?""}]")]
    [DataRow("<?test>", @"[{""type"":""comment"",""data"":""?test""}]")]
    [DataRow("<?\u0000", @"[{""type"":""comment"",""data"":""?\ufffd""}]")]
    [DataRow("<", @"[{""type"":""character"",""data"":""<""}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}