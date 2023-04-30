using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParsecCore
{
    public static partial class Result
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

        /// <summary>
        /// Create a succesful result made out of a previous result.
        /// </summary>
        /// <typeparam name="T"> The output type of the result </typeparam>
        /// <typeparam name="R"> The output type of the previous result </typeparam>
        /// <typeparam name="TInput"> The inut type of the parser </typeparam>
        /// <param name="result"> The value to be the output of the result </param>
        /// <param name="previousResult"> The previous result to construct this result out of </param>
        /// <returns> A new successful result made out of a previous result </returns>
        public static IResult<T, TInput> Success<T, R, TInput>(T result, IResult<R, TInput> previousResult)
        {
            return new Success<T, TInput>(result, previousResult.Error, previousResult.UnconsumedInput);
        }

        /// <summary>
        /// Create a successful result made out of two previous results.
        /// We first parsed <paramref name="firstResult"/> and afterward <paramref name="secondResult"/>.
        /// Now we create a successful result with the errors taken from these previous two results and combined.
        /// </summary>
        /// <typeparam name="T"> The output type of the result </typeparam>
        /// <typeparam name="R"> The output type of the first previous result </typeparam>
        /// <typeparam name="S"> The output type of the second previous result </typeparam>
        /// <typeparam name="TInput"> The input type of the parser </typeparam>
        /// <param name="result"> The value to be the output of the result </param>
        /// <param name="firstResult"> The first previous result to create the new result out of </param>
        /// <param name="secondResult"> The second previous result to create the new result out of </param>
        /// <returns> A successful result with the old error combined from two previous results </returns>
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

        /// <summary>
        /// Create a successful result made out of previous results.
        /// We applied a sequence of parsers and
        /// now we are creating a new result from the output of all of their results.
        /// We are combining their old errors together.
        /// </summary>
        /// <typeparam name="T"> The output type of the result </typeparam>
        /// <typeparam name="R"> The output type of the previous results </typeparam>
        /// <typeparam name="TInput"> The input type of the parser </typeparam>
        /// <param name="result"> The value to be the output of this result </param>
        /// <param name="pastResults">
        /// Results of previous parsers that we applied and the results we are combining 
        /// </param>
        /// <returns> A successful result combined from previous results </returns>
        public static IResult<T, TInput> Success<T, R, TInput>(
            T result,
            IReadOnlyList<IResult<R, TInput>> pastResults
        )
        {
            return new Success<T, TInput>(
                result,
                AggregateErrors(pastResults),
                pastResults[pastResults.Count - 1].UnconsumedInput
            );
        }

        /// <summary>
        /// Create a successful result made out of previous results.
        /// Same as <see cref="Success{T, R, TInput}(T, IReadOnlyList{IResult{R, TInput}})"/>
        /// but we specify the next input.
        /// </summary>
        /// <typeparam name="T"> The output type of the result </typeparam>
        /// <typeparam name="R"> The output type of the previous results </typeparam>
        /// <typeparam name="TInput"> The input type of the parser </typeparam>
        /// <param name="result"> The value to be the output of this result </param>
        /// <param name="pastResults">
        /// Results of previous parsers that we applied and the results we are combining 
        /// </param>
        /// <param name="unconsummedInput"> The input we are continuing with </param>
        /// <returns> A successful result combined from previous results </returns>
        public static IResult<T, TInput> Success<T, R, TInput>(
            T result,
            IReadOnlyList<IResult<R, TInput>> pastResults,
            IParserInput<TInput> unconsummedInput
)
        {
            return new Success<T, TInput>(result, AggregateErrors(pastResults), unconsummedInput);
        }

        /// <summary>
        /// Same as <see cref="Success{T, R, TInput}(T, IReadOnlyList{IResult{R, TInput}}, IParserInput{TInput})"/>
        /// but we add one single extra result.
        /// </summary>
        /// <typeparam name="T"> The output type of the result </typeparam>
        /// <typeparam name="R"> The output type of the previous results </typeparam>
        /// <typeparam name="S"> The output type of the extra result </typeparam>
        /// <typeparam name="TInput"></typeparam>
        /// <param name="result"> The value to be the output of this result </param>
        /// <param name="pastResults">
        /// Results of previous parsers that we applied and the results we are combining 
        /// </param>
        /// <param name="extra"> An extra result we are also using to create this new result </param>
        /// <param name="unconsummedInput"> The input we are continuing with </param>
        /// <returns> A successful result combined from previous results </returns>
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
                ParseError.CombineErrors(extra.Error, AggregateErrors(pastResults)),
                unconsummedInput
            );
        }

        /// <summary>
        /// Create a failed parse result.
        /// </summary>
        /// <typeparam name="T"> The result output type </typeparam>
        /// <typeparam name="TInput"> The input symbol type </typeparam>
        /// <param name="error"> The error to return </param>
        /// <param name="unconsumedInput"> Unconsummed input </param>
        /// <returns> Result of a parse signifying a failure </returns>
        public static IResult<T, TInput> Failure<T, TInput>(ParseError error, IParserInput<TInput> unconsumedInput)
        {
            return new Failure<T, TInput>(error, unconsumedInput);
        }

        /// <summary>
        /// Create a failed error out of two previous results
        /// We first parsed <paramref name="first"/> and afterward <paramref name="second"/>.
        /// Now we create a failed result with the errors taken from these previous two results and combined.
        /// At least one of the results shall be failed result.
        /// </summary>
        /// <typeparam name="T"> The output type of the result </typeparam>
        /// <typeparam name="R"> The output type of the first previous result </typeparam>
        /// <typeparam name="S"> The output type of the second previous result </typeparam>
        /// <typeparam name="TInput"> The input symbol type of the parser </typeparam>
        /// <param name="first"> The first result to combine </param>
        /// <param name="second"> The second result to combine </param>
        /// <returns> A failed result with the errors combined </returns>
        public static IResult<T, TInput> Failure<T, R, S, TInput>(IResult<R, TInput> first, IResult<S, TInput> second)
        {
            return new Failure<T, TInput>(
                ParseError.CombineErrors(first.Error, second.Error)!,
                second.UnconsumedInput
            );
        }

        /// <summary>
        /// Create a failed error out of previous results
        /// Now we create a failed result with the errors taken from these previous results and combined.
        /// At least one of the results shall be a failed result.
        /// </summary>
        /// <typeparam name="T"> The output type of the result </typeparam>
        /// <typeparam name="R"> The output type of the previous results </typeparam>
        /// <typeparam name="TInput"> The input symbol type of the parser </typeparam>
        /// <param name="results"> The previous results whose errors to combine </param>
        /// <returns> A failed result with the errors combined </returns>
        public static IResult<T, TInput> Failure<T, R, TInput>(IReadOnlyList<IResult<R, TInput>> results)
        {
            return new Failure<T, TInput>(AggregateErrors(results)!, results[results.Count - 1].UnconsumedInput);
        }

        /// <summary>
        /// Same as <see cref="Failure{T, R, TInput}(IReadOnlyList{IResult{R, TInput}})"/>
        /// but we add one extra result. We also specify which <paramref name="unconsummedInput"/> to use.
        /// </summary>
        /// <typeparam name="T"> The output type of the result </typeparam>
        /// <typeparam name="R"> The output type of the previous results </typeparam>
        /// <typeparam name="S"> The output type of the extra result </typeparam>
        /// <typeparam name="TInput"> The input symbol type of the parser </typeparam>
        /// <param name="results"> The previous results whose errors to combine </param>
        /// <param name="extra"> The extra result to combine also </param>
        /// <param name="unconsummedInput"> The unconsummed input to report </param>
        /// <returns> A failed result with the errors combined </returns>
        public static IResult<T, TInput> Failure<T, R, S, TInput>(
            IReadOnlyList<IResult<R, TInput>> results,
            IResult<S, TInput> extra,
            IParserInput<TInput> unconsummedInput
        )
        {
            return new Failure<T, TInput>(
                ParseError.CombineErrors(extra.Error, AggregateErrors(results))!,
                unconsummedInput
            );
        }

        /// <summary>
        /// Same as <see cref=
        /// "Failure{T, R, S, TInput}(IReadOnlyList{IResult{R, TInput}}, IResult{S, TInput}, IParserInput{TInput})"
        /// /> but instead of a result we add an extra error.
        /// </summary>
        /// <typeparam name="T"> The output type of the result </typeparam>
        /// <typeparam name="R"> The output type of the previous results </typeparam>
        /// <typeparam name="TInput"> The input symbol type of the parser </typeparam>
        /// <param name="results"> The previous results whose errors to combine </param>
        /// <param name="extra"> The extra error to combine also </param>
        /// <param name="unconsummedInput"> The unconsummed input to report </param>
        /// <returns> A failed result with the errors combined </returns>
        public static IResult<T, TInput> Failure<T, R, TInput>(
            IReadOnlyList<IResult<R, TInput>> results,
            ParseError extra,
            IParserInput<TInput> unconsummedInput
        )
        {
            return new Failure<T, TInput>(
                ParseError.CombineErrors(extra, AggregateErrors(results))!,
                unconsummedInput
            );
        }

        private static ParseError? AggregateErrors<T, TInput>(IReadOnlyList<IResult<T, TInput>> results)
        {
            ParseError? error = null;
            for (int i = results.Count - 1; i >= 0; i--)
            {
                error = ParseError.CombineErrors(error, results[i].Error);
            }
            return error;
        }
    }
}
