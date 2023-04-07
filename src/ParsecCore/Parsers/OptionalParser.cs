using ParsecCore.MaybeNS;

namespace ParsecCore.ParsersHelp
{
    internal class OptionalParser
    {
        public static Parser<Maybe<T>, TInputToken> Parser<T, TInputToken>(Parser<T, TInputToken> parser)
        {
            return (input) =>
            {
                var result = parser(input);
                if (result.IsResult)
                {
                    return Result.Success(Maybe.FromValue(result.Result), result.UnconsumedInput);
                }
                if (input.Equals(result.UnconsumedInput))
                {
                    return Result.Success(Maybe.Nothing<T>(), result.UnconsumedInput);
                }

                return Result.RetypeError<T, Maybe<T>, TInputToken>(result);
            };
        }
    }
}
