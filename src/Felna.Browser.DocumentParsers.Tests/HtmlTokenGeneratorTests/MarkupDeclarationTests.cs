using Felna.Browser.DocumentParsers.TextReferences;

namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests;

[TestClass]
public class MarkupDeclarationTests
{
    [TestMethod]
    [DataRow("<!doctype html test", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    [DataRow("<!doctype html test>", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    [DataRow("<!doctype html test >", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    [DataRow("<!doctype html test \u0000", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public id", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public>", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    [DataRow("<!doctype html system id", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    [DataRow("<!doctype html system", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    [DataRow("<!doctype html system>", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public \"pid\"", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public \"pid\" ", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public \"pid\">", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid""}]")]
    [DataRow("<!doctype html public \"pid\" >", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid""}]")]
    [DataRow("<!doctype html public 'pid'", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public 'pid' ", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public 'pid'>", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid""}]")]
    [DataRow("<!doctype html public 'pid' >", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid""}]")]
    [DataRow("<!doctype html public \"pid\" \"sid\"", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public \"pid\" \"sid\" ", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public \"pid\" \"sid\">", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid""}]")]
    [DataRow("<!doctype html public \"pid\" \"sid\" >", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid""}]")]
    [DataRow("<!doctype html public 'pid' 'sid'", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public 'pid' 'sid' ", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public 'pid' 'sid'>", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid""}]")]
    [DataRow("<!doctype html public 'pid' 'sid' >", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid""}]")]
    [DataRow("<!doctype html public \"pid\" 'sid'", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public \"pid\" 'sid' ", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public \"pid\" 'sid'>", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid""}]")]
    [DataRow("<!doctype html public \"pid\" 'sid' >", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid""}]")]
    [DataRow("<!doctype html public 'pid' \"sid\"", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public 'pid' \"sid\" ", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public 'pid' \"sid\">", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid""}]")]
    [DataRow("<!doctype html public 'pid' \"sid\" >", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid""}]")]
    [DataRow("<!doctype html system \"sid\"", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid"",""forcequirks"":true}]")]
    [DataRow("<!doctype html system \"sid\" ", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid"",""forcequirks"":true}]")]
    [DataRow("<!doctype html system \"sid\">", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid""}]")]
    [DataRow("<!doctype html system \"sid\" >", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid""}]")]
    [DataRow("<!doctype html system 'sid'", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid"",""forcequirks"":true}]")]
    [DataRow("<!doctype html system 'sid' ", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid"",""forcequirks"":true}]")]
    [DataRow("<!doctype html system 'sid'>", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid""}]")]
    [DataRow("<!doctype html system 'sid' >", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid""}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}