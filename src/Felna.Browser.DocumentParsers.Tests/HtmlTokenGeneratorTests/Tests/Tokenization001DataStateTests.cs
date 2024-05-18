namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Tokenization001DataStateTests
{
    [TestMethod]
    // TODO: Ampersand
    // Less-than sign
    [DataRow("<!DOCTYPE html>", @"[{""type"":""doctype"",""name"":""html""}]")]
    // NULL
    [DataRow("\u0000", @"[{""type"":""character"",""data"":""\u0000""}]")]
    // EOF
    [DataRow("", "[]")]
    // Anything else
    [DataRow("a", @"[{""type"":""character"",""data"":""a""}]")]
    [DataRow("\u03B2", @"[{""type"":""character"",""data"":""\u03B2""}]")] // greek letter lower beta
    [DataRow("\ud83d\ude0a", @"[{""type"":""character"",""data"":""\ud83d\ude0a""}]")] // smiling face with smiling eyes
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}