namespace Felna.Browser.Parsing.TokenGeneration.Tests.Tests;

[TestClass]
public class Tokenization007EndTagOpenStateTests
{
    [TestMethod]
    // Alpha
    [DataRow("</p>", @"[{""type"":""tag"",""name"":""p"",""selfclosing"":false,""isendtag"":true}]")]
    // Greater than sign
    [DataRow("</>", @"[]")]
    // EOF
    [DataRow("</", @"[{""type"":""character"",""data"":""<""},{""type"":""character"",""data"":""/""}]")]
    // Anything else
    [DataRow("</1", @"[{""type"":""comment"",""data"":""1""}]")]
    [DataRow("</<", @"[{""type"":""comment"",""data"":""<""}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}