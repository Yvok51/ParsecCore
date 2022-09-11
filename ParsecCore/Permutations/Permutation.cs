using System;

namespace ParsecCore.Permutations
{
    public static class Permutation
    {
        public static Parser<Result, TInput> Permute<Result, TInput>(Perms<Result, TInput> permutationTree)
        {
            return permutationTree.Permute();
        }

        public static Perms<TB, TParserInput> NewPermutation<TA, TB, TParserInput>(
            Parser<TA, TParserInput> parser,
            Func<TA, TB> func
        )
        {
            var newPerm = PermsFunction<TA, TB, TParserInput>.NewPerm(func);
            return Add(newPerm, parser);
        }

        public static Perms<TB, TParserInput> Add<TA, TB, TParserInput>(
            this PermsFunction<TA, TB, TParserInput> perms,
            Parser<TA, TParserInput> parser
        )
        {
            return perms.Add(parser);
        }

        public static Perms<TB, TParserInput> NewOptionalPermutation<TA, TB, TParserInput>(
            Parser<TA, TParserInput> parser,
            Func<TA, TB> func,
            TA defaultValue
        )
        {
            var newPerm = PermsFunction<TA, TB, TParserInput>.NewPerm(func);
            return AddOptional(newPerm, parser, defaultValue);
        }

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
