﻿using JSONtoXML;
using ParsecCore;
using Xunit;

namespace JSONtoXMLTests.Parsers
{
    public class NullValueTests
    {
        [Fact]
        public void NullValueParsed()
        {
            var input = ParserInput.Create("null");
            var result = JSONParsers.NullValue(input);

            Assert.True(result.IsResult);
            Assert.Equal(new NullValue(), result.Result);
        }

        [Fact]
        public void NullValueFails()
        {
            var input = ParserInput.Create("nuxll");
            var result = JSONParsers.NullValue(input);

            Assert.True(result.IsError);
        }
    }
}
