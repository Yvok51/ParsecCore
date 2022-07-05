﻿using Xunit;

using ParsecCore;
using ParsecCore.Input;

namespace ParsecCoreTests
{
    public class ChoiceParserTests
    {
        [Fact]
        public void FirstParserChosen()
        {
            Parser<string> firstParser = Parsers.String("Hold");
            Parser<string> secondParser = Parsers.String("Hold on");
            Parser<string> parser = Combinators.Choice(firstParser, secondParser);
            IParserInput input = ParserInput.Create("Hold on. This whole operation was your idea");

            var result = parser(input);

            Assert.True(result.HasRight);
            Assert.Equal("Hold", result.Right);
        }

        [Fact]
        public void FirstFailsSecondParserChosen()
        {
            Parser<string> firstParser = Parsers.String("Impossible. Prehaps the archives are incomplete");
            Parser<string> secondParser = Parsers.String("Your clones");
            Parser<string> parser = Combinators.Choice(firstParser, secondParser);
            IParserInput input = ParserInput.Create("Your clones are very impressive, you must be very proud.");

            var result = parser(input);

            Assert.True(result.HasRight);
            Assert.Equal("Your clones", result.Right);
        }

        [Fact]
        public void BothParsersFail()
        {
            Parser<string> firstParser = Parsers.String("[Visible confusion]");
            Parser<string> secondParser = Parsers.String("Yep");
            Parser<string> parser = Combinators.Choice(firstParser, secondParser);
            IParserInput input = ParserInput.Create("We will watch your career with great interest");

            var result = parser(input);

            Assert.True(result.HasLeft);
        }
    }
}