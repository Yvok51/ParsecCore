using Xunit;

using ParsecCore;
using ParsecCore.Input;

namespace ParsecCoreTests
{
    public class ChoiceParserTests
    {
        [Fact]
        public void FirstParserChosen()
        {
            IParser<string> firstParser = Parser.String("Hold");
            IParser<string> secondParser = Parser.String("Hold on");
            IParser<string> parser = Combinator.Choice(firstParser, secondParser);
            IParserInput input = ParserInput.Create("Hold on. This whole operation was your idea");

            var result = parser.Parse(input);

            Assert.True(result.HasRight);
            Assert.Equal("Hold", result.Right);
        }

        [Fact]
        public void FirstFailsSecondParserChosen()
        {
            IParser<string> firstParser = Parser.String("Impossible. Prehaps the archives are incomplete");
            IParser<string> secondParser = Parser.String("Your clones");
            IParser<string> parser = Combinator.Choice(firstParser, secondParser);
            IParserInput input = ParserInput.Create("Your clones are very impressive, you must be very proud.");

            var result = parser.Parse(input);

            Assert.True(result.HasRight);
            Assert.Equal("Your clones", result.Right);
        }

        [Fact]
        public void BothParsersFail()
        {
            IParser<string> firstParser = Parser.String("[Visible confusion]");
            IParser<string> secondParser = Parser.String("Yep");
            IParser<string> parser = Combinator.Choice(firstParser, secondParser);
            IParserInput input = ParserInput.Create("We will watch your career with great interest");

            var result = parser.Parse(input);

            Assert.True(result.HasLeft);
        }
    }
}
