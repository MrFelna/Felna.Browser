namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Test000010
{
    [TestMethod]
    public void GivenStartOfDoctypeTokenWithGreaterThanSignDoctypeTokenReturnedWithForceQuirksThenEndOfFile()
    {
        const string html = @"<!DOCTYPE>";
        var tokens = new List<HtmlToken>
        {
            new DocTypeToken {ForceQuirks = true},
            new EndOfFileToken(),
        };
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    } 
}