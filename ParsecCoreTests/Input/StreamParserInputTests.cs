using System.IO;
using System.Text;

using Xunit;
using ParsecCore.Input;

namespace ParsecCoreTests.Input
{
    public class StreamParserInputTests
    {
        private static Encoding _encoding = Encoding.UTF8;

        private Stream createStream(string str)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream, _encoding);
            writer.Write(str);
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
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
        }

        [Fact]
        public void SeekIsWorkingWithLinebreaks()
        {
            using (var input = createStream("abcd\nefghi"))
            {
                var expectedChars = new char[] { 'a', 'b', 'c', 'd', '\n', 'e', 'f', 'c', 'd', '\n', 'e', 'f', 'g', 'h', 'i' };

                IParserInput reader = ParserInput.Create(input, _encoding);
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
}
