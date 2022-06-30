using System;

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
            var input = ParserInput.Create(jsonString);
            var result = JSONParsers.JsonValue(input);

            Console.WriteLine(result);
        }
    }
}
