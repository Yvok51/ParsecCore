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
        public static Parser<IReadOnlyList<T>> Parser<T>(Parser<T> parser)
        {
            Parser<IMaybe<T>> optParser = parser.Optional();
            return (input) =>
            {
                List<T> result = new List<T>();

                var parseResult = optParser(input);
                while (parseResult.HasRight && !parseResult.Right.IsEmpty)
                {
                    result.Add(parseResult.Right.Value);
                    parseResult = optParser(input);
                }

                if (parseResult.HasLeft)
                {
                    return Either.Error<ParseError, IReadOnlyList<T>>(parseResult.Left);
                }

                return Either.Result<ParseError, IReadOnlyList<T>>(result);
            };
        }
    }
}
