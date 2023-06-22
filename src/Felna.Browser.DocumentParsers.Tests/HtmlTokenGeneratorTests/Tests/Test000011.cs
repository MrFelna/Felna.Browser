using Felna.Browser.DocumentParsers.TextReferences;

namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Test000011
{
    [TestMethod]
    public void GivenBogusCommentTokenWithNullCharacterAndGreaterThanSignCommentTokenReturnedThenEndOfFile()
    {
        const string html = "<!DOC\u0000TYPE>";
        var tokens = new List<HtmlToken>
        {
            new CommentToken{Data = $"DOC{CharacterReference.ReplacementCharacter}TYPE"},
            new EndOfFileToken(),
        };
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    } 
}