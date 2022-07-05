using System;
using System.IO;
using System.Text;

using ParsecCore.Input;

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
            if (args.Length != 2)
            {
                Console.WriteLine("Not the right number of arguments");
                return;
            }
            var inputFile = args[0];
            var outputFile = args[1];

            using (StreamReader input = new StreamReader(inputFile))
            using (StreamWriter output = new StreamWriter(outputFile))
            {
                ConvertToXML(input, output);
            }
        }

        static void ConvertToXML(StreamReader input, StreamWriter output)
        {
            var parserInput = ParserInput.Create(input);
            var result = JSONParsers.JsonValue(parserInput);

            if (result.HasLeft)
            {
                Console.WriteLine("Error during parsing:");
                Console.WriteLine(result.Left);
                return;
            }

            var xml = ToXML.ConvertJSON(result.Right);
            PrintXML.Print(xml, output);
        }
    }
}
