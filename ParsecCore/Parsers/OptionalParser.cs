using ParsecCore.EitherNS;
using ParsecCore.MaybeNS;

namespace ParsecCore.ParsersHelp
{
    class OptionalParser
    {
        public static Parser<IMaybe<T>> Parser<T>(Parser<T> parser)
        {
            return (input) =>
            {
                var initialPosition = input.Position;
                var result = parser(input);
                if (result.HasRight)
                {
                    return Either.Result<ParseError, IMaybe<T>>(Maybe.FromValue(result.Right));
                }
                if (initialPosition == input.Position)
                {
                    return Either.Result<ParseError, IMaybe<T>>(Maybe.Nothing<T>());
                }

                return Either.Error<ParseError, IMaybe<T>>(result.Left);
            };
        }
    }
}
