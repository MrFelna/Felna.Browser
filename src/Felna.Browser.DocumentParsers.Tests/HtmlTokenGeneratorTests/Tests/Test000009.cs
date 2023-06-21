namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Test000009
{
    [TestMethod]
    [DataRow("<!DOCTYPE HTML>")]
    [DataRow("<!DOCTYPE html>")]
    [DataRow("<!doctype HTML>")]
    [DataRow("<!doctype html>")]
    [DataRow("<!DOCTYPE  HTML>")]
    [DataRow("<!DOCTYPE  html>")]
    [DataRow("<!doctype  HTML>")]
    [DataRow("<!doctype  html>")]
    [DataRow("<!DOCTYPEHTML>")]
    [DataRow("<!DOCTYPEhtml>")]
    [DataRow("<!doctypeHTML>")]
    [DataRow("<!doctypehtml>")]
    [DataRow("<!DOCTYPE HTML >")]
    [DataRow("<!DOCTYPE html >")]
    [DataRow("<!doctype HTML >")]
    [DataRow("<!doctype html >")]
    public void GivenValidButNoContentHtmlBasicInVariousCasesAndContainingWhitespaceEmptyDocumentReturnedThenEndOfFile(string html)
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