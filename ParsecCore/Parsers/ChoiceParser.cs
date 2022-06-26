using System.Collections.Generic;

using ParsecCore.Input;
using ParsecCore.EitherNS;

namespace ParsecCore.ParsersHelp
{
    class ChoiceParser
    {
        public static Parser<T> Parser<T>(params Parser<T>[] parsers)
        {
            return Parser((IEnumerable<Parser<T>>)parsers);
        }

        public static Parser<T> Parser<T>(IEnumerable<Parser<T>> parsers)
        {
            return (input) =>
            {
                Position initialPosition = input.Position;
                IEither<ParseError, T> parseResult = Either.Error<ParseError, T>(new ParseError("No match found", initialPosition));
                foreach (var parser in parsers)
                {
                    parseResult = parser(input);
                    if (parseResult.HasRight)
                    {
                        return parseResult;
                    }

                    input.Seek(initialPosition);
                }

                return parseResult;
            };
        }
    }
}
