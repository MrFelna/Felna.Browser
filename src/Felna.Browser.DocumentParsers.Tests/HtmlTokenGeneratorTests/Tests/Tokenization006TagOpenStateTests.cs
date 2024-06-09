namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Tokenization006TagOpenStateTests
{
    [TestMethod]
    // Exclamation
    [DataRow("<!--test-->", @"[{""type"":""comment"",""data"":""test""}]")]
    // Solidus
    [DataRow("</p>", @"[{""type"":""tag"",""name"":""p"",""selfclosing"":false,""isendtag"":true}]")]
    // Alpha
    [DataRow("<p>", @"[{""type"":""tag"",""name"":""p"",""selfclosing"":false,""isendtag"":false}]")]
    // Question mark
    [DataRow("<?", @"[{""type"":""comment"",""data"":""?""}]")]
    [DataRow("<?test>", @"[{""type"":""comment"",""data"":""?test""}]")]
    [DataRow("<?\u0000", @"[{""type"":""comment"",""data"":""?\ufffd""}]")]
    // EOF
    [DataRow("<", @"[{""type"":""character"",""data"":""<""}]")]
    // Anything else
    [DataRow("<1", @"[{""type"":""character"",""data"":""<""},{""type"":""character"",""data"":""1""}]")]
    [DataRow("<>", @"[{""type"":""character"",""data"":""<""},{""type"":""character"",""data"":"">""}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}