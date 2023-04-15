using ParsecCore;
using System.IO;
using System.Text;
using Xunit;

namespace ParsecCoreTests.Input
{
    public class StreamParserInputTests
    {
        private static Encoding _encoding = Encoding.UTF8;

        private Stream createStream(string str)
        {
            Stream stream = new MemoryStream(_encoding.GetBytes(str));
            return stream;
        }

        public (IParserInput<char>, int) ReadTill(IParserInput<char> reader, char[] expectedChars, int startIndex, char seekBackTo, char stopAt)
        {
            var inputToSeekTo = reader;
            int i = startIndex;
            while (!reader.EndOfInput)
            {
                char c = reader.Current();
                if (c == seekBackTo)
                {
                    inputToSeekTo = reader;
                }
                Assert.Equal(expectedChars[i], c);
                i++;
                if (c == stopAt)
                {
                    break;
                }
                reader = reader.Advance();
            }

            return (inputToSeekTo, i);
        }

        [Fact]
        public void ReadsAllCharacters()
        {
            string inputString = "abcdefg";
            using (var input = createStream(inputString))
            {
                var expectedChars = inputString.ToCharArray();

                var reader = ParserInput.Create(input, _encoding);

                int i = 0;
                while (!reader.EndOfInput)
                {
                    Assert.Equal(expectedChars[i], reader.Current());
                    reader = reader.Advance();
                    i++;
                }
                Assert.Equal(expectedChars.Length, i);
            }
        }

        [Fact]
        public void SeekIsWorking()
        {
            using (var input = createStream("abcdefg"))
            {
                var expectedChars = new char[] { 'a', 'b', 'c', 'd', 'e', 'c', 'd', 'e', 'f', 'g' };

                var reader = ParserInput.Create(input, _encoding);

                var (newInput, i) = ReadTill(reader, expectedChars, 0, 'c', 'e');

                while (!newInput.EndOfInput)
                {
                    Assert.Equal(expectedChars[i], newInput.Current());
                    newInput = newInput.Advance();
                    i++;
                }
                Assert.Equal(expectedChars.Length, i);
            }
        }

        [Fact]
        public void SeekIsWorkingNonAscii()
        {
            using (var input = createStream("ábčďěfg"))
            {
                var expectedChars = new char[] { 'á', 'b', 'č', 'ď', 'ě', 'č', 'ď', 'ě', 'f', 'g' };

                var reader = ParserInput.Create(input, _encoding);

                var (newInput, i) = ReadTill(reader, expectedChars, 0, 'č', 'ě');

                while (!newInput.EndOfInput)
                {
                    Assert.Equal(expectedChars[i], newInput.Current());
                    newInput = newInput.Advance();
                    i++;
                }
                Assert.Equal(expectedChars.Length, i);
            }
        }

        [Fact]
        public void SeekIsWorkingHiragana()
        {
            using (var input = createStream("あいうえお"))
            {
                var expectedChars = new char[] { 'あ', 'い', 'う', 'え', 'い', 'う', 'え', 'お' };

                var reader = ParserInput.Create(input, _encoding);

                var (newInput, i) = ReadTill(reader, expectedChars, 0, 'い', 'え');

                while (!newInput.EndOfInput)
                {
                    Assert.Equal(expectedChars[i], newInput.Current());
                    newInput = newInput.Advance();
                    i++;
                }
                Assert.Equal(expectedChars.Length, i);
            }
        }

        [Fact]
        public void SeekIsWorkingWithLinebreaks()
        {
            using (var input = createStream("abcd\nefghi"))
            {
                var expectedChars = new char[] { 'a', 'b', 'c', 'd', '\n', 'e', 'f', 'c', 'd', '\n', 'e', 'f', 'g', 'h', 'i' };

                var reader = ParserInput.Create(input, _encoding);

                var (newInput, i) = ReadTill(reader, expectedChars, 0, 'c', 'f');

                while (!newInput.EndOfInput)
                {
                    Assert.Equal(expectedChars[i], newInput.Current());
                    newInput = newInput.Advance();
                    i++;
                }
                Assert.Equal(expectedChars.Length, i);
            }
        }

        [Fact]
        public void SeekMultipleTimes()
        {
            using (var input = createStream("abcdefg"))
            {
                var expectedChars = new char[] { 'a', 'b', 'c', 'd', 'e', 'c', 'd', 'e', 'c', 'd', 'e', 'f', 'd', 'e', 'f', 'g' };

                var reader = ParserInput.Create(input, _encoding);

                var (newInput, i) = ReadTill(reader, expectedChars, 0, 'c', 'e');

                (newInput, i) = ReadTill(newInput, expectedChars, i, 'c', 'e');

                (newInput, i) = ReadTill(newInput, expectedChars, i, 'd', 'f');

                while (!newInput.EndOfInput)
                {
                    Assert.Equal(expectedChars[i], newInput.Current());
                    newInput = newInput.Advance();
                    i++;
                }
                Assert.Equal(expectedChars.Length, i);
            }
        }
    }
}
