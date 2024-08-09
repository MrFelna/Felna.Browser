namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Tokenization035BeforeAttributeValueStateTests
{
    [TestMethod]
    // Tab
    [DataRow("<p a=\tb>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""b""}}]")]
    // Line feed
    [DataRow("<p a=\nb>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""b""}}]")]
    // Form feed
    [DataRow("<p a=\fb>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""b""}}]")]
    // Space
    [DataRow("<p a= b>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""b""}}]")]
    // Quotation mark
    [DataRow("<p a=\"b\">", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""b""}}]")]
    // Apostrophe
    [DataRow("<p a='b'>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""b""}}]")]
    // Greater-than sign
    [DataRow("<p a=>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""""}}]")]
    // EOF
    [DataRow("<p a=", "[]")]
    // Anything else
    [DataRow("<p a=b>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""b""}}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}