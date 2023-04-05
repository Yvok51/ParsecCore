using ParsecCore.EitherNS;
using ParsecCore.MaybeNS;
using System.Collections.Generic;

namespace ParsecCore.ParsersHelp
{
    /// <summary>
    /// Parser which takes a parser and attempts to apply it as many times as possible 
    /// Used in the implementation of the Many method.
    /// </summary>
    internal class ManyParser
    {
        public static Parser<IReadOnlyList<T>, TInputToken> Parser<T, TInputToken>(Parser<T, TInputToken> parser)
        {
            Parser<Maybe<T>, TInputToken> optParser = parser.Optional();
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
                    return Either.RetypeError<ParseError, Maybe<T>, IReadOnlyList<T>>(parseResult);
                }

                return Either.Result<ParseError, IReadOnlyList<T>>(result);
            };
        }
    }
}
