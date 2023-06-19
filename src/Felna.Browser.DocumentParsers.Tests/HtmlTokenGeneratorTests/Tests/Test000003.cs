namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Test000003
{
    [TestMethod]
    public void GivenOnlyOpenBracketSingleCharacterTokenReturnedThenEndOfFile()
    {
        const string html = @"<";
        var tokens = new List<HtmlToken>
        {
            new CharacterToken {Data = "<"},
            new EndOfFileToken(),
        };
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    } 
}