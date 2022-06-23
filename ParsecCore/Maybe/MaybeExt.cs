using System;

namespace ParsecCore.Maybe
{
    public static class MaybeExt
    {
        public static IMaybe<T> FromValue<T>(this T value)
        {
            return new Just<T>(value);
        }

        public static IMaybe<T> Nothing<T>()
        {
            return new Nothing<T>();
        }

        public static T Else<T>(this IMaybe<T> maybe, T defaultValue)
        {
            return maybe.IsEmpty ? defaultValue : maybe.Value;
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
                        secondResult => FromValue(getResult(firstResult, secondResult)),
                        () => Nothing<TResult>()
                    );
                },
                () => Nothing<TResult>()
            );
        }
    }
}
