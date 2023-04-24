using ParsecCore.Help;
using System;
using System.Collections.Generic;

namespace ParsecCore
{
    public static class Result
    {
        /// <summary>
        /// Create a successful parser result.
        /// </summary>
        /// <typeparam name="T"> The output type of the result </typeparam>
        /// <typeparam name="TInput"> The parser input symbol type </typeparam>
        /// <param name="result"> The output of the result </param>
        /// <param name="oldError"> Any old error that occured and has been 'erased' </param>
        /// <param name="unconsumedInput"> Unconsumed input </param>
        /// <returns> Result of a parse signifying a success </returns>
        public static IResult<T, TInput> Success<T, TInput>(T result, ParseError? oldError, IParserInput<TInput> unconsumedInput)
        {
            return new Success<T, TInput>(result, oldError, unconsumedInput);
        }

        public static IResult<T, TInput> Success<T, R, TInput>(T result, IResult<R, TInput> previousResult)
        {
            return new Success<T, TInput>(result, previousResult.Error, previousResult.UnconsumedInput);
        }

        public static IResult<T, TInput> Success<T, R, S, TInput>(
            T result,
            IResult<R, TInput> firstResult,
            IResult<S, TInput> secondResult
        )
        {
            return new Success<T, TInput>(
                result,
                ParseError.CombineErrors(firstResult.Error, secondResult.Error),
                secondResult.UnconsumedInput
            );
        }

        public static IResult<T, TInput> Success<T, R, TInput>(
            T result,
            IReadOnlyList<IResult<R, TInput>> pastResults
        )
        {
            Func<ParseError?, IResult<R, TInput>, ParseError?> aggregateErrors =
                (error, res) => ParseError.CombineErrors(error, res.Error);
            return new Success<T, TInput>(
                result,
                pastResults.RightAggregate(aggregateErrors, null),
                pastResults[pastResults.Count - 1].UnconsumedInput
            );
        }

        public static IResult<T, TInput> Success<T, R, TInput>(
            T result,
            IReadOnlyList<IResult<R, TInput>> pastResults,
            IParserInput<TInput> unconsummedInput
)
        {
            Func<ParseError?, IResult<R, TInput>, ParseError?> aggregateErrors =
                (error, res) => ParseError.CombineErrors(error, res.Error);
            return new Success<T, TInput>(
                result,
                pastResults.RightAggregate(aggregateErrors, null),
                unconsummedInput
            );
        }

        public static IResult<T, TInput> Success<T, R, S, TInput>(
            T result,
            IReadOnlyList<IResult<R, TInput>> pastResults,
            IResult<S, TInput> extra,
            IParserInput<TInput> unconsummedInput
        )
        {
            Func<ParseError?, IResult<R, TInput>, ParseError?> aggregateErrors =
                (error, res) => ParseError.CombineErrors(error, res.Error);
            return new Success<T, TInput>(
                result,
                ParseError.CombineErrors(extra.Error, pastResults.RightAggregate(aggregateErrors, null)),
                unconsummedInput
            );
        }

        public static IResult<T, TInput> Failure<T, TInput>(ParseError error, IParserInput<TInput> unconsumedInput)
        {
            return new Failure<T, TInput>(error, unconsumedInput);
        }

        public static IResult<T, TInput> Failure<T, R, S, TInput>(IResult<R, TInput> first, IResult<S, TInput> second)
        {
            return new Failure<T, TInput>(
                ParseError.CombineErrors(first.Error, second.Error)!,
                second.UnconsumedInput
            );
        }

        public static IResult<T, TInput> Failure<T, R, TInput>(IReadOnlyList<IResult<R, TInput>> results)
        {
            Func<ParseError?, IResult<R, TInput>, ParseError?> aggregateErrors =
                (error, res) => ParseError.CombineErrors(error, res.Error);
            return new Failure<T, TInput>(
                results.RightAggregate(aggregateErrors, null)!,
                results[results.Count - 1].UnconsumedInput
            );
        }

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
