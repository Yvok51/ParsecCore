using ParsecCore.EitherNS;
using ParsecCore.Input;

namespace ParsecCore
{
    public delegate IEither<ParseError, T> Parser<out T, TInput>(IParserInput<TInput> input);
}
