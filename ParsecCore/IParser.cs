using ParsecCore.Input;
using ParsecCore.EitherNS;

namespace ParsecCore
{
    public interface IParser<out T>
    {
        IEither<ParseError, T> Parse(IParserInput input);
    }
}
