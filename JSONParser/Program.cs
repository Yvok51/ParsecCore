using ParsecCore.Input;
using System;
using System.IO;

namespace JSONtoXML
{
    class Program
    {
        private static string jsonString =
@"
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
";
        static void Main(string[] args)
        {
            if (args.Length == 1 && (args[0] == "-h" || args[0] == "--help"))
            {
                PrintUsage();
                return;
            }
            if (args.Length != 2)
            {
                Console.WriteLine("Not the right number of arguments");
                PrintUsage();
                return;
            }
            var inputFile = args[0];
            var outputFile = args[1];

            try
            {
                using (StreamReader input = new StreamReader(inputFile))
                using (StreamWriter output = new StreamWriter(outputFile))
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


        public static void PrintUsage()
        {
            Console.WriteLine("JSONtoXML usage:");
            Console.WriteLine("JSONtoXML.exe {inputFile} {outputFile}");
            Console.WriteLine("- Input file has to exist");
            Console.WriteLine("- Output file will be overwritten or created");
        }

        public static void Test()
        {
            try
            {
                JSONParsers.JsonDocument(
                    ParserInput.Create(File.ReadAllText("../../../../JSONtoXML.Benchmarks/test-files/zips.json"))
                );
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception:");
                Console.WriteLine(e);
            }
        }
    }
}
