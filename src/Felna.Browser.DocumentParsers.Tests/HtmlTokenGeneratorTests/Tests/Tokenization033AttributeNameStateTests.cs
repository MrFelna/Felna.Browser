namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Tokenization033AttributeNameStateTests
{
    [TestMethod]
    // Tab
    [DataRow("<p a\t>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""""}}]")]
    [DataRow("<p a b\t>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":"""",""b"":""""}}]")]
    // Line feed
    [DataRow("<p a\n>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""""}}]")]
    [DataRow("<p a b\n>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":"""",""b"":""""}}]")]
    // Form feed
    [DataRow("<p a\f>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""""}}]")]
    [DataRow("<p a b\f>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":"""",""b"":""""}}]")]
    // Space
    [DataRow("<p a >", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""""}}]")]
    [DataRow("<p a b >", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":"""",""b"":""""}}]")]
    // Solidus
    [DataRow("<p a/>", @"[{""type"":""tag"",""name"":""p"",""selfclosing"":true,""attributes"":{""a"":""""}}]")]
    [DataRow("<p a b/>", @"[{""type"":""tag"",""name"":""p"",""selfclosing"":true,""attributes"":{""a"":"""",""b"":""""}}]")]
    // Greater-than sign
    [DataRow("<p a>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""""}}]")]
    [DataRow("<p a b>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":"""",""b"":""""}}]")]
    // EOF
    [DataRow("<p a", "[]")]
    [DataRow("<p a b", "[]")]
    // Equals
    [DataRow("<p a=>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""""}}]")]
    [DataRow("<p a b=>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":"""",""b"":""""}}]")]
    // ASCII upper alpha
    [DataRow("<p aD>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""ad"":""""}}]")]
    [DataRow("<p a bD>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":"""",""bd"":""""}}]")]
    // NULL
    [DataRow("<p a\u0000>", "[{\"type\":\"tag\",\"name\":\"p\",\"attributes\":{\"a\ufffd\":\"\"}}]")]
    [DataRow("<p a b\u0000>", "[{\"type\":\"tag\",\"name\":\"p\",\"attributes\":{\"a\":\"\",\"b\ufffd\":\"\"}}]")]
    // Quotation mark
    [DataRow("<p a\">", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a\"""":""""}}]")]
    [DataRow("<p a b\">", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":"""",""b\"""":""""}}]")]
    // Apostrophe
    [DataRow("<p a'>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a'"":""""}}]")]
    [DataRow("<p a b'>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":"""",""b'"":""""}}]")]
    // Less-than sign
    [DataRow("<p a<>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a<"":""""}}]")]
    [DataRow("<p a b<>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":"""",""b<"":""""}}]")]
    // Anything else
    [DataRow("<p a1>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a1"":""""}}]")]
    [DataRow("<p a b1>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":"""",""b1"":""""}}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}