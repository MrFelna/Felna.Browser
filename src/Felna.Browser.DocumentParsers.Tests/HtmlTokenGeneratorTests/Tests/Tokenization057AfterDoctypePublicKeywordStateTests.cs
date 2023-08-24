namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Tokenization057AfterDoctypePublicKeywordStateTests
{
    [TestMethod]
    // Tab
    [DataRow("<!doctype html public\t'pid'>", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid""}]")]
    // Line feed
    [DataRow("<!doctype html public\n'pid'>", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid""}]")]
    // Form feed
    [DataRow("<!doctype html public\f'pid'>", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid""}]")]
    // Space
    [DataRow("<!doctype html public 'pid'>", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid""}]")]
    // Quotation mark
    [DataRow("<!doctype html public\"pid\">", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid""}]")]
    // Apostrophe
    [DataRow("<!doctype html public'pid'>", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid""}]")]
    // Greater than sign
    [DataRow("<!doctype html public>", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    // EOF
    [DataRow("<!doctype html public", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    // Anything else
    [DataRow("<!doctype html publicpid'>", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}