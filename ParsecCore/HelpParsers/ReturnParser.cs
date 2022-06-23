using ParsecCore.EitherNS;
using ParsecCore.Input;

namespace ParsecCore.HelpParsers
{
    /// <summary>
    /// Simple parser that takes no input and only returns the value given to it at creation
    /// </summary>
    /// <typeparam name="T"> The type of the returned value </typeparam>
    class ReturnParser<T> : IParser<T>
    {
        public ReturnParser(T value)
        {
            _value = value;
        }

        public IEither<ParseError, T> Parse(IParserInput input)
        {
            return new Result<ParseError, T>(_value);
        }

        private readonly T _value;
    }
}
