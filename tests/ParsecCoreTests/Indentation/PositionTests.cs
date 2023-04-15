using ParsecCore;
using ParsecCore.Indentation;
using Xunit;

namespace ParsecCoreTests.Indentation
{
    public class PositionTests
    {
        public static Parser<string, char> Word = Parsers.Letter.Many1();

        [Fact]
        public void BeginningPositionCorrect()
        {
            var input = ParserInput.Create("input");
            var parser = from pos in ParsecCore.Indentation.Indentation.IndentationLevel<char>()
                         from word in Word
                         select pos;
            var res = parser(input);

            Assert.True(res.IsResult);
            Assert.Equal(new IndentLevel(1), res.Result);
        }


        [Fact]
        public void EndingPositionCorrect()
        {
            var input = ParserInput.Create("input");
            var parser = from word in Word
                         from pos in ParsecCore.Indentation.Indentation.IndentationLevel<char>()
                         select pos;
            var res = parser(input);

            Assert.True(res.IsResult);
            Assert.Equal(new IndentLevel(6), res.Result);
        }

        [Fact]
        public void PositionOnSingleLineCorrect()
        {
            var input = ParserInput.Create("input   ");
            var parser = from word in Word
                         from pos in ParsecCore.Indentation.Indentation.IndentationLevel<char>()
                         select pos;
            var res = parser(input);

            Assert.True(res.IsResult);
            Assert.Equal(new IndentLevel(6), res.Result);
        }

        [Fact]
        public void PositionOnSecondLineCorrect()
        {
            var input = ParserInput.Create("input\nasd");
            var parser = from word in Word
                         from _ in Parsers.EOL
                         from pos in ParsecCore.Indentation.Indentation.IndentationLevel<char>()
                         select pos;
            var res = parser(input);

            Assert.True(res.IsResult);
            Assert.Equal(new IndentLevel(1), res.Result);
        }
    }
}
