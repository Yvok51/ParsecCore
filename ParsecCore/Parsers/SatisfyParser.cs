using System;

using ParsecCore.Either;

namespace ParsecCore
{
    /// <summary>
    /// Parser which consumes a single character and succeeds if the character passes a predicate
    /// </summary>
    class SatisfyParser : IParser<char>
    {
        public SatisfyParser(Predicate<char> predicate)
        {
            _predicate = predicate;
        }

        public IEither<ParseError, char> Parse(IParserInput input)
        {
            if (input.EndOfInput)
            {
                return EitherExt.Error<ParseError, char>(new ParseError("Unexpected end of file, character expected", input.LineNumber));
            }

            char c = input.Read();
            if (!_predicate(c)) 
            {
                return EitherExt.Error<ParseError, char>(new ParseError($"character '{c}' does not conform", input.LineNumber));
            }

            return EitherExt.Result<ParseError, char>(c);
        }

        private readonly Predicate<char> _predicate;
    }
}
