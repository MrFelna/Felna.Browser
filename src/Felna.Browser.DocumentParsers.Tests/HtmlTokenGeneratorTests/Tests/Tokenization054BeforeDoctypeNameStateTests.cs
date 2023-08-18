namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Tokenization054BeforeDoctypeNameStateTests
{
    [TestMethod]
    [DataRow("<!DOCTYPE ", @"[{""type"":""doctype"",""forcequirks"":true}]")]
    [DataRow("<!DOCTYPE  html>", @"[{""type"":""doctype"",""name"":""html""}]")]
    [DataRow("<!DOCTYPE \thtml>", @"[{""type"":""doctype"",""name"":""html""}]")]
    [DataRow("<!DOCTYPE \nhtml>", @"[{""type"":""doctype"",""name"":""html""}]")]
    [DataRow("<!DOCTYPE \fhtml>", @"[{""type"":""doctype"",""name"":""html""}]")]
    [DataRow("<!DOCTYPE \u0000html>", "[{\"type\":\"doctype\",\"name\":\"\ufffdhtml\"}]")]
    [DataRow("<!DOCTYPE >", @"[{""type"":""doctype"",""forcequirks"":true}]")]
    [DataRow("<!DOCTYPE HTML>", @"[{""type"":""doctype"",""name"":""html""}]")]
    [DataRow("<!DOCTYPE html>", @"[{""type"":""doctype"",""name"":""html""}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}