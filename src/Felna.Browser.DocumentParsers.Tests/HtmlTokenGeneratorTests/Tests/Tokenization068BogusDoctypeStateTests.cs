namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Tokenization068BogusDoctypeStateTests
{
    [TestMethod]
    // Greater than sign
    [DataRow("<!doctype html test>", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    [DataRow("<!doctype html test >", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    [DataRow("<!doctype html publictest>", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public test>", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public id>", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public 'pid'test>", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public 'pid' test>", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public 'pid' 'sid'test>", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid""}]")]
    [DataRow("<!doctype html public 'pid' 'sid' test>", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid""}]")]
    [DataRow("<!doctype html system id>", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    [DataRow("<!doctype html system 'sid'test>", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid""}]")]
    [DataRow("<!doctype html system 'sid' test>", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid""}]")]
    // NULL
    [DataRow("<!doctype html te\u0000st>", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    [DataRow("<!doctype html te\u0000st >", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    [DataRow("<!doctype html test \u0000", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    [DataRow("<!doctype html publicte\u0000st>", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public te\u0000st>", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public i\u0000d", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public 'pid'te\u0000st>", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public 'pid' te\u0000st>", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public 'pid' 'sid'te\u0000st>", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid""}]")]
    [DataRow("<!doctype html public 'pid' 'sid' te\u0000st>", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid""}]")]
    [DataRow("<!doctype html system i\u0000d", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    [DataRow("<!doctype html system 'sid'te\u0000st>", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid""}]")]
    [DataRow("<!doctype html system 'sid' te\u0000st>", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid""}]")]
    // EOF
    [DataRow("<!doctype html test", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    [DataRow("<!doctype html test ", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    [DataRow("<!doctype html publictest", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public test", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public id", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public 'pid'test", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public 'pid' test", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public 'pid' 'sid'test", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid""}]")]
    [DataRow("<!doctype html public 'pid' 'sid' test", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid""}]")]
    [DataRow("<!doctype html system id", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    [DataRow("<!doctype html system 'sid'test", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid""}]")]
    [DataRow("<!doctype html system 'sid' test", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid""}]")]
    // Anything else
    [DataRow("<!doctype html test>", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    [DataRow("<!doctype html test >", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    [DataRow("<!doctype html publictest>", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public test>", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public id>", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public 'pid'test>", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public 'pid' test>", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""forcequirks"":true}]")]
    [DataRow("<!doctype html public 'pid' 'sid'test>", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid""}]")]
    [DataRow("<!doctype html public 'pid' 'sid' test>", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid"",""systemidentifier"":""sid""}]")]
    [DataRow("<!doctype html system id>", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    [DataRow("<!doctype html system 'sid'test>", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid""}]")]
    [DataRow("<!doctype html system 'sid' test>", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid""}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}