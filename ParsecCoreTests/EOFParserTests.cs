using Xunit;

using ParsecCore;
using ParsecCore.Input;
using ParsecCore.EitherNS;
using ParsecCore.Help;

namespace ParsecCoreTests
{
    public class EOFParserTests
    {
        [Fact]
        public void ParsesCorrectly()
        {
            IParserInput input = ParserInput.Create("");
            IParser<None> eof = Parser.EOF;

            var result = eof.Parse(input);

            Assert.True(result.HasRight);
            Assert.True(input.EndOfInput);
        }

        [Fact]
        public void FailsCorrectly ()
        {
            IParserInput input = ParserInput.Create("This is outrageous. It's unfair");
            IParser<None> eof = Parser.EOF;

            var result = eof.Parse(input);

            Assert.True(result.HasLeft);
            Assert.False(input.EndOfInput);
        }
    }
}
