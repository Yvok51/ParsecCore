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

        public static IMaybe<TResult> Select<TSource, TResult>(
            this IMaybe<TSource> source,
            Func<TSource, TResult> projection
        )
        {
            return source.Map(projection);
        }

        public static IMaybe<TResult> SelectMany<TFirst, TSecond, TResult>(
            this IMaybe<TFirst> first,
            Func<TFirst, IMaybe<TSecond>> getSecond,
            Func<TFirst, TSecond, TResult> getResult)
        {
            return first.Match(
                firstResult => {
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
