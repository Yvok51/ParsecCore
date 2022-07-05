using ParsecCore.Input;
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

        public (Position, int) ReadTill(IParserInput reader, char[] expectedChars, int startIndex, char seekBackTo, char stopAt)
        {
            Position positionToSeek = reader.Position;
            int i = startIndex;
            while (!reader.EndOfInput)
            {
                Position positionToBeRead = reader.Position;
                char c = reader.Read();
                if (c == seekBackTo)
                {
                    positionToSeek = positionToBeRead;
                }

                Assert.Equal(expectedChars[i], c);
                i++;
                if (c == stopAt)
                {
                    break;
                }
            }

            return (positionToSeek, i);
        }

        [Fact]
        public void ReadsAllCharacters()
        {
            string inputString = "abcdefg";
            using (var input = createStream(inputString))
            {
                var expectedChars = inputString.ToCharArray();

                IParserInput reader = ParserInput.Create(input, _encoding);

                int i = 0;
                while (!reader.EndOfInput)
                {
                    Assert.Equal(expectedChars[i], reader.Read());
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

                IParserInput reader = ParserInput.Create(input, _encoding);

                var (positionToSeek, i) = ReadTill(reader, expectedChars, 0, 'c', 'e');

                reader.Seek(positionToSeek);

                while (!reader.EndOfInput)
                {
                    Assert.Equal(expectedChars[i], reader.Read());
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

                IParserInput reader = ParserInput.Create(input, _encoding);

                var (positionToSeek, i) = ReadTill(reader, expectedChars, 0, 'c', 'f');

                reader.Seek(positionToSeek);

                while (!reader.EndOfInput)
                {
                    Assert.Equal(expectedChars[i], reader.Read());
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

                IParserInput reader = ParserInput.Create(input, _encoding);

                var (positionToSeek, i) = ReadTill(reader, expectedChars, 0, 'c', 'e');
                reader.Seek(positionToSeek);

                (positionToSeek, i) = ReadTill(reader, expectedChars, i, 'c', 'e');
                reader.Seek(positionToSeek);

                (positionToSeek, i) = ReadTill(reader, expectedChars, i, 'd', 'f');
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
}
