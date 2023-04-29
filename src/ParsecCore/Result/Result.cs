using System;

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

        /// <summary>
        /// Combine the errors of two results.
        /// Presumes both results are failures otherwise might throw an <see cref="NullReferenceException"/>.
        /// The result with the more specific error is chosen.
        /// The specificity of the errors is given first by the location where they occured.
        /// The further along in the input the more specific the error is.
        /// Second, by the type of the error. <see cref="StandardError"/> is less specific than <see cref="CustomError"/>.
        /// </summary>
        /// <typeparam name="T"> The output type of the results </typeparam>
        /// <typeparam name="TInput"> The type of the input symbols </typeparam>
        /// <param name="left"> The result to combine </param>
        /// <param name="right"> The result to combine </param>
        /// <returns> Result whose errors are combined </returns>
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

        /// <summary>
        /// Return a value based on the content of the result.
        /// </summary>
        /// <typeparam name="T"> The output type of the result </typeparam>
        /// <typeparam name="TNew"> The type of the value returned </typeparam>
        /// <typeparam name="TInput"> The type of the input symbols </typeparam>
        /// <param name="result"> The result whose value to use </param>
        /// <param name="success">
        /// Function describing how to obtain the value in case the result represents a successful parse
        /// </param>
        /// <param name="failure">
        /// Function describing how to obtain the value in case the result represents a failed parse
        /// </param>
        /// <returns> A new value based on the result </returns>
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

        /// <summary>
        /// Map the output value of a result.
        /// If the <paramref name="result"/> represents a failure, then nothing happens
        /// </summary>
        /// <typeparam name="T"> The output type of the result </typeparam>
        /// <typeparam name="TNew"> The new output type of the result </typeparam>
        /// <typeparam name="TInput"> The type of the input symbols </typeparam>
        /// <param name="result"> The result whose value to map </param>
        /// <param name="map"> The function to map the value with </param>
        /// <returns> Result with the output value mapped </returns>
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
