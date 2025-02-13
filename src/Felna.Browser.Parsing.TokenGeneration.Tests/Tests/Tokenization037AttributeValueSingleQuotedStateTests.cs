namespace Felna.Browser.Parsing.TokenGeneration.Tests.Tests;

[TestClass]
public class Tokenization037AttributeValueSingleQuotedStateTests
{
    [TestMethod]
    // Quotation mark
    [DataRow("<p a=''>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""""}}]")]
    // Ampersand
    [DataRow("<p a='&apos'>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""'""}}]")]
    // NULL
    [DataRow("<p a='\u0000'>", "[{\"type\":\"tag\",\"name\":\"p\",\"attributes\":{\"a\":\"\ufffd\"}}]")]
    // EOF
    [DataRow("<p a='", "[]")]
    // Anything else
    [DataRow("<p a='b'>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""b""}}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}