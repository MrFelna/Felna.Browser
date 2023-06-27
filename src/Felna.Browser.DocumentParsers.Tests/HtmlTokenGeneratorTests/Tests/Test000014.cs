namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Test000014
{
    [TestMethod]
    [DataRow("<!DOCTYPE html test")]
    [DataRow("<!DOCTYPE html test>")]
    [DataRow("<!DOCTYPE html test >")]
    [DataRow("<!DOCTYPE html test \u0000")]
    public void GivenBogusDoctypeTokenDoctypeTokenReturnedThenEndOfFile(string html)
    {
        var tokens = new List<HtmlToken>
        {
            new DocTypeToken {Name = $"html"},
            new EndOfFileToken(),
        };
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    } 
}