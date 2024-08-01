﻿namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Tokenization034AfterAttributeNameStateTests
{
    [TestMethod]
    // Tab
    [DataRow("<p a \t>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""""}}]")]
    // Line feed
    [DataRow("<p a \n>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""""}}]")]
    // Form feed
    [DataRow("<p a \f>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""""}}]")]
    // Space
    [DataRow("<p a  >", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""""}}]")]
    // Solidus
    [DataRow("<p a />", @"[{""type"":""tag"",""name"":""p"",""selfclosing"":true,""attributes"":{""a"":""""}}]")]
    // Equals
    [DataRow("<p a = b >", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""b""}}]")]
    // Greater-than sign
    [DataRow("<p a >", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":""""}}]")]
    // EOF
    [DataRow("<p a ", "[]")]
    // Anything else
    [DataRow("<p a b>", @"[{""type"":""tag"",""name"":""p"",""attributes"":{""a"":"""",""b"":""""}}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }
}