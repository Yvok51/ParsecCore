using System;

using ParsecCore.Either;

namespace ParsecCore
{
    class BindParser<TFirst, TSecond, TResult> : IParser<TResult>
    {
        public BindParser(
            IParser<TFirst> first,
            Func<TFirst, IParser<TSecond>> getSecond,
            Func<TFirst, TSecond, TResult> getResult
        )
        {
            _first = first;
            _getSecond = getSecond;
            _getResult = getResult;
        }
        public IEither<ParseError, TResult> Parse(IParserInput input)
        {
            return from firstValue in _first.Parse(input)
                   from secondValue in _getSecond(firstValue).Parse(input)
                   select _getResult(firstValue, secondValue);
        }

        private readonly IParser<TFirst> _first;
        private readonly Func<TFirst, IParser<TSecond>> _getSecond;
        private readonly Func<TFirst, TSecond, TResult> _getResult;
    }
}
