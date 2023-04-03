using ParsecCore.Help;
using ParsecCore.MaybeNS;
using System;
using System.Collections.Generic;

namespace ParsecCore.Permutations
{
    /// <summary>
    /// Represents a set of parsers which can be parsed in any order.
    /// Thus they are able to parse a permutation of parsers.
    /// <para>
    /// This class is immutable and thus partial <see cref="PermutationParser{T, TInput}"/> can be reused
    /// for the definition of parsers for multiple differing permutations with only a common core.
    /// </para>
    /// <para>
    /// Be aware that all parsers added to the <see cref="PermutationParser{T, TInput}"/> must consume input
    /// whenever they succeed, otherwise the implementation for parsing permutations breaks.
    /// If you want to add a parser which does not consume any input, then use
    /// <see cref="PermutationParser{T, TInput}.AddOptional{U, R}(Parser{U, TInput}, U, Func{T, U, R})"/>.
    /// </para>
    /// <para>
    /// Example for using <see cref="PermutationParser{T, TInput}"/>:
    /// <code>
    ///  var parser = Permutation.NewPermutation(Parsers.Char('a'))
    ///        .AddOptional(Parsers.Char('b'), 'd')
    ///        .Add(Parsers.Char('c'), (pair, c) => (pair.Item1, pair.Item2, c))
    ///        .GetParser();
    /// </code>
    /// The following code creates a parser that parses a permutation of letters 'a', 'b', and 'c'.
    /// The letter 'b' is optional and, in case it is missing, the letter d is provided by the parser instead.
    /// The final return type is a triple of (<see cref="char"/>, <see cref="char"/>, <see cref="char"/>)
    /// </para>
    /// <para>
    /// By default the <see cref="PermutationParser{T, TInput}"/> creates a nested couple of the parsed results
    /// as the result of the parse. For instances, if we did not provide the combination parser in the shown example,
    /// then the final type of the parser would be ((<see cref="char"/>, <see cref="char"/>), <see cref="char"/>)
    /// </para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    public class PermutationParser<T, TInput>
    {
        internal PermutationParser(IReadOnlyList<IPermutationBranch<T, TInput>> branches, IMaybe<T> @default)
        {
            _branches = branches;
            _default = @default;
        }

        /// <summary>
        /// Create the parser that parses the constructed permutation.
        /// After all parsers that we want for our permutation are added, we create the actual parser.
        /// The <see cref="PermutationParser{T, TInput}"/> is able to be used further and added to further,
        /// it will not conflict with the created parser.
        /// </summary>
        /// <returns> Parser for the constructed permutation </returns>
        public Parser<T, TInput> GetParser()
        {
            var parser = Parsers.Choice(_branches.Map(branch => branch.GetParser()));

            return _default.Match(
                just: (val) => parser.Or(Parsers.Return<T, TInput>(val)),
                nothing: () => parser
            );
        }

        /// <summary>
        /// Add a new parser which cannot fail.
        /// <para>
        /// In case <paramref name="parser"/> fails with consuming input, the parser of the entire permutation fails.
        /// Use <see cref="ParserExt.Try{T, TInputToken}(Parser{T, TInputToken})"/> to change this behaviour.
        /// </para>
        /// <para>
        /// The <paramref name="parser"/> must consume input whenever it succeeds, otherwise the implementation
        /// of permutation parsing breaks.
        /// </para>
        /// <para>
        /// This method creates a <see cref="PermutationParser{T, TInput}"/> that has a nested couple of the parsed
        /// results as its result type.
        /// Use <see cref="PermutationParser{T, TInput}.Add{U, R}(Parser{U, TInput}, Func{T, U, R})"/>
        /// to change this.
        /// </para>
        /// </summary>
        /// <typeparam name="U"> The result type of the parser we are adding </typeparam>
        /// <param name="parser"> The parser that is added </param>
        /// <returns>
        /// A new permutation parser who has <paramref name="parser"/> added to its collection of parsers
        /// </returns>
        /// <exception cref="ArgumentNullException"> If any of the arguments are null </exception>
        public PermutationParser<(T, U), TInput> Add<U>(Parser<U, TInput> parser)
        {
            return Add(parser, ValueTuple.Create);
        }

        /// <summary>
        /// Add a new parser which cannot fail.
        /// <para>
        /// In case <paramref name="parser"/> fails with consuming input, the parser of the entire permutation fails.
        /// Use <see cref="ParserExt.Try{T, TInputToken}(Parser{T, TInputToken})"/> to change this behaviour.
        /// </para>
        /// <para>
        /// The <paramref name="parser"/> must consume input whenever it succeeds, otherwise the implementation
        /// of permutation parsing breaks.
        /// </para>
        /// </summary>
        /// <typeparam name="U"> The result type of the parser we are adding </typeparam>
        /// <typeparam name="R"> The result type of the added to PermutationParser </typeparam>
        /// <param name="parser"> The parser that is added </param>
        /// <param name="combine">
        /// The function used to combine the result of the parser and the rest of the permutation
        /// </param>
        /// <returns>
        /// A new permutation parser who has <paramref name="parser"/> added to its collection of parsers
        /// </returns>
        /// <exception cref="ArgumentNullException"> If any of the arguments are null </exception>
        public PermutationParser<R, TInput> Add<U, R>(Parser<U, TInput> parser, Func<T, U, R> combine)
        {
            if (parser is null) throw new ArgumentNullException(nameof(parser));
            if (combine is null) throw new ArgumentNullException(nameof(combine));

            return new PermutationParser<R, TInput>(AddParserToBranches(_branches, parser, combine), Maybe.Nothing<R>());
        }
        /// <summary>
        /// Add a new parser that can fail while not consuming any input.
        /// <para>
        /// If the added parser fails while consuming input, then the entire permutation parser still fails,
        /// use <see cref="ParserExt.Try{T, TInputToken}(Parser{T, TInputToken})"/> to change this behavior.
        /// However, if the this parser fails without consuming input, i.e. the part parsed by
        /// this parser is not in the permutation,
        /// then the defaultValue is used and the permutation parser continues on.
        /// </para>
        /// <para>
        /// The <paramref name="parser"/> must consume input whenever it succeeds, otherwise the implementation
        /// of permutation parsing breaks. 
        /// The case when the part parsed by <paramref name="parser"/> is handled by <paramref name="defaultValue"/>.
        /// </para>
        /// <para>
        /// This method creates a <see cref="PermutationParser{T, TInput}"/> that has a nested couple of the parsed
        /// results as its result type.
        /// Use <see cref="PermutationParser{T, TInput}.AddOptional{U, R}(Parser{U, TInput}, U, Func{T, U, R})"/>
        /// to change this.
        /// </para>
        /// </summary>
        /// <typeparam name="U"> The return type of the added parser </typeparam>
        /// <param name="parser"> The parser to add to permutation </param>
        /// <param name="defaultValue">
        /// The default value to use if the part parsed by <paramref name="parser"/> is missing
        /// </param>
        /// <returns>
        /// A new permutation parser who has <paramref name="parser"/> added to its collection of parsers
        /// </returns>
        /// <exception cref="ArgumentNullException"> If any of the arguments are null </exception>
        public PermutationParser<(T, U), TInput> AddOptional<U>(Parser<U, TInput> parser, U defaultValue)
            => AddOptional(parser, defaultValue, ValueTuple.Create);

        /// <summary>
        /// Add a new parser that can fail while not consuming any input.
        /// <para>
        /// If the added parser fails while consuming input, then the entire permutation parser still fails,
        /// use <see cref="ParserExt.Try{T, TInputToken}(Parser{T, TInputToken})"/> to change this behavior.
        /// However, if the this parser fails without consuming input, i.e. the part parsed by
        /// this parser is not in the permutation,
        /// then the defaultValue is used and the permutation parser continues on.
        /// </para>
        /// <para>
        /// The <paramref name="parser"/> must consume input whenever it succeeds, otherwise the implementation
        /// of permutation parsing breaks. 
        /// The case when the part parsed by <paramref name="parser"/> is handled by <paramref name="defaultValue"/>.
        /// </para>
        /// </summary>
        /// <typeparam name="U"> The return type of the added parser </typeparam>
        /// <typeparam name="R"> The new return type of the permutation parser </typeparam>
        /// <param name="parser"> The parser to add to permutation </param>
        /// <param name="defaultValue">
        /// The default value to use if the part parsed by <paramref name="parser"/> is missing
        /// </param>
        /// <param name="combine">
        /// The function used to combine the result of the parser and the rest of the permutation
        /// </param>
        /// <returns>
        /// A new permutation parser who has <paramref name="parser"/> added to its collection of parsers
        /// </returns>
        /// <exception cref="ArgumentNullException"> If any of the arguments are null </exception>
        public PermutationParser<R, TInput> AddOptional<U, R>(
            Parser<U, TInput> parser,
            U defaultValue,
            Func<T, U, R> combine
        )
        {
            if (parser is null) throw new ArgumentNullException(nameof(parser));
            if (defaultValue is null) throw new ArgumentNullException(nameof(defaultValue));
            if (combine is null) throw new ArgumentNullException(nameof(combine));

            return new PermutationParser<R, TInput>(
                AddOptionalParserToBranches(_branches, parser, defaultValue, combine),
                _default.Map(val => combine(val, defaultValue))
            );
        }


        private IReadOnlyList<IPermutationBranch<Result, TInput>> AddParserToBranches<TNew, Result>(
            IReadOnlyList<IPermutationBranch<T, TInput>> branches,
            Parser<TNew, TInput> parser,
            Func<T, TNew, Result> finalTransform
        )
        {
            return branches
                .Map(branch => branch.Add(parser, finalTransform))
                .Append(new PermutationBranch<TNew, T, Result, TInput>(parser, this, finalTransform));
        }

        private IReadOnlyList<IPermutationBranch<Result, TInput>> AddOptionalParserToBranches<TNew, Result>(
            IReadOnlyList<IPermutationBranch<T, TInput>> branches,
            Parser<TNew, TInput> parser,
            TNew defaultValue,
            Func<T, TNew, Result> finalTransform
        )
        {
            return branches
                .Map(branch => branch.AddOptional(parser, defaultValue, finalTransform))
                .Append(new PermutationBranch<TNew, T, Result, TInput>(parser, this, finalTransform));
        }

        /// <summary>
        /// Branches to try parsing with.
        /// A branch is a rooted tree of parsers and every path from the root
        /// towards the leaves is tried as a potential combination.
        /// </summary>
        private readonly IReadOnlyList<IPermutationBranch<T, TInput>> _branches;
        /// <summary>
        /// The default value returned if all branches fail in case there are missing optional parsers at the end
        /// </summary>
        private readonly IMaybe<T> _default;
    }
}
