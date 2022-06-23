using System;
using System.Collections.Generic;
using System.Linq;

using ParsecCore;
using ParsecCore.Parsers;
using ParsecCore.MaybeNS;

namespace JSONParser.Parsers
{
    static class Parsers
    {
        private static IParser<string> whitespace =
            new SatisfyParser(c => c == ' ' || c == '\n' || c == '\t' || c == '\r', "whitespace").Many();

        private static IParser<string> valueSeparator = Parser.Symbol(",");

        ////////// BOOLEAN //////////
        private static IParser<bool> trueParser = 
            from _ in Parser.Symbol("true")
            select true;
        private static IParser<bool> falseParser = 
            from _ in Parser.Symbol("false")
            select false;
        public static IParser<bool> BoolParser = Parser.Choice(trueParser, falseParser);

        ////////// ARRAY //////////
        private static IParser<T> betweenBrackets<T>(IParser<T> betweenParser) =>
            Parser.Between(Parser.Symbol("["), betweenParser, Parser.Symbol("]"));

        private static IParser<IEnumerable<T>> ListOfParser<T>(IParser<T> valueParser) =>
            betweenBrackets(Parser.SepBy(valueParser, valueSeparator));

        ////////// OBJECT //////////
        private static IParser<T> betweenBraces<T>(IParser<T> betweenParser) =>
            Parser.Between(Parser.Symbol("{"), betweenParser, Parser.Symbol("}"));

        private static IParser<IEnumerable<T>> ObjectOfParser<T>(IParser<T> valueParser) =>
            betweenBraces(Parser.SepBy(valueParser, valueSeparator));

        ////////// NUMBER //////////
        private static IParser<string> zero = Parser.String("0");
        private static IParser<string> nonZeroInteger =
            from firstDigit in new SatisfyParser(c => Char.IsDigit(c) && c != '0', "non-zero digit")
            from nextDigits in Parser.Digit.Many()
            select firstDigit.ToString() + nextDigits;
        private static IParser<string> integer = Parser.Choice(zero, nonZeroInteger);

        private static IParser<string> minus = Parser.String("-");
        private static IParser<string> plus = Parser.String("+");
        private static IParser<string> plusOrMinus = Parser.Choice(plus, minus);
        
        private static IParser<string> decimalPoint = Parser.String(".");
        private static IParser<string> fractionalPart =
            from point in decimalPoint
            from digits in Parser.Digits
            select point + digits;

        private static IParser<string> exponentSymbol =
            Parser.Choice(Parser.String("e"), Parser.String("E"));
        private static IParser<string> exponent =
            from symbol in exponentSymbol
            from sign in plusOrMinus.Optional()
            from digits in Parser.Digits
            select  symbol + sign.Else("") + digits;

        public static IParser<double> number =
            from minus in minus.Optional()
            from integer in integer
            from frac in fractionalPart.Optional()
            from exp in exponent.Optional()
            select Double.Parse(minus.Else("") + integer + frac.Else("") + exp.Else(""));

        ////////// STRING //////////
        private static IParser<char> quote = new CharParser('\"');
        private static IParser<char> escape = new CharParser('\\');

        private static Dictionary<char, char> toEscaped = new Dictionary<char, char> 
        {
            { '"', '"' },
            { '\\', '\\' },
            { 'b', '\b' },
            { 'f', '\f' },
            { 'n', '\n' },
            { 'r', '\r' },
            { 't', '\t' },
        };
        static Func<IEnumerable<char>, IEnumerable<IParser<char>>> charsToParsers = (IEnumerable<char> chars) =>
        {
            List<IParser<char>> parsers = new List<IParser<char>>();
            foreach (var c in chars)
            {
                parsers.Add(new CharParser(c));
            }
            return parsers;
        };
        private static IParser<char> charToEscape = Parser.Choice(charsToParsers(toEscaped.Keys));
        private static IParser<char> escapedChar =
            from esc in escape
            from escapedChar in charToEscape
            select toEscaped[escapedChar];

    }
}
