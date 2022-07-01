using System;
using System.Collections.Generic;

using Xunit;
using JSONtoXML;
using ParsecCore.Input;

namespace JSONtoXMLTests.Parsers
{
    public class ObjectValueTests
    {
        [Fact]
        public void EmptyObjectParsed()
        {
            var input = ParserInput.Create("{}");
            var result = JSONParsers.ObjectValue(input);

            Assert.True(result.HasRight);
            var expected = new ObjectValue(Array.Empty<ObjectKeyValuePair>());
            Assert.True(expected.MemberwiseEquals(result.Right));
        }

        [Fact]
        public void SingleValueObjectParsed()
        {
            var input = ParserInput.Create("{\"a\": 1}");
            var result = JSONParsers.ObjectValue(input);

            List<ObjectKeyValuePair> expectedValues = new List<ObjectKeyValuePair>();
            expectedValues.Add(new ObjectKeyValuePair() { Key = new StringValue("a"), Value = new NumberValue(1)});

            Assert.True(result.HasRight);
            var expected = new ObjectValue(expectedValues);
            Assert.True(expected.MemberwiseEquals(result.Right));
        }

        [Fact]
        public void MultipleValueObjectParsed()
        {
            var input = ParserInput.Create("{\"a\": 1, \"b\": 2, \"c\": 3}");
            var result = JSONParsers.ObjectValue(input);

            List<ObjectKeyValuePair> expectedValues = new List<ObjectKeyValuePair>();
            expectedValues.Add(new ObjectKeyValuePair() { Key = new StringValue("a"), Value = new NumberValue(1) });
            expectedValues.Add(new ObjectKeyValuePair() { Key = new StringValue("b"), Value = new NumberValue(2) });
            expectedValues.Add(new ObjectKeyValuePair() { Key = new StringValue("c"), Value = new NumberValue(3) });

            Assert.True(result.HasRight);
            var expected = new ObjectValue(expectedValues);
            Assert.True(expected.MemberwiseEquals(result.Right));
        }

        [Fact]
        public void MultipleValueTypesArrayParsed()
        {
            var input = ParserInput.Create("{\"a\": 1, \"b\": \"abc\", \"c\": null, \"d\": true}");
            var result = JSONParsers.ObjectValue(input);

            List<ObjectKeyValuePair> expectedValues = new List<ObjectKeyValuePair>();
            expectedValues.Add(new ObjectKeyValuePair() { Key = new StringValue("a"), Value = new NumberValue(1) });
            expectedValues.Add(new ObjectKeyValuePair() { Key = new StringValue("b"), Value = new StringValue("abc") });
            expectedValues.Add(new ObjectKeyValuePair() { Key = new StringValue("c"), Value = new NullValue() });
            expectedValues.Add(new ObjectKeyValuePair() { Key = new StringValue("d"), Value = new BoolValue(true) });

            Assert.True(result.HasRight);
            var expected = new ObjectValue(expectedValues);
            Assert.True(expected.MemberwiseEquals(result.Right));
        }

        [Fact]
        public void MissingBracketParseFails()
        {
            var input = ParserInput.Create("{\"a\": 1, \"b\": \"abc\", \"c\": null, \"d\": true");
            var result = JSONParsers.ObjectValue(input);

            Assert.True(result.HasLeft);
        }

        [Fact]
        public void ExtraCommaParseFails()
        {
            var input = ParserInput.Create("{\"a\": 1, \"b\": \"abc\", \"c\": null, \"d\": true, }");
            var result = JSONParsers.ObjectValue(input);

            Assert.True(result.HasLeft);
        }

        [Fact]
        public void MissingValueParseFails()
        {
            var input = ParserInput.Create("{\"a\": 1, \"b\": \"abc\", \"c\": , \"d\": true }");
            var result = JSONParsers.ObjectValue(input);

            Assert.True(result.HasLeft);
        }

        [Fact]
        public void MissingKeyParseFails()
        {
            var input = ParserInput.Create("{\"a\": 1, : \"abc\", \"c\": null, \"d\": true }");
            var result = JSONParsers.ObjectValue(input);

            Assert.True(result.HasLeft);
        }

        [Fact]
        public void MissingColonParseFails()
        {
            var input = ParserInput.Create("{\"a\" 1, \"b\": \"abc\", \"c\": null, \"d\": true}");
            var result = JSONParsers.ObjectValue(input);

            Assert.True(result.HasLeft);
        }
    }
}
