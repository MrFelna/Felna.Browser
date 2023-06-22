namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Test000012
{
    [TestMethod]
    public void GivenDoctypeTokenWithHtmlAndSpaceDoctypeTokenReturnedWithForceQuirksThenEndOfFile()
    {
        const string html = @"<!DOCTYPE html ";
        var tokens = new List<HtmlToken>
        {
            new DocTypeToken {Name = "html", ForceQuirks = true},
            new EndOfFileToken(),
        };
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    } 
}