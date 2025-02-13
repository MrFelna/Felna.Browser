namespace Felna.Browser.Parsing.TokenGeneration.Tests.Tests;

[TestClass]
public class Tokenization044CommentStartDashStateTests 
{
    [TestMethod]
    // Hyphen-minus
    [DataRow("<!----", @"[{""type"":""comment"",""data"":""""}]")]
    [DataRow("<!-----", @"[{""type"":""comment"",""data"":""-""}]")]
    [DataRow("<!---->", @"[{""type"":""comment"",""data"":""""}]")]
    [DataRow("<!----->", @"[{""type"":""comment"",""data"":""-""}]")]
    [DataRow("<!----!", @"[{""type"":""comment"",""data"":""""}]")]
    [DataRow("<!----!>", @"[{""type"":""comment"",""data"":""""}]")]
    [DataRow("<!----!-", @"[{""type"":""comment"",""data"":""--!""}]")]
    [DataRow("<!----!-->", @"[{""type"":""comment"",""data"":""--!""}]")]
    [DataRow("<!----!-test-->", @"[{""type"":""comment"",""data"":""--!-test""}]")]
    [DataRow("<!----!test-->", @"[{""type"":""comment"",""data"":""--!test""}]")]
    [DataRow("<!----test-->", @"[{""type"":""comment"",""data"":""--test""}]")]
    // Greater-than sign
    [DataRow("<!--->", @"[{""type"":""comment"",""data"":""""}]")]
    // EOF
    [DataRow("<!---", @"[{""type"":""comment"",""data"":""""}]")]
    // Anything else
    [DataRow("<!---test-->", @"[{""type"":""comment"",""data"":""-test""}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}