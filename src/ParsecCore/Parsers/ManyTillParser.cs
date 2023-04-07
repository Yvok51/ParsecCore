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
                    if (!input.Equals(tillResult.UnconsumedInput))
                    {
                        return Result.RetypeError<TEnd, IReadOnlyList<T>, TInputToken>(tillResult);
                    }
                    var manyResult = many(input);
                    input = manyResult.UnconsumedInput;
                    if (manyResult.IsError)
                    {
                        return Result.RetypeError<T, IReadOnlyList<T>, TInputToken>(manyResult);
                    }
                    result.Add(manyResult.Result);

                    tillResult = tryTill(input);
                }

                return Result.Success(result, tillResult.UnconsumedInput);
            };
        }
    }
}
