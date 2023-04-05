using ParsecCore.EitherNS;
using System.Collections.Generic;

namespace ParsecCore.ParsersHelp
{
    /// <summary>
    /// See <see cref="Parsers.ManyTill{T, TEnd, TInputToken}(Parser{T, TInputToken}, Parser{TEnd, TInputToken})"/>
    /// for details.
    /// </summary>
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
                    var position = input.Position;
                    tillResult = tryTill(input);
                    if (input.Position != position && tillResult.IsError)
                    {
                        return Either.RetypeError<ParseError, TEnd, IReadOnlyList<T>>(tillResult);
                    }
                }

                return Either.Result<ParseError, IReadOnlyList<T>>(result);
            };
        }
    }
}
