using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;
using ParsecCore;

namespace JSONtoXML.Benchmarks
{
    public class Program
    {
        public static readonly string FilePath = "test-files/";

        public static void Main(string[] args)
        {
            var summaryMicro = BenchmarkRunner.Run<ParseJSONMicro>();
            var summaryLarge = BenchmarkRunner.Run<ParseJSONMacro>();
            var summaryEmpty = BenchmarkRunner.Run<ParseJSONTest>();

            Console.WriteLine(summaryMicro);
            Console.WriteLine(summaryLarge);
            Console.WriteLine(summaryEmpty);
        }

        [SimpleJob(RunStrategy.Monitoring, launchCount: 5, warmupCount: 0, iterationCount: 20)]
        public class ParseJSONMacro
        {
            public IEnumerable<string> FileNames()
            {
                yield return "zips.json";
                yield return "large.json";
            }

            [Benchmark]
            [ArgumentsSource(nameof(FileNames))]
            public bool ParseMacro(string fileName)
                => JSONParsers.JsonDocument(ParserInput.Create(File.ReadAllText(FilePath + fileName))).IsResult;

        }

        public class ParseJSONMicro
        {
            public IEnumerable<string> FileNames()
            {
                yield return "countries.json";
                yield return "complex1.json";
                yield return "complex2.json";
                yield return "complex3.json";
                yield return "list.json";
                yield return "object.json";
            }

            [Benchmark]
            [ArgumentsSource(nameof(FileNames))]
            public bool ParseMacro(string fileName)
                => JSONParsers.JsonDocument(ParserInput.Create(File.ReadAllText(FilePath + fileName))).IsResult;
        }

        public class ParseJSONTest
        {
            public IEnumerable<string> FileNames()
            {
                yield return "empty.json";
            }

            [Benchmark]
            [ArgumentsSource(nameof(FileNames))]
            public bool ParseMacro(string fileName)
                => JSONParsers.JsonDocument(ParserInput.Create(File.ReadAllText(FilePath + fileName))).IsResult;
        }
    }
}