namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Test000009
{
    [TestMethod]
    [DataRow("<!DOCTYPE HTML>")]
    [DataRow("<!doctype HTML>")]
    [DataRow("<!doctype html>")]
    public void GivenValidButNoContentHtmlBasicInVariousCasesEmptyDocumentReturnedThenEndOfFile(string html)
    {
        var tokens = new List<HtmlToken>
        {
            new DocTypeToken
            {
                Name = "html",
            },
            new EndOfFileToken(),
        };
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    } 
}