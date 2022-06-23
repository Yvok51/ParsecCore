using System;

using ParsecCore.EitherNS;
using ParsecCore.Input;

namespace ParsecCore.Parsers
{
    /// <summary>
    /// Parser which consumes a single character and succeeds if the character passes a predicate
    /// </summary>
    public class SatisfyParser : IParser<char>
    {
        public SatisfyParser(Predicate<char> predicate, string predicateDescription)
        {
            _predicate = predicate;
            _description = predicateDescription;
        }

        public IEither<ParseError, char> Parse(IParserInput input)
        {
            if (input.EndOfInput)
            {
                return Either.Error<ParseError, char>(new ParseError("Unexpected end of file, character expected", input.Position));
            }

            char c = input.Read();
            if (!_predicate(c)) 
            {
                return Either.Error<ParseError, char>(new ParseError($"character '{c}' does not conform, {_description} exprected", input.Position));
            }

            return Either.Result<ParseError, char>(c);
        }

        private readonly Predicate<char> _predicate;
        private readonly string _description;
    }
}
