using System.Text;

namespace PermuteGenerator
{
    internal static class PermuteSourceGenerator
    {
        public static readonly int PREMADE_PERMUTE_FUNCTIONS = 3;
        public static readonly int PREMADE_PARTIAL_FUNCTIONS = 4;

        public static string GeneratePermuteSourceDocument(int from, int upTo)
        {
            if (from > upTo)
            {
                return string.Empty;
            }

            string header = @"
using System;
using System.Collections.Generic;

namespace ParsecCore.Permutations
{
    public static partial class Permutation
    {
";
            string footer = @"
    }
}";
            StringBuilder builder = new StringBuilder(header);
            for (int numOfParsers = from; numOfParsers <= upTo; numOfParsers++)
            {
                builder.Append(PermuteMethodGenerator.GeneratePermuteMethod(numOfParsers));
            }
            builder.AppendLine(footer);
            return builder.ToString();
        }

        public static string GeneratePartialSourceDocument(int from, int upTo)
        {
            string header = @"
using System;

namespace ParsecCore.Permutations
{
    internal static partial class PartialExt
    {
";
            string footer = @"
    }
}";
            StringBuilder builder = new StringBuilder(header);
            for (int numberOfTypes = from; numberOfTypes <= upTo; numberOfTypes++)
            {
                builder.Append(PartialFuncExtensionGenerator.GeneratePartialMethods(numberOfTypes));
            }
            builder.AppendLine(footer);
            return builder.ToString();
        }

        private class PermuteMethodGenerator
        {
            /// <summary>
            /// Generates a Permute method for the given number of parsers
            /// </summary>
            /// <param name="numberOfParsers"> Number of parsers to use in the Permute method </param>
            /// <returns> Source code of a Permute method</returns>
            public static string GeneratePermuteMethod(int numberOfParsers)
            {
                var types = Help.GetTypesList(numberOfParsers);
                string typeList = string.Join(", ", types);
                string tabs = "\t\t";
                // string functionName = "f";

                string signature = tabs + $"public static Parser<TR, TParserInput> Permute<{typeList}, TParserInput, TR>(\n";
                var parsers = GetVariableList(types);
                string argumentList = GetParserArguments(parsers, typeList, tabs);
                string methodBody = GetPermuteBody(parsers, tabs);

                return signature + argumentList + methodBody;
            }

            /// <summary>
            /// Generates the body of a permute function
            /// </summary>
            /// <param name="parsers"> Parsers which we permute </param>
            /// <param name="baseTabs"> The base number of tabs to use </param>
            /// <returns> Body of a Permute function </returns>
            private static string GetPermuteBody(IReadOnlyList<Variable> parsers, string baseTabs)
            {
                StringBuilder body = new StringBuilder();
                string tabs = baseTabs + "\t";
                string indentedTabs = tabs + "\t";

                body.AppendLine(baseTabs + "{");
                body.AppendLine(tabs + "bool allParsersOptional = " + AllParsersIsOptional(parsers) + ";");
                body.AppendLine(tabs + $"int branchCount = allParsersOptional ? {parsers.Count + 1} : {parsers.Count};");

                body.AppendLine(tabs + "List<Parser<TR, TParserInput>> branches = new(branchCount)");
                body.AppendLine(tabs + "{");
                foreach (var branch in GetAllBranches(parsers))
                {
                    foreach (var line in branch)
                    {
                        body.AppendLine(indentedTabs + line);
                    }
                }
                body.AppendLine(tabs + "};");

                body.AppendLine(tabs + "if (allParsersOptional) {");
                body.AppendLine(indentedTabs + $"branches.Add(Parsers.Return<TR, TParserInput>(f({AllParsersDefaultValue(parsers)})));");
                body.AppendLine(tabs + "}");

                body.AppendLine(tabs + "return Combinators.Choice(branches);");
                body.AppendLine(baseTabs + "}");

                return body.ToString();
            }

            /// <summary>
            /// Gets the parser arguments for the permute method
            /// </summary>
            /// <param name="parsers"> The list of parsers which should appear in the arguments </param>
            /// <param name="parserTypesList"> Comma seperated list of the different types </param>
            /// <returns> Parser arguments for the permute method </returns>
            private static string GetParserArguments(IReadOnlyList<Variable> parsers, string parserTypesList, string baseTabs)
            {
                StringBuilder argumentList = new StringBuilder();
                string tabs = baseTabs + "\t";

                var getParserArgument = (Variable parser) => tabs + $"{parser.Type} {parser.Name},";
                foreach (var parser in parsers)
                {
                    argumentList.AppendLine(getParserArgument(parser));
                }

                argumentList.AppendLine(tabs + $"Func<{parserTypesList}, TR> f");
                argumentList.AppendLine(baseTabs + ")");

                return argumentList.ToString();
            }

            /// <summary>
            /// Generate all branches needed
            /// </summary>
            /// <param name="parsers"> Parsers from which to generate the branches </param>
            /// <returns> All branches to be made from given parsers </returns>
            private static IReadOnlyList<IReadOnlyList<string>> GetAllBranches(IReadOnlyList<Variable> parsers)
            {
                static string AllParsersExceptOne(int indexToLeaveOut, IReadOnlyList<Variable> parsers)
                {
                    StringBuilder builder = new();
                    for (int i = 0; i < parsers.Count; i++)
                    {
                        if (i != indexToLeaveOut)
                        {
                            builder.Append(parsers[i].Name + ", ");
                        }
                    }
                    return builder.ToString();
                }
                List<List<string>> branches = new(parsers.Count);
                for (int i = 0; i < parsers.Count; i++)
                {
                    branches.Add(new List<string>() {
                        $"from t in {parsers[i].Name}.Parser",
                        $"from r in Permute({AllParsersExceptOne(i, parsers)}f.Partial(t))",
                        "select r,"
                    });
                }

                return branches;
            }

            /// <summary>
            /// Generate the bool condition of whether all parses are optional
            /// </summary>
            /// <param name="parsers"> The parsers to query </param>
            /// <returns> Bool expression which answers whether all parsers are optional </returns>
            private static string AllParsersIsOptional(IReadOnlyList<Variable> parsers)
            {
                return Help.Join(parsers, (parser) => $"{parser.Name}.IsOptional", " && ");
            }

            /// <summary>
            /// Generate the expression of a list of default values of the parsers
            /// </summary>
            /// <param name="parsers"> Parsers whose default value to ask for </param>
            /// <returns> expression representing a list of default value of given parsers </returns>
            private static string AllParsersDefaultValue(IReadOnlyList<Variable> parsers)
            {
                return Help.Join(parsers, (parser) => $"{parser.Name}.DefaultValue", ", ");
            }

            /// <summary>
            /// Generate the list of parsers from a list of their return types
            /// </summary>
            /// <param name="types"> List of the parsers' return types </param>
            /// <returns> List of parsers, one from each type supplied </returns>
            private static IReadOnlyList<Variable> GetVariableList(IReadOnlyList<string> types)
            {
                List<Variable> list = new List<Variable>();
                var getParserVariable = (string type) => new Variable($"SplitParser<{type}, TParserInput>", $"splitParser{type}");
                foreach (var type in types)
                {
                    list.Add(getParserVariable(type));
                }
                return list;
            }
        }

        private class PartialFuncExtensionGenerator
        {
            /// <summary>
            /// Generate partial function application extension methods
            /// </summary>
            /// <param name="numberOfInputTypes"> Number of input types in the input function </param>
            /// <returns> Partial application function methods for all input variables </returns>
            public static string GeneratePartialMethods(int numberOfInputTypes)
            {
                if (numberOfInputTypes < 1)
                {
                    return string.Empty;
                }

                var inputTypes = Enumerable.Range(0, numberOfInputTypes).Select(i => $"T{i + 1}");
                var variables = inputTypes.Select((string type) => new Variable(type, "v" + type)).ToList();

                var variableNames = string.Join(", ", variables.Select(v => v.Name));
                var inputTypeList = string.Join(", ", variables.Select(v => v.Type));

                var partials = variables.PickOneReturnRest().Select(pair =>
                {
                    (Variable applied, IReadOnlyList<Variable> rest) = pair;
                    var restOfTypes = string.Join(", ", rest.Select(v => v.Type));
                    var restOfVariableNames = string.Join(", ", rest.Select(v => v.Name));
                    return $"internal static Func<{restOfTypes}, TR> Partial<{inputTypeList}, TR>(" +
                                $"this Func<{inputTypeList}, TR> f, " +
                                $"{applied.Type} {applied.Name}) => " +
                                    $"({restOfVariableNames}) => f({variableNames});";
                });

                return string.Join('\n', partials);
            }
        }

        /// <summary>
        /// String representation of a variable (type + name)
        /// </summary>
        private class Variable
        {
            public Variable(string type, string name)
            {
                Type = type;
                Name = name;
            }
            public string Type { get; init; }
            public string Name { get; init; }

            public override string ToString()
            {
                return Type + " " + Name;
            }
        }
    }
}
