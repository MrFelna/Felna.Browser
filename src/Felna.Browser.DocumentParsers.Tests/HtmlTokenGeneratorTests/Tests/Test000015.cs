using Felna.Browser.DocumentParsers.TextReferences;

namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Test000015
{
    [TestMethod]
    [DataRow("<!DOCTYPE html public \"id\"")]
    [DataRow("<!DOCTYPE html public \"id\" ")]
    [DataRow("<!DOCTYPE html public \"id\">")]
    [DataRow("<!DOCTYPE html public \"id\" >")]
    [DataRow("<!DOCTYPE html public 'id'")]
    [DataRow("<!DOCTYPE html public 'id' ")]
    [DataRow("<!DOCTYPE html public 'id'>")]
    [DataRow("<!DOCTYPE html public 'id' >")]
    public void GivenDoctypeWithPublicIdentifierDoctypeTokenReturnedThenEndOfFile(string html)
    {
        ArgumentException.ThrowIfNullOrEmpty(html);
        var tagIsClosed = html.Contains(CharacterReference.GreaterThanSign, StringComparison.InvariantCulture);
        var tokens = new List<HtmlToken>
        {
            new DocTypeToken {Name = $"html", PublicIdentifier = "id", ForceQuirks = !tagIsClosed},
            new EndOfFileToken(),
        };
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    } 
}