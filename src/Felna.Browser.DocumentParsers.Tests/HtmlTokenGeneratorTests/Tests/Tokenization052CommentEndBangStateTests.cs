namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Tokenization052CommentEndBangStateTests 
{
    [TestMethod]
    // Hyphen-minus
    [DataRow("<!----!-", @"[{""type"":""comment"",""data"":""--!""}]")]
    // Greater-than sign
    [DataRow("<!----!>", @"[{""type"":""comment"",""data"":""""}]")]
    // EOF
    [DataRow("<!----!", @"[{""type"":""comment"",""data"":""""}]")]
    // Anything else
    [DataRow("<!----!2", @"[{""type"":""comment"",""data"":""--!2""}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}