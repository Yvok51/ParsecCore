using System;

namespace ParsecCore.MaybeNS
{
    public static class MaybeExt
    {
        public static T Else<T>(this IMaybe<T> maybe, T defaultValue)
        {
            return maybe.IsEmpty ? defaultValue : maybe.Value;
        }

        public static TNew Lift<T, TNew>(this IMaybe<T> maybe, Func<T, TNew> map, Func<TNew> provideDefault)
        {
            return maybe.IsEmpty ? provideDefault() : map(maybe.Value);
        }

        /// <summary>
        /// An extension method used in LINQ expressions for a single `from ... in ...` statement
        /// In context of <see cref="IMaybe{T}"/> we use it to simulate 
        /// the list comprehension notation (weaker do notation) of Haskell.
        /// Inspired by the paper "Encoding monadic computations in C# using iterators" by Tomáš Petříček
        /// and <see href="https://tyrrrz.me/blog/monadic-comprehension-via-linq">this</see> blog post 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="projection"></param>
        /// <returns></returns>
        public static IMaybe<TResult> Select<TSource, TResult>(
            this IMaybe<TSource> source,
            Func<TSource, TResult> projection
        )
        {
            return source.Map(projection);
        }

        /// <summary>
        /// Select many is an extension method used in LINQ expressions to chain multiple `from ... in ...` statements together
        /// In context of <see cref="IMaybe{T}"/> we use it to simulate 
        /// the list comprehension notation (weaker do notation) of Haskell.
        /// Inspired by the paper "Encoding monadic computations in C# using iterators" by Tomáš Petříček
        /// and <see href="https://tyrrrz.me/blog/monadic-comprehension-via-linq">this</see> blog post 
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <typeparam name="TSecond"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="first"></param>
        /// <param name="getSecond"></param>
        /// <param name="getResult"></param>
        /// <returns></returns>
        public static IMaybe<TResult> SelectMany<TFirst, TSecond, TResult>(
            this IMaybe<TFirst> first,
            Func<TFirst, IMaybe<TSecond>> getSecond,
            Func<TFirst, TSecond, TResult> getResult)
        {
            return first.Match(
                firstResult =>
                {
                    var second = getSecond(firstResult);
                    return second.Match(
                        secondResult => Maybe.FromValue(getResult(firstResult, secondResult)),
                        () => Maybe.Nothing<TResult>()
                    );
                },
                () => Maybe.Nothing<TResult>()
            );
        }
    }
}
