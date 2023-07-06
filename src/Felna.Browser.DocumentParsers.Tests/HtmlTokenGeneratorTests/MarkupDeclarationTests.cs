using Felna.Browser.DocumentParsers.TextReferences;

namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests;

[TestClass]
public class MarkupDeclarationTests
{
    [TestMethod]
    [DataRow("<!", @"[{""type"":""comment"",""data"":""""}]")]
    [DataRow("<!D", @"[{""type"":""comment"",""data"":""D""}]")]
    [DataRow("<!DO", @"[{""type"":""comment"",""data"":""DO""}]")]
    [DataRow("<!DOC", @"[{""type"":""comment"",""data"":""DOC""}]")]
    [DataRow("<!DOCT", @"[{""type"":""comment"",""data"":""DOCT""}]")]
    [DataRow("<!DOCTY", @"[{""type"":""comment"",""data"":""DOCTY""}]")]
    [DataRow("<!DOCTYP", @"[{""type"":""comment"",""data"":""DOCTYP""}]")]
    [DataRow("<!DOC\u0000TYPE>", "[{\"type\":\"comment\",\"data\":\"DOC\ufffdTYPE\"}]")]
    [DataRow("<!DOCTYPE", @"[{""type"":""doctype"",""forcequirks"":true}]")]
    [DataRow("<!DOCTYPE>", @"[{""type"":""doctype"",""forcequirks"":true}]")]
    [DataRow("<!DOCTYPE ", @"[{""type"":""doctype"",""forcequirks"":true}]")]
    [DataRow("<!DOCTYPE h", @"[{""type"":""doctype"",""name"":""h"",""forcequirks"":true}]")]
    [DataRow("<!DOCTYPE ht", @"[{""type"":""doctype"",""name"":""ht"",""forcequirks"":true}]")]
    [DataRow("<!DOCTYPE htm", @"[{""type"":""doctype"",""name"":""htm"",""forcequirks"":true}]")]
    [DataRow("<!DOCTYPE html", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    [DataRow("<!DOCTYPE html ", @"[{""type"":""doctype"",""name"":""html"",""forcequirks"":true}]")]
    [DataRow("<!DOCTYPE HTML>", @"[{""type"":""doctype"",""name"":""html""}]")]
    [DataRow("<!DOCTYPE html>", @"[{""type"":""doctype"",""name"":""html""}]")]
    [DataRow("<!doctype HTML>", @"[{""type"":""doctype"",""name"":""html""}]")]
    [DataRow("<!doctype html>", @"[{""type"":""doctype"",""name"":""html""}]")]
    [DataRow("<!DOCTYPE  HTML>", @"[{""type"":""doctype"",""name"":""html""}]")]
    [DataRow("<!DOCTYPE  html>", @"[{""type"":""doctype"",""name"":""html""}]")]
    [DataRow("<!doctype  HTML>", @"[{""type"":""doctype"",""name"":""html""}]")]
    [DataRow("<!doctype  html>", @"[{""type"":""doctype"",""name"":""html""}]")]
    [DataRow("<!DOCTYPEHTML>", @"[{""type"":""doctype"",""name"":""html""}]")]
    [DataRow("<!DOCTYPEhtml>", @"[{""type"":""doctype"",""name"":""html""}]")]
    [DataRow("<!doctypeHTML>", @"[{""type"":""doctype"",""name"":""html""}]")]
    [DataRow("<!doctypehtml>", @"[{""type"":""doctype"",""name"":""html""}]")]
    [DataRow("<!DOCTYPE HTML >", @"[{""type"":""doctype"",""name"":""html""}]")]
    [DataRow("<!DOCTYPE html >", @"[{""type"":""doctype"",""name"":""html""}]")]
    [DataRow("<!doctype HTML >", @"[{""type"":""doctype"",""name"":""html""}]")]
    [DataRow("<!doctype html >", @"[{""type"":""doctype"",""name"":""html""}]")]
    [DataRow("<!DOCTYPE ht\u0000ml>", "[{\"type\":\"doctype\",\"name\":\"ht\ufffdml\"}]")]
    [DataRow("<!doctype html test", @"[{""type"":""doctype"",""name"":""html""}]")]
    [DataRow("<!doctype html test>", @"[{""type"":""doctype"",""name"":""html""}]")]
    [DataRow("<!doctype html test >", @"[{""type"":""doctype"",""name"":""html""}]")]
    [DataRow("<!doctype html test \u0000", @"[{""type"":""doctype"",""name"":""html""}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}