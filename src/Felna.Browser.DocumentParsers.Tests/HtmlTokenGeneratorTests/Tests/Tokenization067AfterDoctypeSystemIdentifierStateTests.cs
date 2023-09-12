namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Tokenization067AfterDoctypeSystemIdentifierStateTests
{
    [TestMethod]
    // Tab
    [DataRow("<!doctype html system 'sid'\t>", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid""}]")]
    // Line feed
    [DataRow("<!doctype html system 'sid'\n>", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid""}]")]
    // Form feed
    [DataRow("<!doctype html system 'sid'\f>", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid""}]")]
    // Space
    [DataRow("<!doctype html system 'sid' >", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid""}]")]
    // Greater than sign
    [DataRow("<!doctype html system 'sid'>", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid""}]")]
    // EOF
    [DataRow("<!doctype html system 'sid'", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid"",""forcequirks"":true}]")]
    // Anything else
    [DataRow("<!doctype html system 'sid'xx>", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid""}]")]
    [DataRow("<!doctype html system 'sid' xx>", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid""}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}