namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Tokenization049CommentLessThanSignBangDashDashStateTests 
{
    [TestMethod]
    // Greater-than sign
    [DataRow("<!--te<!-->", @"[{""type"":""comment"",""data"":""te<!""}]")]
    [DataRow("<!--te<<!-->", @"[{""type"":""comment"",""data"":""te<<!""}]")]
    // EOF
    [DataRow("<!--te<!--", @"[{""type"":""comment"",""data"":""te<!""}]")]
    [DataRow("<!--te<<!--", @"[{""type"":""comment"",""data"":""te<<!""}]")]
    // Anything else
    [DataRow("<!--te<!--\u0000st-->", "[{\"type\":\"comment\",\"data\":\"te<!--\ufffdst\"}]")]
    [DataRow("<!--te<!--", @"[{""type"":""comment"",""data"":""te<!""}]")]
    [DataRow("<!--te<!--st-->", @"[{""type"":""comment"",""data"":""te<!--st""}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}