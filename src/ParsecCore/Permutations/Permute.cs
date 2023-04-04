using ParsecCore.MaybeNS;
using System;
using System.Collections.Generic;

namespace ParsecCore.Permutations
{
    public static partial class Permutation
    {

        /// <summary>
        /// Creates a new <see cref="PermutationParser{T, TInput}"/> that is used to construct parsers for
        /// permutations. 
        /// <para>
        /// Look at <see cref="PermutationParser{T, TInput}.Add{U}(Parser{U, TInput})"/>
        /// and <see cref="PermutationParser{T, TInput}.AddOptional{U, R}(Parser{U, TInput}, U, Func{T, U, R})"/>
        /// for closer instructions on how to construct a parser for permutations.
        /// </para>
        /// <para>
        /// Look at <see cref="PermutationParser{T, TInput}.GetParser"/> for how to instantiate the parser after
        /// it has been constructed.
        /// </para>
        /// <para>
        /// The default, empty permutation parser does not parse any input.
        /// It returns an instance of <see cref="None"/> without consuming any input.
        /// See <see cref="NewPermutation{T, TInput}(Parser{T, TInput})"/> and
        /// <see cref="NewPermutationOptional{T, TInput}(Parser{T, TInput}, T)"/>
        /// for probably more useful starting methods.
        /// </para>
        /// </summary>
        /// <typeparam name="TInput"> The input type of the eventual parser </typeparam>
        /// <returns> New PermutationParser used for creating parsers for permutations </returns>
        public static PermutationParser<None, TInput> NewPermutation<TInput>()
        {
            return new PermutationParser<None, TInput>(
                Array.Empty<IPermutationBranch<None, TInput>>(),
                Maybe.FromValue(None.Instance)
            );
        }

        /// <summary>
        /// Creates a new <see cref="PermutationParser{T, TInput}"/> that is used to construct parsers for
        /// permutations.
        /// <para>
        /// Look at <see cref="PermutationParser{T, TInput}.Add{U}(Parser{U, TInput})"/>
        /// and <see cref="PermutationParser{T, TInput}.AddOptional{U, R}(Parser{U, TInput}, U, Func{T, U, R})"/>
        /// for closer instructions on how to construct a parser for permutations.
        /// </para>
        /// <para>
        /// Look at <see cref="PermutationParser{T, TInput}.GetParser"/> for how to instantiate the parser after
        /// it has been constructed.
        /// </para>
        /// <para>
        /// A PermutationParser created by this method, if built (<see cref="PermutationParser{T, TInput}.GetParser"/>)
        /// right after creation, tries to parse the provided parser and return its result.
        /// It is functionally equivalent with <paramref name="firstParser"/>.
        /// We can of course add more parsers to the permutation after creation.
        /// </para>
        /// </summary>
        /// <typeparam name="T"> The result type of the provided parser </typeparam>
        /// <typeparam name="TInput"> The input type of the eventual parser </typeparam>
        /// <param name="firstParser"> The first parser to be added to the permutation </param>
        /// <returns> New PermutationParser used for creating parsers for permutations </returns>
        public static PermutationParser<T, TInput> NewPermutation<T, TInput>(Parser<T, TInput> firstParser)
        {
            return NewPermutation<TInput>().Add(firstParser, (_, res) => res);
        }

        /// <summary>
        /// Creates a new <see cref="PermutationParser{T, TInput}"/> that is used to construct parsers for
        /// permutations.
        /// <para>
        /// Look at <see cref="PermutationParser{T, TInput}.Add{U}(Parser{U, TInput})"/>
        /// and <see cref="PermutationParser{T, TInput}.AddOptional{U, R}(Parser{U, TInput}, U, Func{T, U, R})"/>
        /// for closer instructions on how to construct a parser for permutations.
        /// </para>
        /// <para>
        /// Look at <see cref="PermutationParser{T, TInput}.GetParser"/> for how to instantiate the parser after
        /// it has been constructed.
        /// </para>
        /// <para>
        /// A PermutationParser created by this method, if built (<see cref="PermutationParser{T, TInput}.GetParser"/>)
        /// right after creation, tries to parse the provided parser and return its result.
        /// If it does not succeed without consuming input, then the default value is used.
        /// It is functionally equivalent with 
        /// <see cref="ParserExt.Option{T, TInputToken}(Parser{T, TInputToken}, T)"/> applied to
        /// <paramref name="firstParser"/>.
        /// We can of course add more parsers to the permutation after creation.
        /// </para>
        /// <para>
        /// It is important to note that <paramref name="firstParser"/> should not be able to succeed
        /// without consuming input.
        /// Otherwise, the implementation for <see cref="PermutationParser{T, TInput}"/> breaks.
        /// </para>
        /// </summary>
        /// <typeparam name="T"> The result type of the provided parser </typeparam>
        /// <typeparam name="TInput"> The input type of the eventual parser </typeparam>
        /// <param name="firstParser"> The first parser to be added to the permutation </param>
        /// <returns> New PermutationParser used for creating parsers for permutations </returns>
        public static PermutationParser<T, TInput> NewPermutationOptional<T, TInput>(Parser<T, TInput> firstParser, T defaultValue)
        {
            return NewPermutation<TInput>().AddOptional(firstParser, defaultValue, (_, res) => res);
        }
    }
}
