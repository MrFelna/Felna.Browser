namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Test000004
{
    [TestMethod]
    public void GivenOnlyOpenBracketAndExclamationCommentTokenReturnedThenEndOfFile()
    {
        const string html = @"<!";
        var tokens = new List<HtmlToken>
        {
            new CommentToken {Data = string.Empty},
            new EndOfFileToken(),
        };
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    } 
}