using ParsecCore.Indentation;
using ParsecCore;
using System.Linq;
using Xunit;

namespace ParsecCoreTests.Indentation
{
    public class Many1Tests
    {
        public static Parser<string, char> Word = Parsers.Letter.Many1();

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData("  \n ")]
        public void Many1RejectsNoItems(string inputStr)
        {
            var input = ParserInput.Create(inputStr);
            var parser = from list in ParsecCore.Indentation.Indentation.Many1(
                (IndentLevel)1, Relation.EQ, Parsers.Spaces, Word)
                         select list;
            var res = parser(input);

            Assert.True(res.IsError);
        }

        [Theory]
        [InlineData("word")]
        [InlineData("  \nword")]
        public void Many1AcceptsOneItem(string inputStr)
        {
            var input = ParserInput.Create(inputStr);
            var parser = from list in ParsecCore.Indentation.Indentation.Many1(
                (IndentLevel)1, Relation.EQ, Parsers.Spaces, Word)
                         select list;
            var res = parser(input);

            Assert.True(res.IsResult);
            Assert.True(res.Result.Count == 1);
            Assert.Equal("word", res.Result[0]);
        }

        [Fact]
        public void Many1AcceptsMultipleItems()
        {
            var input = ParserInput.Create("word\nword\n\nword");
            var parser = from list in ParsecCore.Indentation.Indentation.Many1(
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
        public void Many1Stops(string inputStr)
        {
            var input = ParserInput.Create(inputStr);
            var parser = from list in ParsecCore.Indentation.Indentation.Many1(
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
