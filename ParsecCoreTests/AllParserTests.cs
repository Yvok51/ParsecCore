using System.Collections.Generic;

using Xunit;

using ParsecCore;
using ParsecCore.Input;

namespace ParsecCoreTests
{
    public class AllParserTests
    {
        [Fact]
        public void AllParsersApplied()
        {
            Parser<string> firstParser = Parsers.String("You underestimate");
            Parser<string> secondParser = Parsers.String(" ");
            Parser<string> thirdParser = Parsers.String("my power!");
            Parser<IEnumerable<string>> parser = Combinators.All(firstParser, secondParser, thirdParser);
            IParserInput input = ParserInput.Create("You underestimate my power!");

            var result = parser(input);

            Assert.True(result.HasRight);
            var resultList = new List<string>(result.Right);
            
            Assert.Equal("You underestimate", resultList[0]);
            Assert.Equal(" ", resultList[1]);
            Assert.Equal("my power!", resultList[2]);
        }

        [Fact]
        public void ParsingFailsIfAnyParserFails()
        {
            Parser<string> firstParser = Parsers.String("I ");
            Parser<string> secondParser = Parsers.String("love ");
            Parser<string> thirdParser = Parsers.String("sand");
            Parser<IEnumerable<string>> parser = Combinators.All(firstParser, secondParser, thirdParser);
            IParserInput input = ParserInput.Create("I love democracy");

            var result = parser(input);

            Assert.True(result.HasLeft);
        }

        [Fact]
        public void PositionNotChangedWhenParsingFails()
        {
            Parser<string> firstParser = Parsers.String("I ");
            Parser<string> secondParser = Parsers.String("love ");
            Parser<string> thirdParser = Parsers.String("sand");
            Parser<IEnumerable<string>> parser = Combinators.All(firstParser, secondParser, thirdParser);
            IParserInput input = ParserInput.Create("I love democracy");

            var _ = parser(input);

            Assert.Equal(0, input.Position.Offset);
        }
    }
}
