namespace ParsecCore
{
    public delegate IResult<T, TInput> Parser<out T, TInput>(IParserInput<TInput> input);
}
