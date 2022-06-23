using System;
using Xunit;

using ParsecCore.Input;

namespace ParsecCoreTests.Input
{
    public class StringParserInputTests
    {
        [Fact]
        public void StringParserInputReadsAllCarachters()
        {
            var input = "abcdefg";
            var expectedChars = input.ToCharArray();

            IParserInput reader = ParserInput.Create(input);

            int i = 0;
            while(!reader.EndOfInput)
            {
                Assert.Equal(expectedChars[i], reader.Read());
                i++;
            }
            Assert.Equal(expectedChars.Length, i);
        }

        [Fact]
        public void StringParserSeekIsWorking()
        {
            var input = "abcdefg";
            var expectedChars = new char[] { 'a', 'b', 'c', 'd', 'e', 'c', 'd', 'e', 'f', 'g' };

            IParserInput reader = ParserInput.Create(input);
            Position positionToSeek = Position.Start;

            int i = 0;
            while (!reader.EndOfInput)
            {
                Position positionToBeRead = reader.Position;
                char c = reader.Read();
                if (c == 'c')
                {
                    positionToSeek = positionToBeRead;
                }

                Assert.Equal(expectedChars[i], c);
                i++;
                if (c == 'e')
                {
                    break;
                }
            }

            reader.Seek(positionToSeek);

            while (!reader.EndOfInput)
            {
                Assert.Equal(expectedChars[i], reader.Read());
                i++;
            }
            Assert.Equal(expectedChars.Length, i);
        }

        [Fact]
        public void StringParserSeekIsWorkingWithLinebreaks()
        {
            var input = "abcd\nefghi";
            var expectedChars = new char[] { 'a', 'b', 'c', 'd', '\n', 'e', 'f', 'c', 'd', '\n', 'e', 'f', 'g', 'h', 'i' };

            IParserInput reader = ParserInput.Create(input);
            Position positionToSeek = Position.Start;

            int i = 0;
            while (!reader.EndOfInput)
            {
                Position positionToBeRead = reader.Position;
                char c = reader.Read();
                if (c == 'c')
                {
                    positionToSeek = positionToBeRead;
                }

                Assert.Equal(expectedChars[i], c);
                i++;
                if (c == 'f')
                {
                    break;
                }
            }

            reader.Seek(positionToSeek);

            while (!reader.EndOfInput)
            {
                Assert.Equal(expectedChars[i], reader.Read());
                i++;
            }
            Assert.Equal(expectedChars.Length, i);
        }
    }
}
