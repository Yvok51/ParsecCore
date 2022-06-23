using System;

namespace ParsecCore.MaybeNS
{
    struct Nothing<T> : IMaybe<T>
    {
        public bool IsEmpty => true;

        public T Value => throw new InvalidOperationException();

        public IMaybe<TNew> Bind<TNew>(Func<T, TNew> map)
        {
            return new Nothing<TNew>();
        }

        public TNew Match<TNew>(Func<T, TNew> just, Func<TNew> nothing)
        {
            return nothing();
        }
    }
}
