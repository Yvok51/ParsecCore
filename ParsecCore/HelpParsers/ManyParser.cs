using System.Collections.Generic;

using ParsecCore.Input;
using ParsecCore.EitherNS;

namespace ParsecCore
{
    /// <summary>
    /// Parser which takes a parser and attempts to apply it as many times as possible 
    /// Used in the implementation of the Many method
    /// </summary>
    /// <typeparam name="T"> The type of parser return value </typeparam>
    class ManyParser<T> : IParser<IEnumerable<T>>
    {
        public ManyParser(IParser<T> parser)
        {
            _parser = parser;
        }

        public IEither<ParseError, IEnumerable<T>> Parse(IParserInput input)
        {
            List<T> result = new List<T>();

            var previousPosition = input.Position;
            var parseResult = _parser.Parse(input);
            while (parseResult.HasRight)
            {
                result.Add(parseResult.Right);
                previousPosition = input.Position;
                parseResult = _parser.Parse(input);
            }

            input.Seek(previousPosition);
            return Either.Result<ParseError, IEnumerable<T>>(result);
        }

        private readonly IParser<T> _parser;
    }
}
