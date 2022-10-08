using System;
using System.Collections.Generic;
using System.Linq;
using ParsecCore.MaybeNS;

namespace ParsecCore.Permutations
{
    /// <summary>
    /// Permutation parser
    /// </summary>
    /// <typeparam name="TResult"> The result type of the parser </typeparam>
    /// <typeparam name="TParserInput"> The input type of the parser</typeparam>
    public class Perms<TResult, TParserInput>
    {
        internal static Perms<TResult, TParserInput> Empty(TResult value)
        {
            return new Perms<TResult, TParserInput>(
                Enumerable.Empty<Branch<TResult, TParserInput>>(),
                Maybe.FromValue(value)
            );
        }

        //internal static Perms<Func<TA, TB>, TParserInput> NewPerm<TA, TB>(Func<TA, TB> func)
        //{
        //    return Perms<Func<TA, TB>, TParserInput>.Empty(func);
        //}

        internal Perms(IEnumerable<Branch<TResult, TParserInput>> branches, IMaybe<TResult> defaults)
        {
            _branches = branches;
            _defaults = defaults;
        }

        internal virtual Parser<TResult, TParserInput> Permute()
        {
            if (!_defaults.IsEmpty)
            {
                return Parsers.Return<TResult, TParserInput>(_defaults.Value);
            }

            var branchParsers = _branches.Select(branch => branch.GetParserFromBranch<TResult>());
            return Combinators.Choice(branchParsers);
        }

        protected virtual Perms<TResult, TParserInput> Clone()
        {
            return new Perms<TResult, TParserInput>(_branches, _defaults);
        }

        //internal Perms<TResult, TParserInput> Add<TA>(Parser<TA, TParserInput> parser)
        //{
            
        //}

        //internal Perms<TResult, TParserInput> AddOptional<TA>(Parser<TA, TParserInput> parser, TA defaultValue)
        //{

        //}

        protected virtual Perms<TB, TParserInput> MapPerms<TB>(Func<TResult, TB> func)
        {
            var newDefault = _defaults.Map(func);
            var newBranches = _branches.Select(branch => branch.MapBranch<TResult, TB, TParserInput>(func));
            return new Perms<TB, TParserInput>(newBranches, newDefault);
        }

        private protected readonly IEnumerable<Branch<TResult, TParserInput>> _branches;
        private protected readonly IMaybe<TResult> _defaults;
    }

    /// <summary>
    /// Permutation parser which has a function (TA -> TB) as its return type
    /// </summary>
    /// <typeparam name="TA"> The input type of the return function </typeparam>
    /// <typeparam name="TB"> The output type of the return function </typeparam>
    /// <typeparam name="TParserInput"> The input parser type </typeparam>
    public class PermsFunction<TA, TB, TParserInput> : Perms<Func<TA, TB>, TParserInput>
    {

        internal static PermsFunction<TA, TB, TParserInput> NewPerm(Func<TA, TB> func)
        {
            return new PermsFunction<TA, TB, TParserInput>(
                Enumerable.Empty<Branch<Func<TA, TB>, TParserInput>>(),
                Maybe.FromValue(func)
            );
        }

        private PermsFunction(IEnumerable<Branch<Func<TA, TB>, TParserInput>> branches, IMaybe<Func<TA, TB>> defaults)
            : base(branches, defaults)
        {
        }

        protected override Perms<Func<TA, TB>, TParserInput> Clone()
        {
            return new PermsFunction<TA, TB, TParserInput>(_branches, _defaults);
        }

        internal override Parser<Func<TA, TB>, TParserInput> Permute()
        {
            if (!_defaults.IsEmpty)
            {
                return Parsers.Return<Func<TA, TB>, TParserInput>(_defaults.Value);
            }

            var branchParsers = _branches.Select(branch => branch.GetParserFromBranch<Func<TA, TB>>());
            return Combinators.Choice(branchParsers);
        }

        internal Perms<TB, TParserInput> Add(Parser<TA, TParserInput> parser)
        {
            //var firstBranch = Branch<TB, TParserInput>.Create(Clone(), parser);
            //var newBranches = _branches.Select(branch => branch.addToBranch(parser));

            //return new Perms<TB, TParserInput>(newBranches, Maybe.Nothing<TB>());

            throw new NotImplementedException();
        }

        internal Perms<TB, TParserInput> AddOptional(Parser<TA, TParserInput> parser, TA defaultValue)
        {
            throw new NotImplementedException();
        }
    }
}
