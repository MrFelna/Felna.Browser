namespace Felna.Browser.Parsing.TokenGeneration.Tests.Tests;

[TestClass]
public class Tokenization065DoctypeSystemIdentifierDoubleQuotedStateTests
{
    [TestMethod]
    // Quotation Mark
    [DataRow("<!doctype html system \"\">", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""""}]")]
    // NULL
    [DataRow("<!doctype html system \"s\u0000id\">", "[{\"type\":\"doctype\",\"name\":\"html\",\"systemidentifier\":\"s\ufffdid\"}]")]
    // Greater than sign
    [DataRow("<!doctype html system \"sid>", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid"",""forcequirks"":true}]")]
    // EOF
    [DataRow("<!doctype html system \"sid", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid"",""forcequirks"":true}]")]
    // Anything else
    [DataRow("<!doctype html system \"sid\">", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid""}]")]
    [DataRow("<!doctype html system \"s'id\">", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""s'id""}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}