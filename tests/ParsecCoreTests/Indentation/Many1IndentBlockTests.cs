using ParsecCore;
using System.Linq;
using Xunit;

namespace ParsecCoreTests.Indentation
{
    public class Many1IndentBlockTests
    {
        public static Parser<string, char> Word = Parsers.Letter.Many1();

        [Theory]
        [InlineData("def")]
        [InlineData("def   ")]
        [InlineData("def \t  ")]
        [InlineData("def\n  \n  ")]
        public void Many1IndentBlockRejectsZeroItems(string inputStr)
        {
            var input = ParserInput.Create(inputStr);
            var parser = from list in ParsecCore.Indentation.Indentation.BlockMany1(
                Parsers.Spaces, Parsers.String("def"), Word, (head, list) => list)
                         select list;
            var res = parser(input);

            Assert.True(res.IsError);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\n  hey\n  hello\n  hi")]
        public void Many1IndentBlockRejectsNoHead(string inputStr)
        {
            var input = ParserInput.Create(inputStr);
            var parser = from list in ParsecCore.Indentation.Indentation.BlockMany1(
                Parsers.Spaces, Parsers.String("def"), Word, (head, list) => list)
                         select list;
            var res = parser(input);

            Assert.True(res.IsError);
        }

        [Theory]
        [InlineData("def\n hey\n hello\n hi", "hey hello hi")]
        [InlineData("def\n  hey\n  hello\n  hi", "hey hello hi")]
        [InlineData(" def\n  hey\n  hello\n  hi", "hey hello hi")]
        [InlineData("def\n\they\n\thello\n\thi", "hey hello hi")]
        public void Many1IndentBlockParsesSameIndentedItems(string inputStr, string parsedItems)
        {
            var items = parsedItems.Split(' ');
            var input = ParserInput.Create(inputStr);
            var parser = from list in ParsecCore.Indentation.Indentation.BlockMany1(
                Parsers.Spaces, Parsers.String("def"), Word, (head, list) => list)
                         select list;
            var res = parser(input);

            Assert.True(res.IsResult);
            Assert.True(Enumerable.SequenceEqual(items, res.Result));
        }

        [Theory]
        [InlineData("def\n hey\n hello\n  hi")]
        [InlineData("def\n  hey\n  hello\n hi")]
        [InlineData("def\n\they\n\thello\n\t hi")]
        public void Many1IndentBlockRejectsDifferentlyIndentedItems(string inputStr)
        {
            var input = ParserInput.Create(inputStr);
            var parser = from list in ParsecCore.Indentation.Indentation.BlockMany1(
                Parsers.Spaces, Parsers.String("def"), Word, (head, list) => list)
                         select list;
            var res = parser(input);

            Assert.True(res.IsError);
        }
    }
}
