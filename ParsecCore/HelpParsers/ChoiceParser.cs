using System.Collections.Generic;

using ParsecCore.Input;
using ParsecCore.Either;

namespace ParsecCore
{
    class ChoiceParser<T> : IParser<T>
    {
        public ChoiceParser(params IParser<T>[] parsers)
        {
            _parsers = parsers;
        }

        public ChoiceParser(IEnumerable<IParser<T>> parsers)
        {
            _parsers = parsers;
        }

        public IEither<ParseError, T> Parse(IParserInput input)
        {
            Position initialPosition = input.Position;
            IEither<ParseError, T> parseResult = EitherExt.Error<ParseError, T>(new ParseError("No match found", initialPosition));
            foreach (var parser in _parsers)
            {
                parseResult = parser.Parse(input);
                if (parseResult.HasRight)
                {
                    return parseResult;
                }

                input.Seek(initialPosition);
            }

            return parseResult;
        }

        IEnumerable<IParser<T>> _parsers;
    }
}
