using System.Reflection;

namespace Felna.Browser.DocumentParsers.Tests.HtmlParserTests;

internal static class HtmlParserTestRunner
{
    internal static void Run(string input, DocumentNode expectedResult)
    {
        // act
        var actual = HtmlParser.Parse(input);
        
        // assert
        
        // documents should be the tree roots
        Assert.IsNull(expectedResult.ParentNode);
        Assert.IsNull(actual.ParentNode);
        
        CompareNodes(expectedResult, actual);
    }

    private static void CompareNodes(IDocumentNode expected, IDocumentNode actual)
    {
        var nodeType = expected.GetType();
        
        Assert.AreEqual(nodeType, actual.GetType());

        var properties = nodeType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var propertyInfo in properties)
        {
            if (propertyInfo.Name is nameof(IDocumentNode.ChildNodes) or nameof(IDocumentNode.ParentNode))
                continue;
            
            Assert.AreEqual(propertyInfo.GetValue(expected), propertyInfo.GetValue(actual));
        }

        var nodeCount = expected.ChildNodes.Count;
        Assert.AreEqual(nodeCount, actual.ChildNodes.Count, $"Child node count differs for node {expected.NodeName}");

        for (var i = 0; i < nodeCount; i++)
        {
            var expectedNode = expected.ChildNodes[i];
            var actualNode = expected.ChildNodes[i];
            
            // check parents so the recursive call doesn't have to check
            Assert.AreEqual(expected, expectedNode.ParentNode);
            Assert.AreEqual(actual, actualNode.ParentNode);
            
            CompareNodes(expectedNode, actualNode);
        }
    }
}