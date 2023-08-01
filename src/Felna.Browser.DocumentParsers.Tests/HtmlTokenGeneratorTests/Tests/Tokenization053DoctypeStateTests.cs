namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Tokenization053DoctypeStateTests
{
    [TestMethod]
    [DataRow("<!DOCTYPE html>", @"[{""type"":""doctype"",""name"":""html""}]")]
    [DataRow("<!DOCTYPE\thtml>", @"[{""type"":""doctype"",""name"":""html""}]")]
    [DataRow("<!DOCTYPE\nhtml>", @"[{""type"":""doctype"",""name"":""html""}]")]
    [DataRow("<!DOCTYPE\fhtml>", @"[{""type"":""doctype"",""name"":""html""}]")]
    [DataRow("<!DOCTYPE \thtml>", @"[{""type"":""doctype"",""name"":""html""}]")]
    [DataRow("<!DOCTYPE\t\nhtml>", @"[{""type"":""doctype"",""name"":""html""}]")]
    [DataRow("<!DOCTYPE\n\fhtml>", @"[{""type"":""doctype"",""name"":""html""}]")]
    [DataRow("<!DOCTYPE\f html>", @"[{""type"":""doctype"",""name"":""html""}]")]
    [DataRow("<!DOCTYPE>", @"[{""type"":""doctype"",""forcequirks"":true}]")]
    [DataRow("<!DOCTYPE", @"[{""type"":""doctype"",""forcequirks"":true}]")]
    [DataRow("<!DOCTYPEhtml>", @"[{""type"":""doctype"",""name"":""html""}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}