using System.Collections.Generic;

using ParsecCore.EitherNS;

namespace ParsecCore
{
    class SepBy1Parser
    {
        public static Parser<IEnumerable<TValue>> Parser<TValue, TSeperator>(
            Parser<TValue> valueParser,
            Parser<TSeperator> seperatorParser
        )
        {
            var sepValueParser = (from sep in seperatorParser
                              from val in valueParser
                              select val).Optional();
            return (input) =>
            {
                List<TValue> result = new List<TValue>();

                // Not using Optional so that we propagate error upwards
                var initialPosition = input.Position;
                var firstParse = valueParser(input);
                if (firstParse.HasLeft)
                {
                    input.Seek(initialPosition);
                    return Either.Error<ParseError, IEnumerable<TValue>>(firstParse.Left);
                }

                result.Add(firstParse.Right);

                var additionalParse = sepValueParser(input);
                while (additionalParse.HasRight && !additionalParse.Right.IsEmpty)
                {
                    result.Add(additionalParse.Right.Value);
                    additionalParse = sepValueParser(input);
                }

                return Either.Result<ParseError, IEnumerable<TValue>>(result);
            };
        }
    }
}
