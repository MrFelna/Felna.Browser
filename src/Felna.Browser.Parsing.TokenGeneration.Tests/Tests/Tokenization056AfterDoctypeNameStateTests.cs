namespace Felna.Browser.Parsing.TokenGeneration.Tests.Tests;

[TestClass]
public class Tokenization056AfterDoctypeNameStateTests
{
    [TestMethod]
    // Tab
    [DataRow("<!DOCTYPE html \t>", @"[{""type"":""doctype"",""name"":""html""}]")]
    // Line feed
    [DataRow("<!DOCTYPE html \n>", @"[{""type"":""doctype"",""name"":""html""}]")]
    // Form feed
    [DataRow("<!DOCTYPE html \f>", @"[{""type"":""doctype"",""name"":""html""}]")]
    // Space
    [DataRow("<!DOCTYPE html  >", @"[{""type"":""doctype"",""name"":""html""}]")]
    // Greater than sign
    [DataRow("<!DOCTYPE html >", @"[{""type"":""doctype"",""name"":""html""}]")]
    // EOF
    [DataRow("<!DOCTYPE html ", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    // Anything else - public
    [DataRow("<!doctype html public 'pid'>", @"[{""type"":""doctype"",""name"":""html"",""publicidentifier"":""pid""}]")]
    // Anything else - system
    [DataRow("<!doctype html system 'sid'>", @"[{""type"":""doctype"",""name"":""html"",""systemidentifier"":""sid""}]")]
    // Anything else
    [DataRow("<!doctype html test", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}