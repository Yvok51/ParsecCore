using ParsecCore;
using ParsecCore.Input;
using Xunit;

namespace ParsecCoreTests.Indentation
{
    public class NonIndentedTests
    {
        public static Parser<string, char> Word = Parsers.Letter.Many1();

        [Fact]
        public void NonIndentedParsed()
        {
            var input = ParserInput.Create("input");
            var parser = from word in ParsecCore.Indentation.Indentation.NonIndented(Parsers.Spaces, Word)
                         select word;
            var res = parser(input);

            Assert.True(res.IsResult);
        }

        [Fact]
        public void IndentedRejected()
        {
            var input = ParserInput.Create("  input");
            var parser = from word in ParsecCore.Indentation.Indentation.NonIndented(Parsers.Spaces, Word)
                         select word;
            var res = parser(input);

            Assert.True(res.IsError);
        }

        [Fact]
        public void NonIndentedOnThirdLineParsed()
        {
            var input = ParserInput.Create("\n\ninput");
            var parser = from word in ParsecCore.Indentation.Indentation.NonIndented(Parsers.Spaces, Word)
                         select word;
            var res = parser(input);

            Assert.True(res.IsResult);
        }

        [Fact]
        public void IndentedOnThirdLineRejected()
        {
            var input = ParserInput.Create("\n\n    input");
            var parser = from word in ParsecCore.Indentation.Indentation.NonIndented(Parsers.Spaces, Word)
                         select word;
            var res = parser(input);

            Assert.True(res.IsError);
        }
    }
}
