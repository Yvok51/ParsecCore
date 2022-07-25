using ParsecCore;
using ParsecCore.Input;
using System.Collections.Generic;
using Xunit;

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
            Parser<IReadOnlyList<string>> parser = Combinators.All(firstParser, secondParser, thirdParser);
            var input = ParserInput.Create("You underestimate my power!");

            var result = parser(input);

            Assert.True(result.IsResult);
            var resultList = new List<string>(result.Result);

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
            Parser<IReadOnlyList<string>> parser = Combinators.All(firstParser, secondParser, thirdParser);
            var input = ParserInput.Create("I love democracy");

            var result = parser(input);

            Assert.True(result.IsError);
        }
    }
}
