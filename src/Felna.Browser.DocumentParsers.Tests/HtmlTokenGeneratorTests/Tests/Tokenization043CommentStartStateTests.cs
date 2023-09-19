namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Tokenization043CommentStartStateTests
{
    [TestMethod]
    // Hyphen-minus
    [DataRow("<!---", @"[{""type"":""comment"",""data"":""""}]")]
    [DataRow("<!--->", @"[{""type"":""comment"",""data"":""""}]")]
    // Greater-than sign
    [DataRow("<!-->", @"[{""type"":""comment"",""data"":""""}]")]
    // Anything else
    [DataRow("<!--", @"[{""type"":""comment"",""data"":""""}]")]
    [DataRow("<!--test-->", @"[{""type"":""comment"",""data"":""test""}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}