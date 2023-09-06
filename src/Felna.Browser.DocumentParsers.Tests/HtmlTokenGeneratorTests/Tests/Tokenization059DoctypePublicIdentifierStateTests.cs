namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Tokenization059DoctypePublicIdentifierStateTests
{
    [TestMethod]
    // Quotation mark
    [DataRow("<!doctype html public \"\">", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""""}]")]
    // NULL
    [DataRow("<!doctype html public \"p\u0000id\">", "[{\"type\":\"doctype\",\"name\":\"html\",\"publicidentifier\":\"p\ufffdid\"}]")]
    // Greater than sign
    [DataRow("<!doctype html public \"pid>", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""forcequirks"":true}]")]
    // EOF
    [DataRow("<!doctype html public \"", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":"""",""forcequirks"":true}]")]
    [DataRow("<!doctype html public \"pid", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""forcequirks"":true}]")]
    // Anything else
    [DataRow("<!doctype html public \"pid\">", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid""}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}