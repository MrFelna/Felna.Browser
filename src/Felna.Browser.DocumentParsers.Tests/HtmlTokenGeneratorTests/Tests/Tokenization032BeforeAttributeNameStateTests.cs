namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Tokenization032BeforeAttributeNameStateTests
{
    [TestMethod]
    // Tab
    [DataRow("<p \t>", @"[{""type"":""tag"",""name"":""p""}]")]
    [DataRow("<p a=b \t>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""b""}}]")]
    [DataRow("<p a='b' \t>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""b""}}]")]
    [DataRow("<p /\t>", @"[{""type"":""tag"",""name"":""p""}]")]
    // Line feed
    [DataRow("<p \n>", @"[{""type"":""tag"",""name"":""p""}]")]
    [DataRow("<p a=b \n>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""b""}}]")]
    [DataRow("<p a='b' \n>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""b""}}]")]
    [DataRow("<p /\n>", @"[{""type"":""tag"",""name"":""p""}]")]
    // Form feed
    [DataRow("<p \f>", @"[{""type"":""tag"",""name"":""p""}]")]
    [DataRow("<p a=b \f>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""b""}}]")]
    [DataRow("<p a='b' \f>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""b""}}]")]
    [DataRow("<p /\f>", @"[{""type"":""tag"",""name"":""p""}]")]
    // Space
    [DataRow("<p  >", @"[{""type"":""tag"",""name"":""p""}]")]
    [DataRow("<p a=b  >", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""b""}}]")]
    [DataRow("<p a='b'  >", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""b""}}]")]
    [DataRow("<p / >", @"[{""type"":""tag"",""name"":""p""}]")]
    // Solidus
    [DataRow("<p />", @"[{""type"":""tag"",""name"":""p"",""selfclosing"":true}]")]
    [DataRow("<p a=b />", @"[{""type"":""tag"",""name"":""p"",""selfclosing"":true,""attributes"":{""a"":""b""}}]")]
    [DataRow("<p a='b' />", @"[{""type"":""tag"",""name"":""p"",""selfclosing"":true,""attributes"":{""a"":""b""}}]")]
    [DataRow("<p //>", @"[{""type"":""tag"",""name"":""p"",""selfclosing"":true}]")]
    // Greater-than sign
    [DataRow("<p >", @"[{""type"":""tag"",""name"":""p""}]")]
    [DataRow("<p a=b >", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""b""}}]")]
    [DataRow("<p a='b' >", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""b""}}]")]
    [DataRow("<p />", @"[{""type"":""tag"",""name"":""p"",""selfclosing"":true}]")]
    // EOF
    [DataRow("<p ", "[]")]
    [DataRow("<p a=b ", "[]")]
    [DataRow("<p a='b' ", "[]")]
    [DataRow("<p /", "[]")]
    // Equals
    [DataRow("<p =c=d>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""=c"":""d""}}]")]
    [DataRow("<p a=b =c=d>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""b"",""=c"":""d""}}]")]
    [DataRow("<p a='b' =c=d>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""b"",""=c"":""d""}}]")]
    [DataRow("<p /=c=d>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""=c"":""d""}}]")]
    // Anything else
    [DataRow("<p c=d>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""c"":""d""}}]")]
    [DataRow("<p a=b c=d>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""b"",""c"":""d""}}]")]
    [DataRow("<p a='b' c=d>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""b"",""c"":""d""}}]")]
    [DataRow("<p /c=d>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""c"":""d""}}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}