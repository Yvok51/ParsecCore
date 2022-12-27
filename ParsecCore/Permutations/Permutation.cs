using System;
using ParsecCore.MaybeNS;

namespace ParsecCore.Permutations
{
    public static partial class Permutation
    {
        /// <summary>
        /// Create a <see cref="SplitParser{T, TInput}"/> for use in 
        /// <see cref="Permute{TA, TParserInput, TR}(SplitParser{TA, TParserInput}, Func{TA, TR})"/>.
        /// Signifies a member of a permutation that <b>is</b> optional and may not appear in the input.
        /// Important to note that <paramref name="parser"/> itself <b>shall not</b> accept empty string,
        /// otherwise the parsing is ambiguous.
        /// </summary>
        /// <typeparam name="T"> The return type of the parser </typeparam>
        /// <typeparam name="TInput"> The input type of the parser </typeparam>
        /// <param name="parser"> The parser to add to the permutation </param>
        /// <param name="defaultValue"> The default value to use in case the parsed member is not present </param>
        /// <returns> <see cref="SplitParser{T, TInput}"/> used in parsing permutations </returns>
        public static SplitParser<T, TInput> OptionalPermuteParser<T, TInput>(this Parser<T, TInput> parser, T defaultValue)
        {
            return new SplitParser<T, TInput>(parser, Maybe.FromValue(defaultValue));
        }

        /// <summary>
        /// Create a <see cref="SplitParser{T, TInput}"/> for use in 
        /// <see cref="Permute{TA, TParserInput, TR}(SplitParser{TA, TParserInput}, Func{TA, TR})"/>.
        /// Signifies a member of a permutation that <b>is not</b> optional and must appear in the input.
        /// </summary>
        /// <typeparam name="T"> The return type of the parser </typeparam>
        /// <typeparam name="TInput"> The input type of the parser </typeparam>
        /// <param name="parser"> The parser to add to the permutation </param>
        /// <returns> <see cref="SplitParser{T, TInput}"/> used in parsing permutations </returns>
        public static SplitParser<T, TInput> PermuteParser<T, TInput>(this Parser<T, TInput> parser)
        {
            return new SplitParser<T, TInput>(parser, Maybe.Nothing<T>());
        }
    }
}
