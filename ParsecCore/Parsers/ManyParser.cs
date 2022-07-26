using ParsecCore.EitherNS;
using ParsecCore.MaybeNS;
using System.Collections.Generic;

namespace ParsecCore.ParsersHelp
{
    /// <summary>
    /// Parser which takes a parser and attempts to apply it as many times as possible 
    /// Used in the implementation of the Many method
    /// </summary>
    /// <typeparam name="T"> The type of parser return value </typeparam>
    class ManyParser
    {
        public static Parser<IReadOnlyList<T>, TInputToken> Parser<T, TInputToken>(Parser<T, TInputToken> parser)
        {
            Parser<IMaybe<T>, TInputToken> optParser = parser.Optional();
            return (input) =>
            {
                List<T> result = new List<T>();

                var parseResult = optParser(input);
                while (parseResult.IsResult && !parseResult.Result.IsEmpty)
                {
                    result.Add(parseResult.Result.Value);
                    parseResult = optParser(input);
                }

                if (parseResult.IsError)
                {
                    return Either.Error<ParseError, IReadOnlyList<T>>(parseResult.Error);
                }

                return Either.Result<ParseError, IReadOnlyList<T>>(result);
            };
        }
    }
}
