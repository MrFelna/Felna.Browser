namespace Felna.Browser.Parsing.TokenGeneration.Tests.Tests;

[TestClass]
public class Tokenization055DoctypeNameStateTests
{
    [TestMethod]
    // Tab
    [DataRow("<!DOCTYPE html\t>", @"[{""type"":""doctype"",""name"":""html""}]")]
    // Line feed
    [DataRow("<!DOCTYPE html\n>", @"[{""type"":""doctype"",""name"":""html""}]")]
    // Form feed
    [DataRow("<!DOCTYPE html\f>", @"[{""type"":""doctype"",""name"":""html""}]")]
    // Space
    [DataRow("<!DOCTYPE html >", @"[{""type"":""doctype"",""name"":""html""}]")]
    // Greater than sign
    [DataRow("<!DOCTYPE html>", @"[{""type"":""doctype"",""name"":""html""}]")]
    // ASCII upper alpha
    [DataRow("<!DOCTYPE HTML>", @"[{""type"":""doctype"",""name"":""html""}]")]
    // NULL
    [DataRow("<!DOCTYPE ht\u0000ml>", "[{\"type\":\"doctype\",\"name\":\"ht\ufffdml\"}]")]
    // EOF
    [DataRow("<!DOCTYPE h", @"[{""type"":""doctype"",""name"":""h"",""forcequirks"":true}]")]
    [DataRow("<!DOCTYPE ht", @"[{""type"":""doctype"",""name"":""ht"",""forcequirks"":true}]")]
    [DataRow("<!DOCTYPE htm", @"[{""type"":""doctype"",""name"":""htm"",""forcequirks"":true}]")]
    [DataRow("<!DOCTYPE html", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    [DataRow("<!DOCTYPE html ", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    // Anything else
    [DataRow("<!DOCTYPE html5>", @"[{""type"":""doctype"",""name"":""html5""}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}