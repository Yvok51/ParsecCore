using System.Collections.Generic;

using ParsecCore.Input;
using ParsecCore.EitherNS;
using ParsecCore.MaybeNS;

namespace ParsecCore.Parsers
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
            _parser = parser.Optional();
        }

        public IEither<ParseError, IEnumerable<T>> Parse(IParserInput input)
        {
            List<T> result = new List<T>();

            var parseResult = _parser.Parse(input);
            while (parseResult.HasRight && !parseResult.Right.IsEmpty)
            {
                result.Add(parseResult.Right.Value);
                parseResult = _parser.Parse(input);
            }

            return Either.Result<ParseError, IEnumerable<T>>(result);
        }

        private readonly IParser<IMaybe<T>> _parser;
    }
}
