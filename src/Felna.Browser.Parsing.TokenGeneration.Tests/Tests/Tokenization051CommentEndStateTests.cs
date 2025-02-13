namespace Felna.Browser.Parsing.TokenGeneration.Tests.Tests;

[TestClass]
public class Tokenization051CommentEndStateTests 
{
    [TestMethod]
    // Greater-than sign
    [DataRow("<!---->", @"[{""type"":""comment"",""data"":""""}]")]
    [DataRow("<!--test-->", @"[{""type"":""comment"",""data"":""test""}]")]
    [DataRow("<!--test<!-->", @"[{""type"":""comment"",""data"":""test<!""}]")]
    // Exclamation mark
    [DataRow("<!----!2", @"[{""type"":""comment"",""data"":""--!2""}]")]
    [DataRow("<!--test--!2", @"[{""type"":""comment"",""data"":""test--!2""}]")]
    [DataRow("<!--test<!--!2", @"[{""type"":""comment"",""data"":""test<!--!2""}]")]
    // Hyphen-minus
    [DataRow("<!-----", @"[{""type"":""comment"",""data"":""-""}]")]
    [DataRow("<!--test---", @"[{""type"":""comment"",""data"":""test-""}]")]
    [DataRow("<!--test<!---", @"[{""type"":""comment"",""data"":""test<!-""}]")]
    // EOF
    [DataRow("<!----", @"[{""type"":""comment"",""data"":""""}]")]
    [DataRow("<!--test--", @"[{""type"":""comment"",""data"":""test""}]")]
    [DataRow("<!--test<!--", @"[{""type"":""comment"",""data"":""test<!""}]")]
    // Anything else
    [DataRow("<!----2-->", @"[{""type"":""comment"",""data"":""--2""}]")]
    [DataRow("<!--test--2-->", @"[{""type"":""comment"",""data"":""test--2""}]")]
    [DataRow("<!--test<!--2-->", @"[{""type"":""comment"",""data"":""test<!--2""}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}