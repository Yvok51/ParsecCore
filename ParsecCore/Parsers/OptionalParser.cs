using ParsecCore.EitherNS;
using ParsecCore.MaybeNS;

namespace ParsecCore.ParsersHelp
{
    class OptionalParser
    {
        public static Parser<IMaybe<T>, TInputToken> Parser<T, TInputToken>(Parser<T, TInputToken> parser)
        {
            return (input) =>
            {
                var initialPosition = input.Position;
                var result = parser(input);
                if (result.IsResult)
                {
                    return Either.Result<ParseError, IMaybe<T>>(Maybe.FromValue(result.Result));
                }
                if (initialPosition == input.Position)
                {
                    return Either.Result<ParseError, IMaybe<T>>(Maybe.Nothing<T>());
                }

                return Either.Error<ParseError, IMaybe<T>>(result.Error);
            };
        }
    }
}
