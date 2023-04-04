﻿using ParsecCore.EitherNS;
using System.Collections.Generic;

namespace ParsecCore.ParsersHelp
{
    internal class ManyTillParser
    {
        public static Parser<IReadOnlyList<T>, TInputToken> Parser<T, TEnd, TInputToken>(Parser<T, TInputToken> many, Parser<TEnd, TInputToken> till)
        {
            Parser<TEnd, TInputToken> tryTill = till.Try();
            return (input) =>
            {
                List<T> result = new List<T>();

                var tillResult = tryTill(input);
                while (tillResult.IsError)
                {
                    var manyResult = many(input);
                    if (manyResult.IsError)
                    {
                        return Either.RetypeError<ParseError, T, IReadOnlyList<T>>(manyResult);
                    }

                    result.Add(manyResult.Result);
                    tillResult = tryTill(input);
                }

                return Either.Result<ParseError, IReadOnlyList<T>>(result);
            };
        }
    }
}
