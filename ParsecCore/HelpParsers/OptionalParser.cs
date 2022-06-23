using ParsecCore.Input;
using ParsecCore.Maybe;
using ParsecCore.Either;

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
                return EitherExt.Result<ParseError, IMaybe<T>>(MaybeExt.FromValue(result.Right));
            }

            input.Seek(initialPosition);
            return EitherExt.Result<ParseError, IMaybe<T>>(MaybeExt.Nothing<T>());
        }

        private readonly IParser<T> _parser;
    }
}
