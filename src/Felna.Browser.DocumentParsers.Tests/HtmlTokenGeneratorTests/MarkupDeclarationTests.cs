namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests;

[TestClass]
public class MarkupDeclarationTests
{
    [TestMethod]
    [DataRow("<!", @"[{""type"":""comment"",""data"":""""}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}