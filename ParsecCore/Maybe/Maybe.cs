namespace ParsecCore.MaybeNS
{
    public static class Maybe
    {
        public static IMaybe<T> FromValue<T>(this T value)
        {
            return new Just<T>(value);
        }

        public static IMaybe<T> Nothing<T>()
        {
            return new Nothing<T>();
        }
    }
}
