using ParsecCore.EitherNS;
using ParsecCore.Input;

namespace ParsecCore
{
    internal class ResultImpl<T, TInput> : IResult<T, TInput>
    {
        public ResultImpl(IEither<ParseError, T> parseResult, IParserInput<TInput> unconsumedInput)
        {
            UnconsumedInput = unconsumedInput;
            ParseResult = parseResult;
        }

        public IParserInput<TInput> UnconsumedInput { get; init; }

        public IEither<ParseError, T> ParseResult { get; init; }

        public bool IsError => ParseResult.IsError;

        public bool IsResult => ParseResult.IsResult;

        public ParseError Error => ParseResult.Error;

        T IResult<T, TInput>.Result => ParseResult.Result;
    }
}
