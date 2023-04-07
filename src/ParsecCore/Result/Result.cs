using ParsecCore.EitherNS;
using ParsecCore.Input;
using System;

namespace ParsecCore
{
    public static class Result
    {
        public static IResult<T, TInput> Success<T, TInput>(T result, IParserInput<TInput> unconsumedInput)
        {
            return new ResultImpl<T, TInput>(result, unconsumedInput);
        }

        public static IResult<T, TInput> Failure<T, TInput>(ParseError error, IParserInput<TInput> unconsumedInput)
        {
            return new ResultImpl<T, TInput>(error, unconsumedInput);
        }

        //public static IResult<T, TInput> Create<T, TInput>(IEither<ParseError, T> parseResult, IParserInput<TInput> unconsumedInput)
        //{
        //    return new ResultImpl<T, TInput>(parseResult, unconsumedInput);
        //}

        public static IResult<TNew, TInput> RetypeError<T, TNew, TInput>(
            IResult<T, TInput> result
        )
        {
            return new ResultImpl<TNew, TInput>(
                result.Error,
                result.UnconsumedInput
            );
        }

        public static IResult<T, TInput> CombineErrors<T, TInput>(this IResult<T, TInput> left, IResult<T, TInput> right)
        {
            if (left.Error.Position > right.Error.Position)
            {
                return left;
            }
            else if (left.Error.Position < right.Error.Position)
            {
                return right;
            }
            return Failure<T, TInput>(left.Error.Accept(right.Error, new None()), left.UnconsumedInput);
        }

        public static TNew Match<T, TNew, TInput>(
            this IResult<T, TInput> result,
            Func<T, TNew> success,
            Func<ParseError, TNew> failure
        )
        {
            if (result.IsError)
            {
                return failure(result.Error);
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
                return Result.Success(map(result.Result), result.UnconsumedInput);
            }
        }
    }
}
