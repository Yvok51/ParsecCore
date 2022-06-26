using ParsecCore.MaybeNS;
using ParsecCore.EitherNS;

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

                input.Seek(initialPosition);
                return Either.Result<ParseError, IMaybe<T>>(Maybe.Nothing<T>());
            };
        }
    }
}
