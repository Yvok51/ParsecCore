using System;
using System.Collections.Generic;

namespace ParsecCore.Permutations
{
    public static partial class Permutation
    {
        /// <summary>
        /// <para>
        /// Create a parser which parses a permutation phrase.
        /// Permutation phrase is a sequence of elements in which each element occurs exactly once
        /// and the order is irrelevant.
        /// </para>
        /// <para>
        /// The created permutation parser will parse any combination of the given parsers,
        /// but will pass the parsed values to the final transformation function <paramref name="f"/>
        /// in the specified order.
        /// </para>
        /// <para>
        /// The parsers passed to the function should not parse empty string, for such functionality see
        /// <see cref="OptionalPermuteParser{T, TInput}(ParsecCore.Parser{T, TInput}, T)"/>.
        /// If any of the parsers do accept an empty string then the parsing of the permutation phrase is ambiguous
        /// and the result is not guaranteed to be correct.
        /// </para>
        /// </summary>
        /// <typeparam name="TA"> The type of the parsed value </typeparam>
        /// <typeparam name="TParserInput"> The input type of the parsers </typeparam>
        /// <typeparam name="TR"> The return type of the created parser </typeparam>
        /// <param name="splitParser"> The parser from which to construct the permutation parser </param>
        /// <param name="f">
        /// Final transformation function which takes all parsed values and creates the result value
        /// </param>
        /// <returns> Parser which parses a permutation phrase </returns>
        public static Parser<TR, TParserInput> Permute<TA, TParserInput, TR>(
            SplitParser<TA, TParserInput> splitParser,
            Func<TA, TR> f
        )
        {
            int branchCount = splitParser.IsOptional ? 2 : 1;
            List<Parser<TR, TParserInput>> branches = new(branchCount)
            {
                from a in splitParser.Parser
                select f(a)
            };

            if (splitParser.IsOptional)
            {
                branches.Add(Parsers.Return<TR, TParserInput>(f(splitParser.DefaultValue)));
            }

            return Combinators.Choice(branches);
        }

        /// <summary>
        /// <para>
        /// Create a parser which parses a permutation phrase.
        /// Permutation phrase is a sequence of elements in which each element occurs exactly once
        /// and the order is irrelevant.
        /// </para>
        /// <para>
        /// The created permutation parser will parse any combination of the given parsers,
        /// but will pass the parsed values to the final transformation function <paramref name="f"/>
        /// in the specified order.
        /// </para>
        /// <para>
        /// The parsers passed to the function should not parse empty string, for such functionality see
        /// <see cref="OptionalPermuteParser{T, TInput}(ParsecCore.Parser{T, TInput}, T)"/>.
        /// If any of the parsers do accept an empty string then the parsing of the permutation phrase is ambiguous
        /// and the result is not guaranteed to be correct.
        /// </para>
        /// </summary>
        /// <typeparam name="TA"> The type of the first parsed value </typeparam>
        /// <typeparam name="TB"> The type of the second parsed value </typeparam>
        /// <typeparam name="TParserInput"> The input type of the parser </typeparam>
        /// <typeparam name="TR"> The return type of the created parser </typeparam>
        /// <param name="splitParserA"> The first parser from which to construct the permutation parser </param>
        /// <param name="splitParserB"> The second parser from which to construct the permutation parser </param>
        /// <param name="f">
        /// Final transformation function which takes all parsed values and creates the result value
        /// </param>
        /// <returns> Parser which parses a permutation phrase </returns>
        public static Parser<TR, TParserInput> Permute<TA, TB, TParserInput, TR>(
            SplitParser<TA, TParserInput> splitParserA,
            SplitParser<TB, TParserInput> splitParserB,
            Func<TA, TB, TR> f
        )
        {
            int branchCount = splitParserA.IsOptional && splitParserB.IsOptional ? 3 : 2;
            List<Parser<TR, TParserInput>> branches = new(branchCount)
            {
                from a in splitParserA.Parser
                from r in Permute(splitParserB, f.Partial(a))
                select r,
                from b in splitParserB.Parser
                from r in Permute(splitParserA, f.Partial(b))
                select r,
            };

            if (splitParserA.IsOptional && splitParserB.IsOptional)
            {
                branches.Add(Parsers.Return<TR, TParserInput>(f(splitParserA.DefaultValue, splitParserB.DefaultValue)));
            }

            return Combinators.Choice(branches);
        }

        /// <summary>
        /// <para>
        /// Create a parser which parses a permutation phrase.
        /// Permutation phrase is a sequence of elements in which each element occurs exactly once
        /// and the order is irrelevant.
        /// </para>
        /// <para>
        /// The created permutation parser will parse any combination of the given parsers,
        /// but will pass the parsed values to the final transformation function <paramref name="f"/>
        /// in the specified order.
        /// </para>
        /// <para>
        /// The parsers passed to the function should not parse empty string, for such functionality see
        /// <see cref="OptionalPermuteParser{T, TInput}(ParsecCore.Parser{T, TInput}, T)"/>.
        /// If any of the parsers do accept an empty string then the parsing of the permutation phrase is ambiguous
        /// and the result is not guaranteed to be correct.
        /// </para>
        /// </summary>
        /// <typeparam name="TA"> The type of the first parsed value </typeparam>
        /// <typeparam name="TB"> The type of the second parsed value </typeparam>
        /// <typeparam name="TC"> The type of the third parsed value </typeparam>
        /// <typeparam name="TParserInput"> The input type of the parser </typeparam>
        /// <typeparam name="TR"> The return type of the created parser </typeparam>
        /// <param name="splitParserA"> The first parser from which to construct the permutation parser </param>
        /// <param name="splitParserB"> The second parser from which to construct the permutation parser </param>
        /// <param name="splitParserC"> The third parser from which to construct the permutation parser </param>
        /// <param name="f">
        /// Final transformation function which takes all parsed values and creates the result value
        /// </param>
        /// <returns> Parser which parses a permutation phrase </returns>
        public static Parser<TR, TParserInput> Permute<TA, TB, TC, TParserInput, TR>(
            SplitParser<TA, TParserInput> splitParserA,
            SplitParser<TB, TParserInput> splitParserB,
            SplitParser<TC, TParserInput> splitParserC,
            Func<TA, TB, TC, TR> f
        )
        {
            bool allParsersOptional = splitParserA.IsOptional && splitParserB.IsOptional && splitParserC.IsOptional;
            int branchCount = allParsersOptional ? 4 : 3;
            List<Parser<TR, TParserInput>> branches = new(branchCount)
            {
                from a in splitParserA.Parser
                from r in Permute(splitParserB, splitParserC, f.Partial(a))
                select r,
                from b in splitParserB.Parser
                from r in Permute(splitParserA, splitParserC, f.Partial(b))
                select r,
                from c in splitParserC.Parser
                from r in Permute(splitParserA, splitParserB, f.Partial(c))
                select r,
            };

            if (allParsersOptional)
            {
                branches.Add(Parsers.Return<TR, TParserInput>(f(splitParserA.DefaultValue, splitParserB.DefaultValue, splitParserC.DefaultValue)));
            }

            return Combinators.Choice(branches);
        }
    }
}
