
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using CommandLine;
using System.Diagnostics;
using ParsecCore;
using ParsecCore.Input;
using ParsecCore.EitherNS;

namespace JSONtoXML.Benchmarks
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // var parser = JSONtoXML.JSONParsers.JsonDocument;

            IParserInput<char> source = null;

            try
            {
                using (StreamReader reader = new StreamReader(args[0]))
                {
                    source = ParserInput.Create(reader.ReadToEnd());
                }
            }
            catch
            {
            }

            var summary = BenchmarkRunner.Run<ParseJSON>();
        }

        public class ParseJSON
        {
            public ParseJSON(IParserInput<char> source, Parser<JsonValue, char> parser)
            {
                _parser = parser;
                _source = source;
            }

            [Benchmark]
            public IEither<ParseError, JsonValue> Parse() => _parser(_source);

            private readonly Parser<JsonValue, char> _parser;
            private readonly IParserInput<char> _source;

        }
    }
}