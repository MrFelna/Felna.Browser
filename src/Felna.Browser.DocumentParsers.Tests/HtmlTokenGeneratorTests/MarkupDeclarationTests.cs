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
    [DataRow("<!DOCTYPE", @"[{""type"":""doctype"",""forcequirks"":true}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}