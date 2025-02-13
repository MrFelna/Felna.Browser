namespace Felna.Browser.Parsing.TokenGeneration.Tests.Tests;

[TestClass]
public class Tokenization053DoctypeStateTests
{
    [TestMethod]
    // Tab
    [DataRow("<!DOCTYPE\thtml>", @"[{""type"":""doctype"",""name"":""html""}]")]
    [DataRow("<!DOCTYPE\t\nhtml>", @"[{""type"":""doctype"",""name"":""html""}]")]
    // Line feed
    [DataRow("<!DOCTYPE\nhtml>", @"[{""type"":""doctype"",""name"":""html""}]")]
    [DataRow("<!DOCTYPE\n\fhtml>", @"[{""type"":""doctype"",""name"":""html""}]")]
    // Form feed
    [DataRow("<!DOCTYPE\fhtml>", @"[{""type"":""doctype"",""name"":""html""}]")]
    [DataRow("<!DOCTYPE\f html>", @"[{""type"":""doctype"",""name"":""html""}]")]
    // Space
    [DataRow("<!DOCTYPE html>", @"[{""type"":""doctype"",""name"":""html""}]")]
    [DataRow("<!DOCTYPE \thtml>", @"[{""type"":""doctype"",""name"":""html""}]")]
    // Greater than sign
    [DataRow("<!DOCTYPE>", @"[{""type"":""doctype"",""forcequirks"":true}]")]
    // EOF
    [DataRow("<!DOCTYPE", @"[{""type"":""doctype"",""forcequirks"":true}]")]
    // Anything else
    [DataRow("<!DOCTYPEHTML>", @"[{""type"":""doctype"",""name"":""html""}]")]
    [DataRow("<!DOCTYPEhtml>", @"[{""type"":""doctype"",""name"":""html""}]")]
    [DataRow("<!doctypeHTML>", @"[{""type"":""doctype"",""name"":""html""}]")]
    [DataRow("<!doctypehtml>", @"[{""type"":""doctype"",""name"":""html""}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}