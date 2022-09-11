using System;
using System.Collections.Generic;
using System.Linq;
using ParsecCore.MaybeNS;

namespace ParsecCore.Permutations
{
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

        private Perms(IEnumerable<Branch<TResult, TParserInput>> branches, IMaybe<TResult> defaults)
        {
            _branches = branches;
            _defaults = defaults;
        }

        internal Parser<TResult, TParserInput> Permute()
        {
            if (!_defaults.IsEmpty)
            {
                return Parsers.Return<TResult, TParserInput>(_defaults.Value);
            }

            var branchParsers = _branches.Select(branch => branch.GetParserFromBranch<TResult>());
            return Combinators.Choice(branchParsers);
        }

        //internal Perms<TResult, TParserInput> Add<TA>(Parser<TA, TParserInput> parser)
        //{
            
        //}

        //internal Perms<TResult, TParserInput> AddOptional<TA>(Parser<TA, TParserInput> parser, TA defaultValue)
        //{

        //}

        private Perms<TB, TParserInput> MapPerms<TB>(Func<TResult, TB> func)
        {
            var newDefault = _defaults.Map(func);
            var newBranches = _branches.Select(branch => branch.MapBranch<TResult, TB, TParserInput>(func));
            return new Perms<TB, TParserInput>(newBranches, newDefault);
        }

        private readonly IEnumerable<Branch<TResult, TParserInput>> _branches;
        private readonly IMaybe<TResult> _defaults;
    }

    public class PermsFunction<TA, TB, TParserInput>
    {

        internal static PermsFunction<TA, TB, TParserInput> NewPerm(Func<TA, TB> func)
        {
            return new PermsFunction<TA, TB, TParserInput>(
                Enumerable.Empty<Branch<Func<TA, TB>, TParserInput>>(),
                Maybe.FromValue(func)
            );
        }

        private PermsFunction(IEnumerable<Branch<Func<TA, TB>, TParserInput>> branches, IMaybe<Func<TA, TB>> defaults)
        {
            _branches = branches;
            _defaults = defaults;
        }

        internal Parser<Func<TA, TB>, TParserInput> Permute()
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
            throw new NotImplementedException();
        }

        internal Perms<TB, TParserInput> AddOptional(Parser<TA, TParserInput> parser, TA defaultValue)
        {
            throw new NotImplementedException();
        }

        private readonly IEnumerable<Branch<Func<TA, TB>, TParserInput>> _branches;
        private readonly IMaybe<Func<TA, TB>> _defaults;
    }
    
    internal static class PermsExt
    {
        private static Perms<TB, TInput> MapPerms<TA, TB, TInput>(this Perms<TA, TInput> perms, Func<TA, TB> func)
        {
            throw new NotImplementedException();
        }

        internal static Perms<TNewResult, TParserInput> Add<TResult, TNewResult, TParserInput>(
            this Perms<Func<TResult, TNewResult>, TParserInput> perms,
            Parser<TResult, TParserInput> parser
        )
        {
            throw new NotImplementedException();
        }
    }
}
