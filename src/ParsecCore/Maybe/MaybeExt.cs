using System;

namespace ParsecCore
{
    public static class MaybeExt
    {
        /// <summary>
        /// Returns the value contained in maube or the <paramref name="defaultValue"/>
        /// if <paramref name="maybe"/> is empty.
        /// </summary>
        /// <typeparam name="T"> The type contained in <see cref="Maybe{T}"/></typeparam>
        /// <param name="maybe"> The <see cref="Maybe{T}"/> whose value we are extracting </param>
        /// <param name="defaultValue"> The default value to use if <paramref name="maybe"/> is empty </param>
        /// <returns></returns>
        public static T Else<T>(this Maybe<T> maybe, T defaultValue)
        {
            return maybe.IsEmpty ? defaultValue : maybe.Value;
        }

        /// <summary>
        /// Maps the stored value if it is present
        /// </summary>
        /// <typeparam name="TNew"> The type of the new stored value </typeparam>
        /// <param name="map"> The function to map the value </param>
        /// <returns> <c>IMaybe</c> with the new mapped value </returns>
        public static Maybe<TNew> Map<T, TNew>(this Maybe<T> maybe, Func<T, TNew> map)
        {
            if (maybe.IsEmpty)
            {
                return Maybe.Nothing<TNew>();
            }
            else
            {
                return Maybe.FromValue<TNew>(map(maybe.Value));
            }
        }

        /// <summary>
        /// Returns a mapped value
        /// </summary>
        /// <typeparam name="TNew"> The type of the mapped stored value </typeparam>
        /// <param name="just"> Mapping function to apply if value is present </param>
        /// <param name="nothing"> Fucntion to use if the instance is empty </param>
        /// <returns> Mapped value stored inside </returns>
        public static TNew Match<T, TNew>(this Maybe<T> maybe, Func<T, TNew> just, Func<TNew> nothing)
        {
            if (maybe.IsEmpty)
            {
                return nothing();
            }
            else
            {
                return just(maybe.Value);
            }
        }

        /// <summary>
        /// An extension method used in LINQ expressions for a single `from ... in ...` statement
        /// In context of <see cref="Maybe{T}"/> we use it to simulate 
        /// the list comprehension notation (weaker do notation) of Haskell.
        /// Inspired by the paper "Encoding monadic computations in C# using iterators" by Tomáš Petříček
        /// and <see href="https://tyrrrz.me/blog/monadic-comprehension-via-linq">this</see> blog post 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="projection"></param>
        /// <returns></returns>
        public static Maybe<TResult> Select<TSource, TResult>(
            this Maybe<TSource> source,
            Func<TSource, TResult> projection
        )
        {
            return source.Map(projection);
        }

        /// <summary>
        /// Select many is an extension method used in LINQ expressions to chain multiple `from ... in ...` statements together
        /// In context of <see cref="Maybe{T}"/> we use it to simulate 
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
        public static Maybe<TResult> SelectMany<TFirst, TSecond, TResult>(
            this Maybe<TFirst> first,
            Func<TFirst, Maybe<TSecond>> getSecond,
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
