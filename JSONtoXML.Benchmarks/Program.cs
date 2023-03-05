
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Diagnostics;
using ParsecCore.Input;
using BenchmarkDotNet.Engines;

namespace JSONtoXML.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var summaryMacro = BenchmarkRunner.Run<ParseJSONMacro>();
            var summaryMicro = BenchmarkRunner.Run<ParseJSONMicro>();

            Console.WriteLine(summaryMicro);
            Console.WriteLine(summaryMacro);
        }

        [SimpleJob(RunStrategy.Monitoring, launchCount: 20, warmupCount: 0, iterationCount: 20)]
        public class ParseJSONMacro
        {
            public ParseJSONMacro()
            {
                Files = LoadFiles(FileNames());
            }

            public IEnumerable<string> FileNames()
            {
                yield return "test-files/countries.json";
                yield return "test-files/zips.json";
                yield return "test-files/large.json";
            }

            public Dictionary<string, string> Files;

            [Benchmark]
            [ArgumentsSource(nameof(FileNames))]
            public bool ParseMacro(string fileName) => JSONParsers.JsonDocument(ParserInput.Create(File.ReadAllText(fileName))).IsResult;

            public static Dictionary<string, string> LoadFiles(IEnumerable<string> files)
            {
                var fileContent = new Dictionary<string, string>();
                foreach (var file in files)
                {
                    try
                    {
                        using (StreamReader reader = new StreamReader(file))
                        {
                            fileContent.Add(file, reader.ReadToEnd());
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }

                return fileContent;
            }
        }

        public class ParseJSONMicro
        {
            [Benchmark]
            [Arguments(@"
{
    ""glossary"": {
        ""title"": ""example glossary"",
        ""GlossDiv"": {
            ""title"": ""S"",
            ""GlossList"": {
                ""GlossEntry"": {
                    ""ID"": 1526,
                    ""SortAs"": ""SGML"",
                    ""GlossTerm"": ""Standard Generalized Markup Language"",
                    ""Acronym"": null,
                    ""Abbrev"": ""ISO 8879:1986"",
                    ""GlossDef"": {
                        ""para"": ""A meta-markup language, used to create markup languages such as DocBook."",
                        ""GlossSeeAlso"": [""GML"", ""XML""]
                    },
                    ""GlossSee"": true
                }
            }
        }
    }
}
")]
            [Arguments(@"
{""widget"": {
    ""debug"": ""on"",
    ""window"": {
        ""title"": ""Sample Konfabulator Widget"",
        ""name"": ""main_window"",
        ""width"": 500,
        ""height"": 500
    },
    ""image"": { 
        ""src"": ""Images/Sun.png"",
        ""name"": ""sun1"",
        ""hOffset"": 250,
        ""vOffset"": 250,
        ""alignment"": ""center""
    },
    ""text"": {
        ""data"": ""Click Here"",
        ""size"": 36,
        ""style"": ""bold"",
        ""name"": ""text1"",
        ""hOffset"": 250,
        ""vOffset"": 100,
        ""alignment"": ""center"",
        ""onMouseUp"": ""sun1.opacity = (sun1.opacity / 100) * 90;""
    }
}}")]
            [Arguments(@"
{""menu"": {
    ""id"": ""file"",
    ""value"": ""File"",
    ""popup"": {
        ""menuitem"": [
            {""value"": ""New"", ""onclick"": ""CreateNewDoc()""},
            {""value"": ""Open"", ""onclick"": ""OpenDoc()""},
            {""value"": ""Close"", ""onclick"": ""CloseDoc()""}
        ]
    }
}}
")]
            [Arguments("[\"Simple JSON list\", 1, 2, 3, 4, 5, \"a\", \"b\", \"c\", \"d\", \"e\"]")]
            [Arguments("{\"key0\": \"Simple JSON object\", \"key1\": 1, \"key2\": 2, \"key3\": 3, " +
                "\"key4\": \"a\", \"key5\": \"b\", \"key6\": \"c\"}")]
            public bool ParseMicro(string input) => JSONParsers.JsonDocument(ParserInput.Create(input)).IsResult;
        }
    }
}