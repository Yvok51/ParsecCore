using ParsecCore;
using ParsecCore.Indentation;
using System.Linq;
using Xunit;

namespace ParsecCoreTests.Indentation
{
    public class OptionalTests
    {
        public static Parser<string, char> Word = Parsers.Letter.Many1();

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        public void OptionalAcceptsFailure(string inputStr)
        {
            var input = ParserInput.Create(inputStr);
            var parser = ParsecCore.Indentation.Indentation.Optional((IndentLevel)1, Relation.EQ, Word);
            var res = parser(input);

            Assert.True(res.IsResult);
            Assert.True(res.Result.IsEmpty);
        }

        [Fact]
        public void OptionalAcceptsItem()
        {
            var input = ParserInput.Create("word");
            var parser = ParsecCore.Indentation.Indentation.Optional((IndentLevel)1, Relation.EQ, Word);
            var res = parser(input);

            Assert.True(res.IsResult);
            Assert.True(res.Result.HasValue);
            Assert.Equal("word", res.Result.Value);
        }

        [Fact]
        public void OptionalRejectsPartialParse()
        {
            var input = ParserInput.Create("word");
            var parser = ParsecCore.Indentation.Indentation.Optional((IndentLevel)1, Relation.EQ, Parsers.String("woman"));
            var res = parser(input);

            Assert.True(res.IsError);
            Assert.Equal(new Position(1, 3), res.Error.Position);
        }

        [Fact]
        public void OptionalRejectsWrongIndentation()
        {
            var input = ParserInput.Create("  word");
            var parser = Parsers.Spaces
                .Then(ParsecCore.Indentation.Indentation.Optional((IndentLevel)1, Relation.EQ, Word));
            var res = parser(input);

            Assert.True(res.IsError);
            Assert.Equal(new Position(1, 3), res.Error.Position);
            Assert.IsType<CustomError>(res.Error);
            Assert.True(((CustomError)res.Error).Customs.ToList().Count == 1);
            Assert.IsType<IndentationError>(((CustomError)res.Error).Customs.ToList()[0]);
        }
    }
}
