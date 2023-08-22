namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Tokenization042MarkupDeclarationOpenStateTests
{
    [TestMethod]
    // TODO: Double Hyphen
    // ASCII case-insensitive DOCTYPE
    [DataRow("<!DOCTYPE html>", @"[{""type"":""doctype"",""name"":""html""}]")]
    [DataRow("<!doctype html>", @"[{""type"":""doctype"",""name"":""html""}]")]
    // TODO: CDATA
    // Anything else
    [DataRow("<!", @"[{""type"":""comment"",""data"":""""}]")]
    [DataRow("<!D", @"[{""type"":""comment"",""data"":""D""}]")]
    [DataRow("<!DO", @"[{""type"":""comment"",""data"":""DO""}]")]
    [DataRow("<!DOC", @"[{""type"":""comment"",""data"":""DOC""}]")]
    [DataRow("<!DOCT", @"[{""type"":""comment"",""data"":""DOCT""}]")]
    [DataRow("<!DOCTY", @"[{""type"":""comment"",""data"":""DOCTY""}]")]
    [DataRow("<!DOCTYP", @"[{""type"":""comment"",""data"":""DOCTYP""}]")]
    [DataRow("<!DOC\u0000TYPE>", "[{\"type\":\"comment\",\"data\":\"DOC\ufffdTYPE\"}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}