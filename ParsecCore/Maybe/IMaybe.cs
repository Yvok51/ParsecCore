using System;

namespace ParsecCore.Maybe
{
    public interface IMaybe<T>
    {
        bool IsEmpty { get; }

        T Value { get; }

        IMaybe<TNew> Bind<TNew>(Func<T, TNew> map);

        TNew Match<TNew>(Func<T, TNew> just, Func<TNew> nothing);
    }
}
