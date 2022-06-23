using ParsecCore.Input;
using ParsecCore.MaybeNS;
using ParsecCore.EitherNS;

namespace ParsecCore
{
    class OptionalParser<T> : IParser<IMaybe<T>>
    {
        public OptionalParser(IParser<T> parser)
        {
            _parser = parser;
        }

        public IEither<ParseError, IMaybe<T>> Parse(IParserInput input)
        {
            var initialPosition = input.Position;
            var result = _parser.Parse(input);
            if (result.HasRight)
            {
                return Either.Result<ParseError, IMaybe<T>>(Maybe.FromValue(result.Right));
            }

            input.Seek(initialPosition);
            return Either.Result<ParseError, IMaybe<T>>(Maybe.Nothing<T>());
        }

        private readonly IParser<T> _parser;
    }
}
