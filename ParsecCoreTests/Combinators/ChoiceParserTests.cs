using ParsecCore;
using ParsecCore.Input;
using Xunit;

namespace ParsecCoreTests
{
    public class ChoiceParserTests
    {
        [Fact]
        public void FirstParserChosen()
        {
            var firstParser = Parsers.String("Hold");
            var secondParser = Parsers.String("Hold on");
            var parser = Combinators.Choice(firstParser, secondParser);
            var input = ParserInput.Create("Hold on. This whole operation was your idea");

            var result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal("Hold", result.Result);
        }

        [Fact]
        public void FirstFailsSecondParserChosen()
        {
            var firstParser = Parsers.String("Impossible. Prehaps the archives are incomplete");
            var secondParser = Parsers.String("Your clones");
            var parser = Combinators.Choice(firstParser, secondParser);
            var input = ParserInput.Create("Your clones are very impressive, you must be very proud.");

            var result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal("Your clones", result.Result);
        }

        [Fact]
        public void BothParsersFail()
        {
            var firstParser = Parsers.String("[Visible confusion]");
            var secondParser = Parsers.String("Yep");
            var parser = Combinators.Choice(firstParser, secondParser);
            var input = ParserInput.Create("We will watch your career with great interest");

            var result = parser(input);

            Assert.True(result.IsError);
        }
    }
}
