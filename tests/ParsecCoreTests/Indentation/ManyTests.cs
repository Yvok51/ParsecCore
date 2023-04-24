using ParsecCore;
using ParsecCore.Indentation;
using Xunit;

namespace ParsecCoreTests.Indentation
{
    public class ManyTests
    {
        public static Parser<string, char> Word = Parsers.Letter.Many1();

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData("  \n ")]
        public void ManyAcceptsNoItems(string inputStr)
        {
            var input = ParserInput.Create(inputStr);
            var parser = from list in ParsecCore.Indentation.Indentation.Many(
                (IndentLevel)1, Relation.EQ, Parsers.Spaces, Word)
                         select list;
            var res = parser(input);

            Assert.True(res.IsResult);
            Assert.True(res.Result.Count == 0);
        }

        [Theory]
        [InlineData("word")]
        [InlineData("  \nword")]
        public void ManyAcceptsOneItem(string inputStr)
        {
            var input = ParserInput.Create(inputStr);
            var parser = from list in ParsecCore.Indentation.Indentation.Many(
                (IndentLevel)1, Relation.EQ, Parsers.Spaces, Word)
                         select list;
            var res = parser(input);

            Assert.True(res.IsResult);
            Assert.True(res.Result.Count == 1);
            Assert.Equal("word", res.Result[0]);
        }

        [Fact]
        public void ManyAcceptsMultipleItems()
        {
            var input = ParserInput.Create("word\nword\n\nword");
            var parser = from list in ParsecCore.Indentation.Indentation.Many(
                (IndentLevel)1, Relation.EQ, Parsers.Spaces, Word)
                         select list;
            var res = parser(input);

            Assert.True(res.IsResult);
            Assert.True(res.Result.Count == 3);
            foreach (var item in res.Result)
            {
                Assert.Equal("word", item);
            }
        }

        [Theory]
        [InlineData(" word\n word\n word\n 123")]
        [InlineData(" word\n word\n word\n  123")]
        [InlineData(" word\n word\n word\n123")]
        public void ManyStops(string inputStr)
        {
            var input = ParserInput.Create(inputStr);
            var parser = from list in ParsecCore.Indentation.Indentation.Many(
                (IndentLevel)2, Relation.EQ, Parsers.Spaces, Word)
                         select list;
            var res = parser(input);

            Assert.True(res.IsResult);
            Assert.True(res.Result.Count == 3);
            foreach (var item in res.Result)
            {
                Assert.Equal("word", item);
            }
        }
    }
}
