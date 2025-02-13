namespace Felna.Browser.Parsing.TokenGeneration.Tests.Tests;

[TestClass]
public class Tokenization060DoctypePublicIdentifierSingleQuotedStateTests
{
    [TestMethod]
    // Apostrophe
    [DataRow("<!doctype html public ''>", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""""}]")]
    // NULL
    [DataRow("<!doctype html public 'p\u0000id'>", "[{\"type\":\"doctype\",\"name\":\"html\",\"publicidentifier\":\"p\ufffdid\"}]")]
    // Greater than sign
    [DataRow("<!doctype html public 'pid>", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""forcequirks"":true}]")]
    // EOF
    [DataRow("<!doctype html public '", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":"""",""forcequirks"":true}]")]
    [DataRow("<!doctype html public 'pid", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""forcequirks"":true}]")]
    // Anything else
    [DataRow("<!doctype html public 'pid'>", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid""}]")]
    [DataRow("<!doctype html public 'p\"id'>", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""p\""id""}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}