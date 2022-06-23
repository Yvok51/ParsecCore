using ParsecCore.Input;
using ParsecCore.EitherNS;

namespace ParsecCore
{
    public interface IParser<T>
    {
        IEither<ParseError, T> Parse(IParserInput input);
    }
}
