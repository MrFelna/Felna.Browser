namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Tokenization001DataStateTests
{
    [TestMethod]
    // TODO: Ampersand
    // Less-than sign
    [DataRow("<!DOCTYPE html>", @"[{""type"":""doctype"",""name"":""html""}]")]
    // TODO: NULL
    // EOF
    [DataRow("", "[]")]
    // TODO: Anything else
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}