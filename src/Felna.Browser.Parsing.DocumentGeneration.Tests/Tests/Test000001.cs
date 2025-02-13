namespace Felna.Browser.Parsing.DocumentGeneration.Tests.Tests;

[TestClass]
public class Test000001
{
    [TestMethod, Ignore]
    public void GivenValidButNoContentHtmlBasicEmptyDocumentReturned()
    {
        var html = @"<!DOCTYPE html><html><head></head><body></body></html>";
        var document = new DocumentNode();

        var htmlNode = new ElementNode(null, "html") {ParentNode = document};

        var headNode = new ElementNode(null, "head") {ParentNode = htmlNode};
        var bodyNode = new ElementNode(null, "body") {ParentNode = htmlNode};
        
        document.AppendNode(new DocumentTypeNode{ParentNode = document});
        document.AppendNode(htmlNode);
        
        htmlNode.AppendNode(headNode);
        htmlNode.AppendNode(bodyNode);
        
        HtmlParserTestRunner.Run(html, document);
    } 
}