using System.Text;
using Felna.Browser.DocumentParsers.HtmlTokens;

namespace Felna.Browser.DocumentParsers.Tests.HtmlTokenGeneratorTests;

public static class HtmlTokenGeneratorTestRunner
{
    
    internal static void Run(string input, IReadOnlyList<HtmlToken> expectedTokens)
    {
        // arrange
        var streamConsumer = new TestStreamConsumer(input);
        var tokenGenerator = new HtmlTokenGenerator(streamConsumer);
        var tokens = new List<HtmlToken>();
        
        // act
        while (tokens.Count <= expectedTokens.Count)
        {
            var nextToken = tokenGenerator.GetNextToken();
            tokens.Add(nextToken);
            if (nextToken is EndOfFileToken)
                break;
        }
        
        // assert
        Assert.AreEqual(expectedTokens.Count, tokens.Count);

        for (var i = 0; i < tokens.Count; i++)
        {
            Assert.IsTrue(expectedTokens[i].AreValueEqual(tokens[i]));
        }
    }
}