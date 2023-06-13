using System.Text;
using Felna.Browser.DocumentParsers.HtmlTokens;

namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests;

public static class HtmlTokenGeneratorTestRunner
{
    
    internal static void Run(string input, IReadOnlyList<HtmlToken> expectedTokens)
    {
        // arrange
        var utf8Bytes = Encoding.UTF8.GetBytes(input);
        var stream = new MemoryStream(utf8Bytes);
        
        
        // act
        var actual = HtmlTokenGenerator.TokenizeStream(stream, Encoding.UTF8.GetDecoder()).ToList();
        
        // assert
        Assert.AreEqual(expectedTokens.Count, actual.Count);

        for (var i = 0; i < actual.Count; i++)
        {
            Assert.IsTrue(expectedTokens[i].AreValueEqual(actual[i]));
        }
    }
}