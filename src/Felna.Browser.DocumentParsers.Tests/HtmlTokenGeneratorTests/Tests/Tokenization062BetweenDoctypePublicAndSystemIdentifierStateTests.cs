namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Tokenization062BetweenDoctypePublicAndSystemIdentifierStateTests
{
    [TestMethod]
    // Tab
    [DataRow("<!doctype html public 'pid' \t>", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid""}]")]
    // Line feed
    [DataRow("<!doctype html public 'pid' \n>", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid""}]")]
    // Form feed
    [DataRow("<!doctype html public 'pid' \f>", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid""}]")]
    // Space
    [DataRow("<!doctype html public 'pid'  >", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid""}]")]
    // Greater than sign
    [DataRow("<!doctype html public 'pid' >", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid""}]")]
    // Quotation mark
    [DataRow("<!doctype html public 'pid' \"sid\">", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid""}]")]
    // Apostrophe
    [DataRow("<!doctype html public 'pid' 'sid'>", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid""}]")]
    // EOF
    [DataRow("<!doctype html public 'pid' ", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""forcequirks"":true}]")]
    // Anything else
    [DataRow("<!doctype html public 'pid' sid>", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""forcequirks"":true}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}