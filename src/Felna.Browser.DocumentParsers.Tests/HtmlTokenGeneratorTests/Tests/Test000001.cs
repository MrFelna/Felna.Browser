﻿namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests.Tests;

[TestClass]
public class Test000001
{
    [TestMethod]
    public void GivenValidButNoContentHtmlBasicEmptyDocumentReturnedThenEndOfFile()
    {
        const string html = @"<!DOCTYPE html>";
        var tokens = new List<HtmlToken>
        {
            new DocTypeToken
            {
                Name = "html",
            },
            new EndOfFileToken(),
        };
        
        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    } 
}