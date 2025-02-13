namespace Felna.Browser.Parsing.TokenGeneration.Tests.Tests;

[TestClass]
public class Tokenization040SelfClosingStartTagStateTests
{
    [TestMethod]
    // Greater-than sign
    [DataRow("<p/>", @"[{""type"":""tag"",""name"":""p"",""selfclosing"":true}]")]
    [DataRow("<p a/>", @"[{""type"":""tag"",""name"":""p"",""selfclosing"":true,""attributes"":{""a"":""""}}]")]
    [DataRow("<p a='b'/>", @"[{""type"":""tag"",""name"":""p"",""selfclosing"":true,""attributes"":{""a"":""b""}}]")]
    // EOF
    [DataRow("<p/", "[]")]
    [DataRow("<p a/", "[]")]
    [DataRow("<p a='b'/", "[]")]
    // Anything else
    [DataRow("<p//>", @"[{""type"":""tag"",""name"":""p"",""selfclosing"":true}]")]
    [DataRow("<p a//>", @"[{""type"":""tag"",""name"":""p"",""selfclosing"":true,""attributes"":{""a"":""""}}]")]
    [DataRow("<p a='b'//>", @"[{""type"":""tag"",""name"":""p"",""selfclosing"":true,""attributes"":{""a"":""b""}}]")]
    [DataRow("<p/c>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""c"":""""}}]")]
    [DataRow("<p a/c>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":"""",""c"":""""}}]")]
    [DataRow("<p a='b'/c>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""b"",""c"":""""}}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}