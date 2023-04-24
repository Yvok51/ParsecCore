using System;
using System.Collections.Generic;

namespace ParsecCore
{
    public static partial class Result
    {

        public static IResult<TNew, TInput> RetypeError<T, TNew, TInput>(
            IResult<T, TInput> result
        )
        {
            return new Failure<TNew, TInput>(
                result.Error!,
                result.UnconsumedInput
            );
        }

        public static IResult<T, TInput> CombineErrors<T, TInput>(
            this IResult<T, TInput> left,
            IResult<T, TInput> right
        )
        {
            if (left.Error!.Position > right.Error!.Position)
            {
                return left;
            }
            else if (left.Error.Position < right.Error.Position)
            {
                return right;
            }
            return Failure<T, TInput>(left.Error.Accept(right.Error, None.Instance), left.UnconsumedInput);
        }

        public static TNew Match<T, TNew, TInput>(
            this IResult<T, TInput> result,
            Func<T, TNew> success,
            Func<ParseError, TNew> failure
        )
        {
            if (result.IsError)
            {
                return failure(result.Error!);
            }
            else
            {
                return success(result.Result);
            }
        }

        public static IResult<TNew, TInput> Map<T, TNew, TInput>(
            this IResult<T, TInput> result,
            Func<T, TNew> map
        )
        {
            if (result.IsError)
            {
                return RetypeError<T, TNew, TInput>(result);
            }
            else
            {
                return Success(map(result.Result), result);
            }
        }
    }
}
