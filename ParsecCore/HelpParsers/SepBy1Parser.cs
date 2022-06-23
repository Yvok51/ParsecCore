using System;
using System.Collections.Generic;

using ParsecCore.EitherNS;
using ParsecCore.Input;
using ParsecCore.MaybeNS;

namespace ParsecCore.HelpParsers
{
    class SepBy1Parser<TValue, TSeperator> : IParser<IEnumerable<TValue>>
    {
        public SepBy1Parser(IParser<TValue> valueParser, IParser<TSeperator> seperatorParser)
        {
            _valueParser = valueParser;
            _sepValueParser = (from sep in seperatorParser
                              from val in valueParser
                              select val).Optional();
        }

        public IEither<ParseError, IEnumerable<TValue>> Parse(IParserInput input)
        {
            List<TValue> result = new List<TValue>();

            // Not using Optional so that we propagate error upwards
            var initialPosition = input.Position;
            var firstParse = _valueParser.Parse(input);
            if (firstParse.HasLeft) 
            {
                input.Seek(initialPosition);
                return Either.Error<ParseError, IEnumerable<TValue>>(firstParse.Left);
            }

            result.Add(firstParse.Right);

            var additionalParse = _sepValueParser.Parse(input);
            while (additionalParse.HasRight && !additionalParse.Right.IsEmpty)
            {
                result.Add(additionalParse.Right.Value);
                additionalParse = _sepValueParser.Parse(input);
            }

            return Either.Result<ParseError, IEnumerable<TValue>>(result);
        }

        private readonly IParser<TValue> _valueParser;
        private readonly IParser<IMaybe<TValue>> _sepValueParser;
    }
}
