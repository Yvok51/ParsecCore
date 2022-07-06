using JSONtoXML;
using ParsecCore.Input;
using System;
using System.Collections.Generic;
using Xunit;

namespace JSONtoXMLTests.Parsers
{
    public class ArrayValueTests
    {
        [Fact]
        public void EmptyArrayParsed()
        {
            var input = ParserInput.Create("[ ]");
            var result = JSONParsers.ArrayValue(input);

            Assert.True(result.IsResult);
            var expected = new ArrayValue(Array.Empty<JsonValue>());
            Assert.True(expected.MemberwiseEquals(result.Result));
        }

        [Fact]
        public void SingleValueArrayParsed()
        {
            var input = ParserInput.Create("[1]");
            var result = JSONParsers.ArrayValue(input);

            List<JsonValue> expectedArray = new List<JsonValue>();
            expectedArray.Add(new NumberValue(1));

            Assert.True(result.IsResult);
            var expected = new ArrayValue(expectedArray);
            Assert.True(expected.MemberwiseEquals(result.Result));
        }

        [Fact]
        public void MultipleValueArrayParsed()
        {
            var input = ParserInput.Create("[1, 2, 3]");
            var result = JSONParsers.ArrayValue(input);

            List<JsonValue> expectedArray = new List<JsonValue>();
            expectedArray.Add(new NumberValue(1));
            expectedArray.Add(new NumberValue(2));
            expectedArray.Add(new NumberValue(3));

            Assert.True(result.IsResult);
            var expected = new ArrayValue(expectedArray);
            Assert.True(expected.MemberwiseEquals(result.Result));
        }

        [Fact]
        public void MultipleValueTypesArrayParsed()
        {
            var input = ParserInput.Create("[1, \"abc\", null, true]");
            var result = JSONParsers.ArrayValue(input);

            List<JsonValue> expectedArray = new List<JsonValue>();
            expectedArray.Add(new NumberValue(1));
            expectedArray.Add(new StringValue("abc"));
            expectedArray.Add(new NullValue());
            expectedArray.Add(new BoolValue(true));

            Assert.True(result.IsResult);
            var expected = new ArrayValue(expectedArray);
            Assert.True(expected.MemberwiseEquals(result.Result));
        }

        [Fact]
        public void MissingBracketParseFails()
        {
            var input = ParserInput.Create("[1, \"abc\", null, true");
            var result = JSONParsers.ArrayValue(input);

            Assert.True(result.IsError);
        }

        [Fact]
        public void ExtraCommaParseFails()
        {
            var input = ParserInput.Create("[1, \"abc\", null, true,]");
            var result = JSONParsers.ArrayValue(input);

            Assert.True(result.IsError);
        }
    }
}
