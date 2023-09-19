namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Tokenization041BogusCommentStateTests
{
    [TestMethod]
    // Greater-than sign
    [DataRow("<!>", @"[{""type"":""comment"",""data"":""""}]")]
    [DataRow("<!Test>", @"[{""type"":""comment"",""data"":""Test""}]")]
    // EOF
    [DataRow("<!", @"[{""type"":""comment"",""data"":""""}]")]
    [DataRow("<!Test", @"[{""type"":""comment"",""data"":""Test""}]")]
    // NULL
    [DataRow("<!\u0000>", "[{\"type\":\"comment\",\"data\":\"\ufffd\"}]")]
    [DataRow("<!Te\u0000st>", "[{\"type\":\"comment\",\"data\":\"Te\ufffdst\"}]")]
    // Anything else
    [DataRow("<! test>", @"[{""type"":""comment"",""data"":"" test""}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}