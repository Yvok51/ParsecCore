using ParsecCore;
using PythonParser.Parser;

namespace PythonParserTests.Parser.LiteralTests
{
    public class IntegerTests
    {
        [Theory]
        [InlineData("0", 0)]
        [InlineData("10", 10)]
        [InlineData("0xA", 10)]
        [InlineData("0o11", 9)]
        [InlineData("0b1010", 10)]
        public void ParsedCorrectly(string inputString, int expected)
        {
            var input = ParserInput.Create(inputString);
            var result = Literals.Integer(input);

            Assert.True(result.IsResult);
            Assert.Equal(expected, result.Result.Value);
        }

        [Theory]
        [InlineData("01")]
        [InlineData("0b120")]
        [InlineData("0o9")]
        [InlineData("0x2G")]
        public void ParsingFails(string inputString)
        {
            var input = ParserInput.Create(inputString);
            var result = Literals.Integer(input);

            Assert.True(result.IsError);
        }
    }
}
