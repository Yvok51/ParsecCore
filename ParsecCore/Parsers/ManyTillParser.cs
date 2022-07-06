using ParsecCore.EitherNS;
using System.Collections.Generic;

namespace ParsecCore.ParsersHelp
{
    class ManyTillParser
    {
        public static Parser<IReadOnlyList<T>> Parser<T, TEnd>(Parser<T> many, Parser<TEnd> till)
        {
            Parser<TEnd> tryTill = till.Try();
            return (input) =>
            {
                List<T> result = new List<T>();

                var tillResult = tryTill(input);
                while (tillResult.IsError)
                {
                    var manyResult = many(input);
                    if (manyResult.IsError)
                    {
                        return Either.Error<ParseError, IReadOnlyList<T>>(manyResult.Error);
                    }

                    result.Add(manyResult.Result);
                    tillResult = tryTill(input);
                }

                return Either.Result<ParseError, IReadOnlyList<T>>(result);
            };
        }
    }
}
