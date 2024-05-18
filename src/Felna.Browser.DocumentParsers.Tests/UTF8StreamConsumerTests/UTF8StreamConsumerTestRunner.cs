using System.Text;
using Felna.Browser.DocumentParsers.StreamConsumers;
using Felna.Browser.DocumentParsers.TextReferences;

namespace Felna.Browser.DocumentParsers.Tests.UTF8StreamConsumerTests;

[TestClass]
public class UTF8StreamConsumerTestRunner
{

    [TestMethod]
    [DataRow("Basic", 'B')]
    [DataRow("\r\n", '\r')]
    [DataRow("\t", '\t')]
    [DataRow("\r", '\r')]
    [DataRow("\n", '\n')]
    [DataRow("\u03B2", '\u03B2')] // greek letter lower beta
    public void GivenSourceStringCurrentCharCorrectlyReturned(string input, char expected)
    {
        // arrange
        var utf8Bytes = Encoding.UTF8.GetBytes(input);
        var stream = new MemoryStream(utf8Bytes);
        var consumer = new UTF8StreamConsumer(stream);
        
        // act
        var (success, character) = consumer.TryGetCurrentCodePoint();
        
        // assert
        Assert.IsTrue(success);
        Assert.AreEqual(new UnicodeCodePoint(expected), character);
    }

    [TestMethod]
    public void GivenEmojiStringDoubleUTF16CharStringProduced()
    {
        var smilingFaceWithSmilingEyes = char.ConvertFromUtf32(0x1F60A);
        var utf8Bytes = Encoding.UTF8.GetBytes(smilingFaceWithSmilingEyes);
        var stream = new MemoryStream(utf8Bytes);
        var consumer = new UTF8StreamConsumer(stream);
        
        // act
        var (success, result) = consumer.LookAhead(1);
        
        // assert
        Assert.IsTrue(success);
        Assert.AreEqual(smilingFaceWithSmilingEyes, result);
        
    }
    
    [TestMethod]
    [DataRow("Basic")]
    [DataRow("\r\n")]
    [DataRow("\t")]
    [DataRow("\r")]
    [DataRow("\n")]
    public void GivenSourceStringDoubleRetrieveCurrentCharReturnsSameThing(string input)
    {
        // arrange
        var utf8Bytes = Encoding.UTF8.GetBytes(input);
        var stream = new MemoryStream(utf8Bytes);
        var consumer = new UTF8StreamConsumer(stream);
        
        // act
        var (success1, character1) = consumer.TryGetCurrentCodePoint();
        var (success2, character2) = consumer.TryGetCurrentCodePoint();
        
        // assert
        Assert.IsTrue(success1);
        Assert.IsTrue(success2);
        Assert.AreEqual(character1, character2);
    }

    [TestMethod]
    [DataRow("Basic", 3, true, "Bas")]
    [DataRow("Basic", 8, false, "")]
    [DataRow("\r\n", 3, false, "")]
    [DataRow("\r\n", 2, true, "\r\n")]
    public void LookAheadGivesSubStringOfInput(string input, int lookAheadCount, bool expectedSuccess, string expectedResult)
    {
        // arrange
        var utf8Bytes = Encoding.UTF8.GetBytes(input);
        var stream = new MemoryStream(utf8Bytes);
        var consumer = new UTF8StreamConsumer(stream);
        
        // act
        var (actualSuccess, actualResult) = consumer.LookAhead(lookAheadCount);
        
        // assert
        Assert.AreEqual(expectedSuccess, actualSuccess);
        if (expectedSuccess)
        {
            Assert.AreEqual(expectedResult, actualResult);
        }
    }

    [TestMethod]
    [DataRow(1, "Basic", true, 'a')]
    [DataRow(5, "Basic", false, CharacterReference.ReplacementCharacter)]
    [DataRow(8, "Basic", false, CharacterReference.ReplacementCharacter)]
    [DataRow(5,"\r\n", false, CharacterReference.ReplacementCharacter)]
    [DataRow(2,"\r\n", false, CharacterReference.ReplacementCharacter)]
    [DataRow(1,"\r\n", true, '\n')]
    public void ConsumeThenTryGetCurrentCharGivesCharOfInput(int consumeCount, string input, bool expectedSuccess, char expectedCharacter)
    {
        // arrange
        var utf8Bytes = Encoding.UTF8.GetBytes(input);
        var stream = new MemoryStream(utf8Bytes);
        var consumer = new UTF8StreamConsumer(stream);
        
        // act
        consumer.ConsumeCodePoint(consumeCount);
        var (actualSuccess, actualCharacter) = consumer.TryGetCurrentCodePoint();
        
        // assert
        Assert.AreEqual(expectedSuccess, actualSuccess);
        Assert.AreEqual(new UnicodeCodePoint(expectedCharacter), actualCharacter);
    }

    [TestMethod]
    [DataRow(1, "Basic", 3, true, "asi")]
    [DataRow(3, "Basic", 3, false, "")]
    [DataRow(8, "Basic", 3, false, "")]
    [DataRow(5,"\r\n", 2, false, "")]
    [DataRow(1,"\r\n", 2, false, "")]
    [DataRow(1,"\r\n", 1, true, "\n")]
    public void ConsumeThenLookAheadGivesSubStringOfInput(int consumeCount, string input, int lookAheadCount, bool expectedSuccess, string expectedResult)
    {
        // arrange
        var utf8Bytes = Encoding.UTF8.GetBytes(input);
        var stream = new MemoryStream(utf8Bytes);
        var consumer = new UTF8StreamConsumer(stream);
        
        // act
        consumer.ConsumeCodePoint(consumeCount);
        var (actualSuccess, actualResult) = consumer.LookAhead(lookAheadCount);
        
        // assert
        Assert.AreEqual(expectedSuccess, actualSuccess);
        if (expectedSuccess)
        {
            Assert.AreEqual(expectedResult, actualResult);
        }
    }
    
    
    [TestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    [DataRow(-2)]
    [DataRow(-10)]
    [DataRow(-100)]
    public void TryingToConsumeZeroOrNegativeCharsResultsInException(int charCount)
    {
        // arrange
        var utf8Bytes = Encoding.UTF8.GetBytes("Basic");
        var stream = new MemoryStream(utf8Bytes);
        var consumer = new UTF8StreamConsumer(stream);
        
        // act
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => consumer.ConsumeCodePoint(charCount));
    }
    
    [TestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    [DataRow(-2)]
    [DataRow(-10)]
    [DataRow(-100)]
    public void TryingToLookAheadZeroOrNegativeCharsResultsInException(int charCount)
    {
        // arrange
        var utf8Bytes = Encoding.UTF8.GetBytes("Basic");
        var stream = new MemoryStream(utf8Bytes);
        var consumer = new UTF8StreamConsumer(stream);
        
        // act
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => consumer.LookAhead(charCount));
    }

    [TestMethod]
    public void GivenEmptyInputFailToGetCurrentCharOrLookAhead()
    {
        // arrange
        var utf8Bytes = Array.Empty<byte>();
        var stream = new MemoryStream(utf8Bytes);
        var consumer = new UTF8StreamConsumer(stream);
        
        // act
        var (charSuccess, charResult) = consumer.TryGetCurrentCodePoint();
        var (stringSuccess, stringResult) = consumer.LookAhead(1);
        
        // assert
        Assert.IsFalse(charSuccess);
        Assert.IsFalse(stringSuccess);
        Assert.AreEqual(UnicodeCodePoint.ReplacementCharacter, charResult);
        Assert.AreEqual(string.Empty, stringResult);
    }
}