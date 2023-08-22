namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Tokenization054BeforeDoctypeNameStateTests
{
    [TestMethod]
    // Tab
    [DataRow("<!DOCTYPE \thtml>", @"[{""type"":""doctype"",""name"":""html""}]")]
    // Line feed
    [DataRow("<!DOCTYPE \nhtml>", @"[{""type"":""doctype"",""name"":""html""}]")]
    // Form feed
    [DataRow("<!DOCTYPE \fhtml>", @"[{""type"":""doctype"",""name"":""html""}]")]
    // Space
    [DataRow("<!DOCTYPE  html>", @"[{""type"":""doctype"",""name"":""html""}]")]
    // ASCII upper alpha
    [DataRow("<!DOCTYPE HTML>", @"[{""type"":""doctype"",""name"":""html""}]")]
    // NULL
    [DataRow("<!DOCTYPE \u0000html>", "[{\"type\":\"doctype\",\"name\":\"\ufffdhtml\"}]")]
    // Greater than sign
    [DataRow("<!DOCTYPE >", @"[{""type"":""doctype"",""forcequirks"":true}]")]
    // EOF
    [DataRow("<!DOCTYPE ", @"[{""type"":""doctype"",""forcequirks"":true}]")]
    // Anything else
    [DataRow("<!DOCTYPE html>", @"[{""type"":""doctype"",""name"":""html""}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}