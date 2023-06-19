namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Test000006
{
    [TestMethod]
    public void GivenStartOfDoctypeTokenDoctypeTokenReturnedWithForceQuirksThenEndOfFile()
    {
        const string html = @"<!DOCTYPE";
        var tokens = new List<HtmlToken>
        {
            new DocTypeToken {ForceQuirks = true},
            new EndOfFileToken(),
        };
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    } 
}