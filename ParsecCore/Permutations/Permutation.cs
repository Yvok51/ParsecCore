using System;
using System.Collections.Generic;
using ParsecCore.MaybeNS;

namespace ParsecCore.Permutations
{
    public static partial class Permutation
    {
        public static SplitParser<T, TInput> OptionalParser<T, TInput>(Parser<T, TInput> parser, T defaultValue)
        {
            return new SplitParser<T, TInput>(parser, Maybe.FromValue(defaultValue));
        }

        public static SplitParser<T, TInput> Parser<T, TInput>(Parser<T, TInput> parser)
        {
            return new SplitParser<T, TInput>(parser, Maybe.Nothing<T>());
        }
    }
}
