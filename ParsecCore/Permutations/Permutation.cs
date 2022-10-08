using System;

namespace ParsecCore.Permutations
{
    public static class Permutation
    {
        /// <summary>
        /// Create a parser (<see cref="Parser{T, TInput}"/>) from a permutation parser
        /// (<see cref="Perms{TResult, TParserInput}"/>)
        /// </summary>
        /// <typeparam name="Result"> The result type of the parser </typeparam>
        /// <typeparam name="TInput"> The input type of the parser </typeparam>
        /// <param name="permutationTree"> The permutation tree which represents a permutation parser </param>
        /// <returns> Parser able to parse a permutation </returns>
        public static Parser<Result, TInput> Permute<Result, TInput>(Perms<Result, TInput> permutationTree)
        {
            return permutationTree.Permute();
        }

        /// <summary>
        /// Create a new permutation parser which parses <paramref name="parser"/>. The final value is gained
        /// by applying <paramref name="func"/> to the parsed value of <paramref name="parser"/>.
        /// 
        /// <paramref name="parser"/> cannot accept empty input. Use 
        /// <see cref="NewOptionalPermutation{TA, TB, TParserInput}(Parser{TA, TParserInput}, Func{TA, TB}, TA)"/>
        /// instead if that is the case
        /// </summary>
        /// <typeparam name="TA"> The result type of the parser </typeparam>
        /// <typeparam name="TB"> The result type of the permutation parser </typeparam>
        /// <typeparam name="TParserInput"> The input type of the parsers </typeparam>
        /// <param name="parser"> The base parser </param>
        /// <param name="func"> The final transformation </param>
        /// <returns> New permutation parser </returns>
        public static Perms<TB, TParserInput> NewPermutation<TA, TB, TParserInput>(
            Parser<TA, TParserInput> parser,
            Func<TA, TB> func
        )
        {
            var newPerm = PermsFunction<TA, TB, TParserInput>.NewPerm(func);
            return Add(newPerm, parser);
        }

        /// <summary>
        /// Add a new parser to the permutation parser
        /// </summary>
        /// <typeparam name="TA"> The return type of the added parser </typeparam>
        /// <typeparam name="TB"> The return type of the new parser </typeparam>
        /// <typeparam name="TParserInput"> The input type of the parsers </typeparam>
        /// <param name="perms"> Permutation parser with the return type of function TA -> TB </param>
        /// <param name="parser"> Parser to add to the permutation parser </param>
        /// <returns> A new parser which adds <paramref name="parser"/> to the permutation </returns>
        public static Perms<TB, TParserInput> Add<TA, TB, TParserInput>(
            this PermsFunction<TA, TB, TParserInput> perms,
            Parser<TA, TParserInput> parser
        )
        {
            return perms.Add(parser);
        }

        /// <summary>
        /// Create a new permutation parser which parses <paramref name="parser"/>. 
        /// The final value is gained by applying <paramref name="func"/> 
        /// to the parsed value of <paramref name="parser"/>.
        /// 
        /// <paramref name="parser"/> CAN accept empty input. In such a case the <paramref name="defaultValue"/>
        /// is used as input to <paramref name="func"/>
        /// </summary>
        /// <typeparam name="TA"> The result type of the parser </typeparam>
        /// <typeparam name="TB"> The result type of the permutation parser </typeparam>
        /// <typeparam name="TParserInput"> The input type of the parsers </typeparam>
        /// <param name="parser"> The base parser </param>
        /// <param name="func"> The final transformation </param>
        /// <param name="defaultValue"> The default value to use when an empty string is parsed </param>
        /// <returns> New permutation parser </returns>
        public static Perms<TB, TParserInput> NewOptionalPermutation<TA, TB, TParserInput>(
            Parser<TA, TParserInput> parser,
            Func<TA, TB> func,
            TA defaultValue
        )
        {
            var newPerm = PermsFunction<TA, TB, TParserInput>.NewPerm(func);
            return AddOptional(newPerm, parser, defaultValue);
        }

        /// <summary>
        /// Add a new parser to the permutation parser
        /// </summary>
        /// <typeparam name="TA"> The return type of the added parser </typeparam>
        /// <typeparam name="TB"> The return type of the new parser </typeparam>
        /// <typeparam name="TParserInput"> The input type of the parsers </typeparam>
        /// <param name="perms"> Permutation parser with the return type of function TA -> TB </param>
        /// <param name="parser"> Parser to add to the permutation parser </param>
        /// <param name="defaultValue"> 
        /// The default value to use if <paramref name="parser"/> parses an empty string 
        /// </param>
        /// <returns> A new parser which adds <paramref name="parser"/> to the permutation </returns>
        public static Perms<TB, TParserInput> AddOptional<TA, TB, TParserInput>(
            this PermsFunction<TA, TB, TParserInput> perms,
            Parser<TA, TParserInput> parser,
            TA defaultValue
        )
        {
            return perms.AddOptional(parser, defaultValue);
        }
    }
}
