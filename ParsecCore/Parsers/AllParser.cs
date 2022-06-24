using System.Collections.Generic;

using ParsecCore.Input;
using ParsecCore.EitherNS;

namespace ParsecCore.Parsers
{
    /// <summary>
    /// Parser tries to parse all of the given parsers in a sequence
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class AllParser<T> : IParser<IEnumerable<T>>
    {
        public AllParser(params IParser<T>[] parsers)
        {
            _parsers = parsers;    
        }

        public AllParser(IEnumerable<IParser<T>> parsers)
        {
            _parsers = parsers;
        }

        public IEither<ParseError, IEnumerable<T>> Parse(IParserInput input)
        {
            var initialPosition = input.Position;
            List<T> result = new List<T>();
            
            foreach (var parser in _parsers)
            {
                var parsedResult = parser.Parse(input);
                if (parsedResult.HasLeft)
                {
                    input.Seek(initialPosition);
                    return Either.Error<ParseError, IEnumerable<T>>(parsedResult.Left);
                }

                result.Add(parsedResult.Right);
            }

            return Either.Result<ParseError, IEnumerable<T>>(result);
        }

        private readonly IEnumerable<IParser<T>> _parsers; 
    }
}
