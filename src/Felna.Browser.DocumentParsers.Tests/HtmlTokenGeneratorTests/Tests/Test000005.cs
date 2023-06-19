using Felna.Browser.DocumentParsers.TextReferences;

namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Test000005
{
    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(4)]
    [DataRow(5)]
    [DataRow(6)]
    public void GivenIncompleteDoctypeCommentTokenReturnedThenEndOfFile(int partialLength)
    {
        var partialDoctype = StringReference.DocType[..partialLength];
        var html = @"<!" + partialDoctype;
        var tokens = new List<HtmlToken>
        {
            new CommentToken {Data = partialDoctype},
            new EndOfFileToken(),
        };
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}