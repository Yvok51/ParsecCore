using System;

namespace ParsecCore.MaybeNS
{
    public interface IMaybe<T>
    {
        bool IsEmpty { get; }

        T Value { get; }

        IMaybe<TNew> Map<TNew>(Func<T, TNew> map);

        TNew Match<TNew>(Func<T, TNew> just, Func<TNew> nothing);
    }
}
