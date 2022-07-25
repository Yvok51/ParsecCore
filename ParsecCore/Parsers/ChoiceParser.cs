using ParsecCore.EitherNS;
using ParsecCore.Input;
using System.Collections.Generic;

namespace ParsecCore.ParsersHelp
{
    class ChoiceParser
    {
        public static Parser<T, TInputToken> Parser<T, TInputToken>(params Parser<T, TInputToken>[] parsers)
        {
            return Parser((IEnumerable<Parser<T, TInputToken>>)parsers);
        }

        public static Parser<T, TInputToken> Parser<T, TInputToken>(IEnumerable<Parser<T, TInputToken>> parsers)
        {
            return (input) =>
            {
                Position initialPosition = input.Position;
                IEither<ParseError, T> parseResult = Either.Error<ParseError, T>(new ParseError("No match found", initialPosition));
                foreach (var parser in parsers)
                {
                    parseResult = parser(input);
                    if (parseResult.IsResult || input.Position != initialPosition)
                    {
                        return parseResult;
                    }
                }

                return parseResult;
            };
        }
    }
}
