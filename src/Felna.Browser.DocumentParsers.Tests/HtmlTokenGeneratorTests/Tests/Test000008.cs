namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Test000008
{
    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(4)]
    public void GivenStartOfDoctypeTokenWithSpaceAndPartialNameDoctypeTokenReturnedThenEndOfFile(int partialLength)
    {
        var partialDoctypeName = "html"[..partialLength];
        var html = @"<!DOCTYPE " + partialDoctypeName;
        var tokens = new List<HtmlToken>
        {
            new DocTypeToken {Name = partialDoctypeName, ForceQuirks = true},
            new EndOfFileToken(),
        };
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}