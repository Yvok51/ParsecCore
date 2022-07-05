﻿using ParsecCore.Input;
using Xunit;

namespace ParsecCoreTests.Input
{
    public class StringParserInputTests
    {
        [Fact]
        public void ReadsAllCharacters()
        {
            var input = "abcdefg";
            var expectedChars = input.ToCharArray();

            IParserInput reader = ParserInput.Create(input);

            int i = 0;
            while (!reader.EndOfInput)
            {
                Assert.Equal(expectedChars[i], reader.Read());
                i++;
            }
            Assert.Equal(expectedChars.Length, i);
        }

        [Fact]
        public void SeekIsWorking()
        {
            var input = "abcdefg";
            var expectedChars = new char[] { 'a', 'b', 'c', 'd', 'e', 'c', 'd', 'e', 'f', 'g' };

            IParserInput reader = ParserInput.Create(input);
            Position positionToSeek = reader.Position;

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
        public void SeekIsWorkingWithLinebreaks()
        {
            var input = "abcd\nefghi";
            var expectedChars = new char[] { 'a', 'b', 'c', 'd', '\n', 'e', 'f', 'c', 'd', '\n', 'e', 'f', 'g', 'h', 'i' };

            IParserInput reader = ParserInput.Create(input);
            Position positionToSeek = reader.Position;

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
