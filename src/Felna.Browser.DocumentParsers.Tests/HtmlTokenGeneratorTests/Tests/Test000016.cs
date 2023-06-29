namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Test000016
{
    [TestMethod]
    [DataRow("<!DOCTYPE html public id")]
    [DataRow("<!DOCTYPE html public")]
    [DataRow("<!DOCTYPE html public>")]
    public void GivenDoctypeWithIncompletePublicIdentifierDoctypeTokenReturnedThenEndOfFile(string html)
    {
        var tokens = new List<HtmlToken>
        {
            new DocTypeToken {Name = $"html", ForceQuirks = true},
            new EndOfFileToken(),
        };
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    } 
}