using System.Globalization;

namespace Felna.Browser.Parsing.TokenGeneration.Tests.Tests;

[TestClass]
public class Tokenization080NumericCharacterReferenceEndStateTests
{
    [TestMethod]
    // Zero
    [DataRow("&#x0", "[{\"type\":\"character\",\"data\":\"\ufffd\"}]")]
    // Too big
    [DataRow("&#x110000", "[{\"type\":\"character\",\"data\":\"\ufffd\"}]")]
    // Surrogate
    [DataRow("&#xD800", "[{\"type\":\"character\",\"data\":\"\ufffd\"}]")]
    [DataRow("&#xDFFF", "[{\"type\":\"character\",\"data\":\"\ufffd\"}]")]
    // Non character
    [DataRow("&#xFD00", "[{\"type\":\"character\",\"data\":\"\ufd00\"}]")]
    [DataRow("&#xFDEF", "[{\"type\":\"character\",\"data\":\"\ufdef\"}]")]
    [DataRow("&#xFFFE", "[{\"type\":\"character\",\"data\":\"\ufffe\"}]")]
    [DataRow("&#xFFFF", "[{\"type\":\"character\",\"data\":\"\uffff\"}]")]
    [DataRow("&#x1FFFE", "[{\"type\":\"character\",\"data\":\"\uD83F\uDFFE\"}]")]
    [DataRow("&#x1FFFF", "[{\"type\":\"character\",\"data\":\"\uD83F\uDFFF\"}]")]
    [DataRow("&#x2FFFE", "[{\"type\":\"character\",\"data\":\"\uD87F\uDFFE\"}]")]
    [DataRow("&#x2FFFF", "[{\"type\":\"character\",\"data\":\"\uD87F\uDFFF\"}]")]
    [DataRow("&#x3FFFE", "[{\"type\":\"character\",\"data\":\"\uD8BF\uDFFE\"}]")]
    [DataRow("&#x3FFFF", "[{\"type\":\"character\",\"data\":\"\uD8BF\uDFFF\"}]")]
    [DataRow("&#x4FFFE", "[{\"type\":\"character\",\"data\":\"\uD8FF\uDFFE\"}]")]
    [DataRow("&#x4FFFF", "[{\"type\":\"character\",\"data\":\"\uD8FF\uDFFF\"}]")]
    [DataRow("&#x5FFFE", "[{\"type\":\"character\",\"data\":\"\uD93F\uDFFE\"}]")]
    [DataRow("&#x5FFFF", "[{\"type\":\"character\",\"data\":\"\uD93F\uDFFF\"}]")]
    [DataRow("&#x6FFFE", "[{\"type\":\"character\",\"data\":\"\uD97F\uDFFE\"}]")]
    [DataRow("&#x6FFFF", "[{\"type\":\"character\",\"data\":\"\uD97F\uDFFF\"}]")]
    [DataRow("&#x7FFFE", "[{\"type\":\"character\",\"data\":\"\uD9BF\uDFFE\"}]")]
    [DataRow("&#x7FFFF", "[{\"type\":\"character\",\"data\":\"\uD9BF\uDFFF\"}]")]
    [DataRow("&#x8FFFE", "[{\"type\":\"character\",\"data\":\"\uD9FF\uDFFE\"}]")]
    [DataRow("&#x8FFFF", "[{\"type\":\"character\",\"data\":\"\uD9FF\uDFFF\"}]")]
    [DataRow("&#x9FFFE", "[{\"type\":\"character\",\"data\":\"\uDA3F\uDFFE\"}]")]
    [DataRow("&#x9FFFF", "[{\"type\":\"character\",\"data\":\"\uDA3F\uDFFF\"}]")]
    [DataRow("&#xAFFFE", "[{\"type\":\"character\",\"data\":\"\uDA7F\uDFFE\"}]")]
    [DataRow("&#xAFFFF", "[{\"type\":\"character\",\"data\":\"\uDA7F\uDFFF\"}]")]
    [DataRow("&#xBFFFE", "[{\"type\":\"character\",\"data\":\"\uDABF\uDFFE\"}]")]
    [DataRow("&#xBFFFF", "[{\"type\":\"character\",\"data\":\"\uDABF\uDFFF\"}]")]
    [DataRow("&#xCFFFE", "[{\"type\":\"character\",\"data\":\"\uDAFF\uDFFE\"}]")]
    [DataRow("&#xCFFFF", "[{\"type\":\"character\",\"data\":\"\uDAFF\uDFFF\"}]")]
    [DataRow("&#xDFFFE", "[{\"type\":\"character\",\"data\":\"\uDB3F\uDFFE\"}]")]
    [DataRow("&#xDFFFF", "[{\"type\":\"character\",\"data\":\"\uDB3F\uDFFF\"}]")]
    [DataRow("&#xEFFFE", "[{\"type\":\"character\",\"data\":\"\uDB7F\uDFFE\"}]")]
    [DataRow("&#xEFFFF", "[{\"type\":\"character\",\"data\":\"\uDB7F\uDFFF\"}]")]
    [DataRow("&#xFFFFE", "[{\"type\":\"character\",\"data\":\"\uDBBF\uDFFE\"}]")]
    [DataRow("&#xFFFFF", "[{\"type\":\"character\",\"data\":\"\uDBBF\uDFFF\"}]")]
    [DataRow("&#x10FFFE", "[{\"type\":\"character\",\"data\":\"\uDBFF\uDFFE\"}]")]
    [DataRow("&#x10FFFF", "[{\"type\":\"character\",\"data\":\"\uDBFF\uDFFF\"}]")]
    // table
    [DataRow("&#x80", "[{\"type\":\"character\",\"data\":\"\u20ac\"}]")]
    [DataRow("&#x82", "[{\"type\":\"character\",\"data\":\"\u201a\"}]")]
    [DataRow("&#x83", "[{\"type\":\"character\",\"data\":\"\u0192\"}]")]
    [DataRow("&#x84", "[{\"type\":\"character\",\"data\":\"\u201e\"}]")]
    [DataRow("&#x85", "[{\"type\":\"character\",\"data\":\"\u2026\"}]")]
    [DataRow("&#x86", "[{\"type\":\"character\",\"data\":\"\u2020\"}]")]
    [DataRow("&#x87", "[{\"type\":\"character\",\"data\":\"\u2021\"}]")]
    [DataRow("&#x88", "[{\"type\":\"character\",\"data\":\"\u02c6\"}]")]
    [DataRow("&#x89", "[{\"type\":\"character\",\"data\":\"\u2030\"}]")]
    [DataRow("&#x8A", "[{\"type\":\"character\",\"data\":\"\u0160\"}]")]
    [DataRow("&#x8B", "[{\"type\":\"character\",\"data\":\"\u2039\"}]")]
    [DataRow("&#x8C", "[{\"type\":\"character\",\"data\":\"\u0152\"}]")]
    [DataRow("&#x8E", "[{\"type\":\"character\",\"data\":\"\u017d\"}]")]
    [DataRow("&#x91", "[{\"type\":\"character\",\"data\":\"\u2018\"}]")]
    [DataRow("&#x92", "[{\"type\":\"character\",\"data\":\"\u2019\"}]")]
    [DataRow("&#x93", "[{\"type\":\"character\",\"data\":\"\u201c\"}]")]
    [DataRow("&#x94", "[{\"type\":\"character\",\"data\":\"\u201d\"}]")]
    [DataRow("&#x95", "[{\"type\":\"character\",\"data\":\"\u2022\"}]")]
    [DataRow("&#x96", "[{\"type\":\"character\",\"data\":\"\u2013\"}]")]
    [DataRow("&#x97", "[{\"type\":\"character\",\"data\":\"\u2014\"}]")]
    [DataRow("&#x98", "[{\"type\":\"character\",\"data\":\"\u02dc\"}]")]
    [DataRow("&#x99", "[{\"type\":\"character\",\"data\":\"\u2122\"}]")]
    [DataRow("&#x9A", "[{\"type\":\"character\",\"data\":\"\u0161\"}]")]
    [DataRow("&#x9B", "[{\"type\":\"character\",\"data\":\"\u203a\"}]")]
    [DataRow("&#x9C", "[{\"type\":\"character\",\"data\":\"\u0153\"}]")]
    [DataRow("&#x9E", "[{\"type\":\"character\",\"data\":\"\u017e\"}]")]
    [DataRow("&#x9F", "[{\"type\":\"character\",\"data\":\"\u0178\"}]")]
    public void GivenHtmlCorrectTokensGenerated(string html, string json)
    {
        var tokens = HtmlTokenGeneratorTestRunner.ConvertJsonToTokens(json);

        HtmlTokenGeneratorTestRunner.Run(html, tokens);
    }

    [TestMethod]
    public void EnsureAllNumbersMatch()
    {
        var codePointMap = new Dictionary<int, int>
        {
            {0x80, 0x20ac},
            {0x82, 0x201a},
            {0x83, 0x0192},
            {0x84, 0x201e},
            {0x85, 0x2026},
            {0x86, 0x2020},
            {0x87, 0x2021},
            {0x88, 0x02c6},
            {0x89, 0x2030},
            {0x8a, 0x0160},
            {0x8b, 0x2039},
            {0x8c, 0x0152},
            {0x8e, 0x017d},
            {0x91, 0x2018},
            {0x92, 0x2019},
            {0x93, 0x201c},
            {0x94, 0x201d},
            {0x95, 0x2022},
            {0x96, 0x2013},
            {0x97, 0x2014},
            {0x98, 0x02dc},
            {0x99, 0x2122},
            {0x9a, 0x0161},
            {0x9b, 0x203a},
            {0x9c, 0x0153},
            {0x9e, 0x017e},
            {0x9f, 0x0178},
        };
        
        for (var i = 0; i < 0x111111; i++)
        {
            var hex = i.ToString("X", CultureInfo.InvariantCulture);
            var variations = GenerateAllCaseVariationsOf(hex);
            
            var formats = new List<string> { $"&#{i};" };
            formats.AddRange(variations.Select(v => $"&#x{v};"));

            var codePoint= i switch
            {
                0 or > 0x10ffff or >= 0xd800 and <= 0xdfff => 0xfffd,
                _ => i
            };
            if (codePointMap.TryGetValue(codePoint, out var value))
            {
                codePoint = value;
            }

            var tokens = new HtmlToken[] { new CharacterToken { Data = char.ConvertFromUtf32(codePoint) } };

            foreach (var format in formats)
            {
                HtmlTokenGeneratorTestRunner.Run(format, tokens);
            }
        }
    }

    private static List<string> GenerateAllCaseVariationsOf(string input)
    {
        ArgumentException.ThrowIfNullOrEmpty(input);

        var firstChar = new List<string> { input[0].ToString() };
        
        if (input[0] >= 'a' && input[0] <= 'z')
            firstChar.Add(((char)(input[0] - 32)).ToString());
        if (input[0] >= 'A' && input[0] <= 'Z')
            firstChar.Add(((char)(input[0] + 32)).ToString());

        if (input.Length == 1)
        {
            return firstChar;
        }

        var otherVariations = GenerateAllCaseVariationsOf(input[1..]);

        return (from variation in otherVariations from first in firstChar select first + variation).ToList();
    }
}