namespace Felna.Browser.Parsing.TokenGeneration.Tests.Tests;

[TestClass]
public class Tokenization038AttributeValueUnquotedStateTests
{
    [TestMethod]
    // Tab
    [DataRow("<p a=b\tc=d>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""b"",""c"":""d""}}]")]
    // Line feed
    [DataRow("<p a=b\nc=d>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""b"",""c"":""d""}}]")]
    // Form feed
    [DataRow("<p a=b\fc=d>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""b"",""c"":""d""}}]")]
    // Space
    [DataRow("<p a=b c=d>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""b"",""c"":""d""}}]")]
    // Ampersand
    [DataRow("<p a=&apos; >", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""'""}}]")]
    // Greater-than sign
    [DataRow("<p a=b>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""b""}}]")]
    // NULL
    [DataRow("<p a=\u0000>", "[{\"type\":\"tag\",\"name\":\"p\",\"attributes\":{\"a\":\"\ufffd\"}}]")]
    // Quotation mark
    [DataRow("<p a=b\">", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""b\""""}}]")]
    // Apostrophe
    [DataRow("<p a=b'>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""b'""}}]")]
    // Less-than sign
    [DataRow("<p a=<>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""<""}}]")]
    // Equals
    [DataRow("<p a==>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""=""}}]")]
    // Space
    [DataRow("<p a=`>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""`""}}]")]
    // EOF
    [DataRow("<p a=b", "[]")]
    // Anything else
    [DataRow("<p a=b1>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""b1""}}]")]
    [DataRow("<p a=b/>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""b/""}}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}