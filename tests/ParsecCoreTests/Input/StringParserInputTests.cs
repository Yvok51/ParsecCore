using ParsecCore;
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

            var reader = ParserInput.Create(input);

            int i = 0;
            while (!reader.EndOfInput)
            {
                Assert.Equal(expectedChars[i], reader.Current());
                reader = reader.Advance();
                i++;
            }
            Assert.Equal(expectedChars.Length, i);
        }

        [Fact]
        public void SeekIsWorking()
        {
            var input = "abcdefg";
            var expectedChars = new char[] { 'a', 'b', 'c', 'd', 'e', 'c', 'd', 'e', 'f', 'g' };

            var reader = ParserInput.Create(input);
            var inputToSeekTo = reader;

            int i = 0;
            while (!reader.EndOfInput)
            {
                char c = reader.Current();
                if (c == 'c')
                {
                    inputToSeekTo = reader;
                }
                Assert.Equal(expectedChars[i], c);
                i++;
                if (c == 'e')
                {
                    break;
                }
                reader = reader.Advance();
            }

            while (!inputToSeekTo.EndOfInput)
            {
                Assert.Equal(expectedChars[i], inputToSeekTo.Current());
                inputToSeekTo = inputToSeekTo.Advance();
                i++;
            }
            Assert.Equal(expectedChars.Length, i);
        }

        [Fact]
        public void SeekIsWorkingWithLinebreaks()
        {
            var input = "abcd\nefghi";
            var expectedChars = new char[] { 'a', 'b', 'c', 'd', '\n', 'e', 'f', 'c', 'd', '\n', 'e', 'f', 'g', 'h', 'i' };

            var reader = ParserInput.Create(input);
            var inputToSeekTo = reader;

            int i = 0;
            while (!reader.EndOfInput)
            {
                char c = reader.Current();
                if (c == 'c')
                {
                    inputToSeekTo = reader;
                }
                Assert.Equal(expectedChars[i], c);
                i++;
                if (c == 'f')
                {
                    break;
                }
                reader = reader.Advance();
            }

            while (!inputToSeekTo.EndOfInput)
            {
                Assert.Equal(expectedChars[i], inputToSeekTo.Current());
                inputToSeekTo = inputToSeekTo.Advance();
                i++;
            }
            Assert.Equal(expectedChars.Length, i);
        }
    }
}
