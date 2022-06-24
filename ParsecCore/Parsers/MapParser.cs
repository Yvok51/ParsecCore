using System;

using ParsecCore.EitherNS;
using ParsecCore.Input;

namespace ParsecCore.Parsers
{
    /// <summary>
    /// Class used in the Select extension method to enable "do" notation 
    /// (`from ... in ...`) for a single variable
    /// Performs the equaivalent to the fmap function
    /// </summary>
    /// <typeparam name="TSource"> The result type of the provided parser </typeparam>
    /// <typeparam name="TResult"> The result type of the parser with the result value mapped by the given function </typeparam>
    class MapParser<TSource, TResult> : IParser<TResult>
    {
        public MapParser(IParser<TSource> sourceParser, Func<TSource, TResult> projection)
        {
            _sourceParser = sourceParser;
            _projection = projection;
        }

        public IEither<ParseError, TResult> Parse(IParserInput input)
        {
            return from value in _sourceParser.Parse(input)
                   select _projection(value);
        }

        private readonly IParser<TSource> _sourceParser;
        private readonly Func<TSource, TResult> _projection;
    }
}
