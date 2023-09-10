namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Tokenization063AfterDoctypeSystemKeywordStateTests
{
    [TestMethod]
    // Tab
    [DataRow("<!doctype html system\t'sid'>", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid""}]")]
    // Line feed
    [DataRow("<!doctype html system\n'sid'>", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid""}]")]
    // Form feed
    [DataRow("<!doctype html system\f'sid'>", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid""}]")]
    // Space
    [DataRow("<!doctype html system 'sid'>", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid""}]")]
    // Quotation mark
    [DataRow("<!doctype html system\"sid\">", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid""}]")]
    // Apostrophe
    [DataRow("<!doctype html system'sid'>", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid""}]")]
    // Greater than sign
    [DataRow("<!doctype html system>", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    // EOF
    [DataRow("<!doctype html system", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    // Anything else
    [DataRow("<!doctype html systemsid'>", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}