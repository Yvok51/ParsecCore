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
            IParser<string> firstParser = Parser.String("You underestimate");
            IParser<string> secondParser = Parser.String(" ");
            IParser<string> thirdParser = Parser.String("my power!");
            IParser<IEnumerable<string>> parser = Combinator.All(firstParser, secondParser, thirdParser);
            IParserInput input = ParserInput.Create("You underestimate my power!");

            var result = parser.Parse(input);

            Assert.True(result.HasRight);
            var resultList = new List<string>(result.Right);
            
            Assert.Equal("You underestimate", resultList[0]);
            Assert.Equal(" ", resultList[1]);
            Assert.Equal("my power!", resultList[2]);
        }

        [Fact]
        public void ParsingFailsIfAnyParserFails()
        {
            IParser<string> firstParser = Parser.String("I ");
            IParser<string> secondParser = Parser.String("love ");
            IParser<string> thirdParser = Parser.String("sand");
            IParser<IEnumerable<string>> parser = Combinator.All(firstParser, secondParser, thirdParser);
            IParserInput input = ParserInput.Create("I love democracy");

            var result = parser.Parse(input);

            Assert.True(result.HasLeft);
        }

        [Fact]
        public void PositionNotChangedWhenParsingFails()
        {
            IParser<string> firstParser = Parser.String("I ");
            IParser<string> secondParser = Parser.String("love ");
            IParser<string> thirdParser = Parser.String("sand");
            IParser<IEnumerable<string>> parser = Combinator.All(firstParser, secondParser, thirdParser);
            IParserInput input = ParserInput.Create("I love democracy");

            var _ = parser.Parse(input);

            Assert.Equal(0, input.Position.Offset);
        }
    }
}
