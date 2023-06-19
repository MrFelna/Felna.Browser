namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Test000002
{
    [TestMethod]
    public void GivenEmptyHtmlBasicOnlyEndOfFileReturned()
    {
        const string html = @"";
        var tokens = new List<HtmlToken>
        {
            new EndOfFileToken(),
        };
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    } 
}