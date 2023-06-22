using Felna.Browser.DocumentParsers.TextReferences;

namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Test000013
{
    [TestMethod]
    public void GivenDoctypeTokenWithNullInNameDoctypeTokenReturnedThenEndOfFile()
    {
        const string html = "<!DOCTYPE ht\u0000ml>";
        var tokens = new List<HtmlToken>
        {
            new DocTypeToken {Name = $"ht{CharacterReference.ReplacementCharacter}ml"},
            new EndOfFileToken(),
        };
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    } 
}