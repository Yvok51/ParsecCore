using ParsecCore;
using ParsecCore.Permutations;
using System;
using System.IO;

namespace JSONtoXML
{
    class Program
    {
//        private static string jsonString =
//@"
//{
//    ""glossary"": {
//        ""title"": ""example glossary"",
//        ""GlossDiv"": {
//            ""title"": ""S"",
//            ""GlossList"": {
//                ""GlossEntry"": {
//                    ""ID"": 1526,
//                    ""SortAs"": ""SGML"",
//                    ""GlossTerm"": ""Standard Generalized Markup Language"",
//                    ""Acronym"": null,
//                    ""Abbrev"": ""ISO 8879:1986"",
//                    ""GlossDef"": {
//                        ""para"": ""A meta-markup language, used to create markup languages such as DocBook."",
//                        ""GlossSeeAlso"": [""GML"", ""XML""]
//                    },
//                    ""GlossSee"": true
//                }
//            }
//        }
//    }
//}
//";
        static void Main(string[] args)
        {
            var parser = CommandLineParser();
            var result = parser(ParserInput.Create(string.Join(' ', args)));

            if (result.IsError)
            {
                Console.WriteLine("Incorrect usage");
                PrintUsage();
                return;
            }

            var commands = result.Result;

            if (commands.help)
            {
                PrintUsage();
                return;
            }

            try
            {
                using (StreamReader input = new StreamReader(commands.input))
                using (TextWriter output = commands.output.Match(
                    just: outputFile => new StreamWriter(outputFile),
                    nothing: () => Console.Out
                ))
                {
                    ConvertToXML(input, output);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception:");
                Console.WriteLine(e);
            }
        }

        static void ConvertToXML(StreamReader input, TextWriter output)
        {
            var parserInput = ParserInput.Create(input);
            var result = JSONParsers.JsonDocument(parserInput);

            if (result.IsError)
            {
                Console.WriteLine("Error during parsing:");
                Console.WriteLine(result.Error);
                return;
            }

            var xml = ToXML.ConvertJSON(result.Result);
            PrintXML.Print(xml, output);
        }

        static Parser<(bool help, string input, Maybe<string> output), char> CommandLineParser()
        {
            var NonWhitespace = Parsers.Satisfy(c => !char.IsWhiteSpace(c), "not whitespace").Many1();
            Func<string, Parser<string, char>> argumentParser =
                (argument) => Parsers.Symbol("--" + argument).Try().Then(Parsers.Token(NonWhitespace));

            var perm = Permutation.NewPermutation(
                argumentParser("input")
            ).AddOptional(
                argumentParser("output").Map(arg => Maybe.FromValue(arg)),
                Maybe.Nothing<string>()
            ).AddOptional(
                Parsers.Token(
                    Parsers.Char('-').Then(
                        Parsers.String("h").Or(Parsers.String("-help"))
                    )
                ).MapConstant(true),
                false,
                (pair, help) => (help, pair.Item1, pair.Item2)
            );

            return perm.GetParser().FollowedBy(Parsers.EOF<char>());
        }


        public static void PrintUsage()
        {
            Console.WriteLine("JSONtoXML: JSONtoXML.exe --input {inputFile} [--output {outputFile}]");
            Console.WriteLine("    Convert a JSON file into the XML format");
            Console.WriteLine();
            Console.WriteLine("    Input file has to exist.");
            Console.WriteLine("    Output file will be overwritten or created.");
            Console.WriteLine("    If no output file is specified, prints to standard output");
        }

        public static void Test(string input)
        {
            try
            {
                // input = File.ReadAllText("../../../../../../benchmarks/JSONtoXML.Benchmarks/test-files/zips.json");
                var result = JSONParsers.JsonDocument(
                    ParserInput.Create(input)
                );

                Console.WriteLine(result);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception:");
                Console.WriteLine(e);
            }
        }
    }
}
