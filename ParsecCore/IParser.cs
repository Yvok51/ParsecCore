using ParsecCore.Input;

namespace ParsecCore
{
    public interface IParser<T>
    {
        IEither<ParseError, T> Parse(IParserInput input);
    }
}
