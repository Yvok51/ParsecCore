using System;

namespace ParsecCore
{
    static class ParserExt
    {
        public static IParser<char> Whitespace = new SatisfyParser(char.IsWhiteSpace, "whitespace");
        public static IParser<char> Digit = new SatisfyParser(char.IsDigit, "digit");

        public static IParser<T> Return<T>(T value)
        {
            return new ReturnParser<T>(value);
        }

        public static IParser<TResult> Select<TSource, TResult>(
            this IParser<TSource> parser,
            Func<TSource, TResult> projection
        )
        {
            return new MapParser<TSource, TResult>(parser, projection);
        }

        public static IParser<TResult> SelectMany<TFirst, TSecond, TResult>(
            this IParser<TFirst> first,
            Func<TFirst, IParser<TSecond>> getSecond,
            Func<TFirst, TSecond, TResult> getResult)
        {
            return new BindParser<TFirst, TSecond, TResult>(first, getSecond, getResult);
        }

        public static IParser<T> Choice<T>(IParser<T> firstParser, IParser<T> secondParser)
        {
            return new ChoiceParser<T>(firstParser, secondParser);
        }

    }
}
