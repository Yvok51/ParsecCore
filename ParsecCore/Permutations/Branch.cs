using System;
using System.Collections.Generic;
using System.Linq;

namespace ParsecCore.Permutations
{
    internal class Branch<TResult, TParserInput>
    {
        internal Branch(Perms<IBranchFunc<TResult>, TParserInput> branch, IBranchParser parser)
        {
            _branch = branch;
            _parserGetter = parser;
        }

        // Branch is represented by a permutation which a returns a function (T -> Result)
        // where T is existentially quantified
        private readonly Perms<IBranchFunc<TResult>, TParserInput> _branch;

        private readonly IBranchParser _parserGetter;

        internal Parser<TResult, TParserInput> GetParserFromBranch<TInterResult>() // We have to know the type of the intermediary result though :(
        {
            return from x in _parserGetter.getParser<TInterResult, TParserInput>() // Not fully generic - the input
                                                                                   // types have tp match (but that
                                                                                   // makes sense logically)
                   from f in Permutation.Permute(_branch)
                   select f.Apply(x);
        }

        internal Branch<TB, TInput> MapBranch<TA, TB, TInput>(Func<TA, TB> func)
        {
            throw new NotImplementedException();

            //Func < T, TB > composedFunction<T>(Func<T, TA> suppliedFunc)
            //{
            //    return (t) => func(suppliedFunc(t));
            //}
            //  new Branch<TB, TInput>()
        }
    }

    // Simulate the existential quantification of the (x -> a)
    // function in original Branch constructor
    internal interface IBranchFunc<Result>
    {
        public Result Apply<T>(T input);
    }

    // Simulate existential quantification of the (p x) parser
    internal interface IBranchParser
    {
        public Parser<TResult, TInput> getParser<TResult, TInput>();
    }
}
