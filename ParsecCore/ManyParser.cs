using System.Text;
using System.Collections.Generic;

using ParsecCore.Input;
using ParsecCore.Either;

namespace ParsecCore
{
    class ManyParser<T> : IParser<IEnumerable<T>>
    {
        public ManyParser(IParser<T> parser)
        {
            _parser = parser;
        }

        public IEither<ParseError, IEnumerable<T>> Parse(IParserInput input)
        {
            List<T> result = new List<T>();

            var previousPosition = input.Position;
            var parseResult = _parser.Parse(input);
            while (parseResult.HasRight)
            {
                result.Add(parseResult.Right);
                previousPosition = input.Position;
                parseResult = _parser.Parse(input);
            }

            input.Seek(previousPosition);
            return EitherExt.Result<ParseError, IEnumerable<T>>(result);
        }

        private readonly IParser<T> _parser;
    }

    class ManyParser : IParser<string>
    {
        public ManyParser(IParser<char> parser)
        {
            _parser = parser;
        }

        public IEither<ParseError, string> Parse(IParserInput input)
        {
            StringBuilder result = new StringBuilder();

            var previousPosition = input.Position;
            var parseResult = _parser.Parse(input);
            while (parseResult.HasRight)
            {
                result.Append(parseResult.Right);
                previousPosition = input.Position;
                parseResult = _parser.Parse(input);
            }

            input.Seek(previousPosition);
            return EitherExt.Result<ParseError, string>(result.ToString());
        }

        private readonly IParser<char> _parser;
    }
}
