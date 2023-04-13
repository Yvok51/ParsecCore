using System;

namespace ParsecCore.Permutations
{
    /// <summary>
    /// Represents branches in the permutation ending with result type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">
    /// The type that results from sequential application of parsers and transformation functions in the branch
    /// </typeparam>
    /// <typeparam name="TInput"> The input type </typeparam>
    internal interface IPermutationBranch<T, TInput>
    {
        /// <summary>
        /// Add a new parser to branches.
        /// This will create a new subbranch with <paramref name="parser"/> as the first parser tried
        /// and also add <paramref name="parser"/> to every branch at different positions.
        /// </summary>
        /// <remarks>
        /// For example, if we have two branches and 'A' and 'B' are parsers and 'F' is the first parser,
        /// then before the addition the branches look like this:
        /// F -- A -- B 
        ///  \
        ///    -- B -- A
        /// After the addition of the parser 'C' the structure looks like this:
        /// F -- A -- B -- C
        /// |     \
        /// |       - C -- B
        /// |\
        /// |  - B -- A -- C
        /// |     \
        /// |       - C -- A
        ///  \
        ///    - C -- A -- B
        ///       \
        ///         - B -- A
        /// </remarks>
        /// <typeparam name="Between"> The type of the first parser </typeparam>
        /// <typeparam name="Result"> The result type of the entire permutation parser </typeparam>
        /// <param name="parser"> The parser to add </param>
        /// <param name="combine">
        /// The function used to combine the results of the given parser and rest of the branch
        /// </param>
        /// <returns> An updated branch with an added parser </returns>
        public abstract IPermutationBranch<Result, TInput> Add<Between, Result>(
            Parser<Between, TInput> parser,
            Func<T, Between, Result> combine
        );

        /// <summary>
        /// Add a new parser to branches.
        /// This will create a new subbranch with <paramref name="parser"/> as the first parser tried
        /// and also add <paramref name="parser"/> to every branch at different positions.
        /// </summary>
        /// <remarks>
        /// For example, if we have two branches and 'A' and 'B' are parsers and 'F' is the first parser,
        /// then before the addition the branches look like this:
        /// F -- A -- B 
        ///  \
        ///    -- B -- A
        /// After the addition of the parser 'C' the structure looks like this:
        /// F -- A -- B -- C
        /// |     \
        /// |       - C -- B
        /// |\
        /// |  - B -- A -- C
        /// |     \
        /// |       - C -- A
        ///  \
        ///    - C -- A -- B
        ///       \
        ///         - B -- A
        /// </remarks>
        /// <typeparam name="Between"> The result type of the added parser </typeparam>
        /// <typeparam name="Result"> The new result type of the entire permutation parser </typeparam>
        /// <param name="parser"> The parser to add </param>
        /// <param name="combine">
        /// The function used to combine the results of the given parser and rest of the branch
        /// </param>
        /// <returns> An updated branch with an added parser </returns>
        public abstract IPermutationBranch<Result, TInput> AddOptional<Between, Result>(
            Parser<Between, TInput> parser,
            Between defaultValue,
            Func<T, Between, Result> combine
        );

        /// <summary>
        /// Provide a final parser which parses the permutation we have constructed
        /// </summary>
        /// <returns> Parser for the constructed permutation </returns>
        public abstract Parser<T, TInput> GetParser();
    }

    /// <summary>
    /// The implementation is split into an interface and implementing class so that we hide the types
    /// <typeparamref name="Current"/> and <typeparamref name="Rest"/> from the rest of the implementation.
    /// This simulates existential types used in the original algorithm "Parsing permutation phrases"
    /// </summary>
    /// <typeparam name="Current"> The result type of the first parser in the branch </typeparam>
    /// <typeparam name="Rest">
    /// The result type of the rest of the branch
    /// (the <see cref="PermutationParser{T, TInput}"/> which represents the rest of the branch)
    /// </typeparam>
    /// <typeparam name="Final"> The return type of the entire branch </typeparam>
    /// <typeparam name="TInput"> The input type of the parsers </typeparam>
    internal class PermutationBranch<Current, Rest, Final, TInput> : IPermutationBranch<Final, TInput>
    {
        public PermutationBranch(
            Parser<Current, TInput> parser,
            PermutationParser<Rest, TInput> permutation,
            Func<Rest, Current, Final> combine
        )
        {
            _parser = parser;
            _permutation = permutation;
            _combine = combine;
        }

        public IPermutationBranch<Result, TInput> Add<Between, Result>(
            Parser<Between, TInput> parser,
            Func<Final, Between, Result> combine
        )
        {
            return new PermutationBranch<Current, (Rest, Between), Result, TInput>(
                _parser,
                _permutation.Add(parser),
                (pair, curr) => combine(_combine(pair.Item1, curr), pair.Item2)
            );
        }

        public IPermutationBranch<Result, TInput> AddOptional<Between, Result>(
            Parser<Between, TInput> parser,
            Between defaultValue,
            Func<Final, Between, Result> combine
        )
        {
            return new PermutationBranch<Current, (Rest, Between), Result, TInput>(
                _parser,
                _permutation.AddOptional(parser, defaultValue),
                (pair, curr) => combine(_combine(pair.Item1, curr), pair.Item2)
            );
        }

        public Parser<Final, TInput> GetParser()
        {
            return from first in _parser
                   from rest in _permutation.GetParser()
                   select _combine(rest, first);
        }

        /// <summary>
        /// The parser which is first in this branch
        /// </summary>
        private readonly Parser<Current, TInput> _parser;
        /// <summary>
        /// The permutation representing the rest of the branches that shoot off from our first parser,
        /// <see cref="PermutationBranch{Current, Rest, Final, TInput}._parser"/>.
        /// </summary>
        private readonly PermutationParser<Rest, TInput> _permutation;
        /// <summary>
        /// The final transformation function applied to the result of the parse to combine the results
        /// of the first parser, <see cref="PermutationBranch{Current, Rest, Final, TInput}._parser"/>, and the rest
        /// of the permutation, <see cref="PermutationBranch{Current, Rest, Final, TInput}._permutation"/>.
        /// </summary>
        private readonly Func<Rest, Current, Final> _combine;
    }
}
