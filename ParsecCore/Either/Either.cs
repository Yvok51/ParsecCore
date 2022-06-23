namespace ParsecCore.EitherNS
{
    public static class Either
    {
        public static IEither<TLeft, TRight> Result<TLeft, TRight>(TRight value)
        {
            return new Result<TLeft, TRight>(value);
        }

        public static IEither<TLeft, TRight> Error<TLeft, TRight>(TLeft value)
        {
            return new Error<TLeft, TRight>(value);
        }
    }
}
