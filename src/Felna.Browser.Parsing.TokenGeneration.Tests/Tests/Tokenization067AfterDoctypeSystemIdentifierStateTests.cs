﻿namespace Felna.Browser.Parsing.TokenGeneration.Tests.Tests;

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
    [DataRow("<!doctype html public \"pid\" \"sid\" >", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid""}]")]
    [DataRow("<!doctype html public 'pid' 'sid' >", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid""}]")]
    // Greater than sign
    [DataRow("<!doctype html public \"pid\" 'sid'>", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid""}]")]
    [DataRow("<!doctype html public \"pid\" 'sid' >", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid""}]")]
    [DataRow("<!doctype html public 'pid' \"sid\">", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid""}]")]
    [DataRow("<!doctype html public 'pid' \"sid\" >", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid""}]")]
    [DataRow("<!doctype html system \"sid\">", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid""}]")]
    [DataRow("<!doctype html system \"sid\" >", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid""}]")]
    [DataRow("<!doctype html system 'sid' >", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid""}]")]
    [DataRow("<!doctype html public \"pid\" \"sid\">", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid""}]")]
    [DataRow("<!doctype html public 'pid' 'sid'>", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid""}]")]
    // EOF
    [DataRow("<!doctype html system 'sid'", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public \"pid\" \"sid\"", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid"",""forcequirks"":true}]")]
    [DataRow("<!doctype html system 'sid' ", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public 'pid' 'sid'", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public 'pid' 'sid' ", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public \"pid\" 'sid'", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public \"pid\" 'sid' ", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public 'pid' \"sid\"", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public 'pid' \"sid\" ", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid"",""forcequirks"":true}]")]
    [DataRow("<!doctype html system \"sid\"", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid"",""forcequirks"":true}]")]
    [DataRow("<!doctype html system \"sid\" ", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid"",""forcequirks"":true}]")]
    // Anything else
    [DataRow("<!doctype html system 'sid'xx>", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid""}]")]
    [DataRow("<!doctype html system 'sid' xx>", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid""}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}