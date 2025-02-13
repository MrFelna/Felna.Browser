namespace Felna.Browser.Parsing.TokenGeneration.Tests.Tests;

[TestClass]
public class Tokenization008TagNameStateTests
{
    [TestMethod]
    // Tab
    [DataRow("<p\t>", @"[{""type"":""tag"",""name"":""p""}]")]
    [DataRow("</p\t>", @"[{""type"":""tag"",""name"":""p"",""isendtag"":true}]")]
    // Line feed
    [DataRow("<p\n>", @"[{""type"":""tag"",""name"":""p""}]")]
    [DataRow("</p\n>", @"[{""type"":""tag"",""name"":""p"",""isendtag"":true}]")]
    // Form feed
    [DataRow("<p\f>", @"[{""type"":""tag"",""name"":""p""}]")]
    [DataRow("</p\f>", @"[{""type"":""tag"",""name"":""p"",""isendtag"":true}]")]
    // Space
    [DataRow("<p >", @"[{""type"":""tag"",""name"":""p""}]")]
    [DataRow("</p >", @"[{""type"":""tag"",""name"":""p"",""isendtag"":true}]")]
    // Solidus
    [DataRow("<p/>", @"[{""type"":""tag"",""name"":""p"",""selfclosing"":true}]")]
    [DataRow("</p/>", @"[{""type"":""tag"",""name"":""p"",""selfclosing"":true,""isendtag"":true}]")]
    // Greater-than sign
    [DataRow("<p>", @"[{""type"":""tag"",""name"":""p""}]")]
    [DataRow("</p>", @"[{""type"":""tag"",""name"":""p"",""isendtag"":true}]")]
    // ASCII upper alpha
    [DataRow("<P>", @"[{""type"":""tag"",""name"":""p""}]")]
    [DataRow("</P>", @"[{""type"":""tag"",""name"":""p"",""isendtag"":true}]")]
    // NULL
    [DataRow("<p\u0000>", "[{\"type\":\"tag\",\"name\":\"p\ufffd\"}]")]
    [DataRow("</p\u0000>", "[{\"type\":\"tag\",\"name\":\"p\ufffd\",\"isendtag\":true}]")]
    // EOF
    [DataRow("<p", @"[]")]
    [DataRow("</p", @"[]")]
    // Anything else
    [DataRow("<p1>", @"[{""type"":""tag"",""name"":""p1""}]")]
    [DataRow("</p1>", @"[{""type"":""tag"",""name"":""p1"",""isendtag"":true}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}