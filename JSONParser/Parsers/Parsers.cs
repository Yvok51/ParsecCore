using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

using ParsecCore;
using ParsecCore.MaybeNS;

namespace JSONParser
{
    /// <summary>
    /// Parsers for JSON.
    /// Based on this RFC: https://datatracker.ietf.org/doc/html/rfc8259
    /// </summary>
    static class Parsers
    {
        // We don't use Parser.Spaces since the its definition of whitespace is different
        // to the definition found in the JSON RFC.
        // Subsequently we also don't use Parser.Token and Parser.Symbol but instead Combinator.Between
        private static readonly IParser<string> whitespace =
            Parser.Satisfy(c => c == ' ' || c == '\n' || c == '\t' || c == '\r', "whitespace").Many();

        private static readonly IParser<string> valueSeparator = Parser.Symbol(",");

        ////////// NULL //////////
        private static readonly IParser<NullValue> NullValue =
            from _ in Parser.Symbol("null")
            select new NullValue();

        ////////// BOOLEAN //////////
        private static readonly IParser<bool> trueParser =
            from _ in Parser.Symbol("true")
            select true;
        private static readonly IParser<bool> falseParser =
            from _ in Parser.Symbol("false")
            select false;
        private static readonly IParser<bool> Boolean = Combinator.Choice(trueParser, falseParser);
        public static IParser<BoolValue> BoolValue =
            from b in Boolean
            select new BoolValue(b);

        ////////// NUMBER //////////
        private static readonly IParser<string> zero = Parser.String("0");
        private static readonly IParser<string> nonZeroInteger =
            from firstDigit in Parser.Satisfy(c => Char.IsDigit(c) && c != '0', "non-zero digit")
            from nextDigits in Parser.Digit.Many()
            select firstDigit.ToString() + nextDigits;
        private static readonly IParser<string> integer = Combinator.Choice(zero, nonZeroInteger);

        private static readonly IParser<string> minus = Parser.String("-");
        private static readonly IParser<string> plus = Parser.String("+");
        private static readonly IParser<string> plusOrMinus = Combinator.Choice(plus, minus);

        private static readonly IParser<string> decimalPoint = Parser.String(".");
        private static readonly IParser<string> fractionalPart =
            from point in decimalPoint
            from digits in Parser.Digits
            select point + digits;

        private static readonly IParser<string> exponentSymbol =
            Combinator.Choice(Parser.String("e"), Parser.String("E"));
        private static readonly IParser<string> exponent =
            from symbol in exponentSymbol
            from sign in plusOrMinus.Optional()
            from digits in Parser.Digits
            select symbol + sign.Else("") + digits;

        private static readonly CultureInfo USCulture = new CultureInfo("en-US");
        private static readonly IParser<double> Number =
            from minus in minus.Optional()
            from integer in integer
            from frac in fractionalPart.Optional()
            from exp in exponent.Optional()
            select Double.Parse(minus.Else("") + integer + frac.Else("") + exp.Else(""), USCulture);

        public static readonly IParser<NumberValue> NumberValue =
            from n in Number
            select new NumberValue(n);

        ////////// STRING //////////
        private static readonly IParser<char> quote = Parser.Char('\"');
        private static readonly IParser<char> escape = Parser.Char('\\');

        private static readonly IParser<char> hexadecimalDigit = Parser.Satisfy(
            c => Char.IsDigit(c) || c == 'A' || c == 'B' || c == 'C' || c == 'D' || c == 'E' || c == 'F',
            "hexadecimal digit"
        );
        private static readonly IParser<char> hexEncoded =
            from _ in escape
            from __ in Parser.Char('u')
            from first in hexadecimalDigit
            from second in hexadecimalDigit
            from third in hexadecimalDigit
            from fourth in hexadecimalDigit
            select (char)Int32.Parse(
                new string(new char[] { first, second, third, fourth }),
                NumberStyles.AllowHexSpecifier
            );

        private static readonly IParser<char> nonQouteChar = Parser.Satisfy(c => c != '"', "non-quote character");

        private static readonly Dictionary<char, char> toEscaped = new Dictionary<char, char>
        {
            { '"', '"' },
            { '\\', '\\' },
            { 'b', '\b' },
            { 'f', '\f' },
            { 'n', '\n' },
            { 'r', '\r' },
            { 't', '\t' },
        };
        private static IEnumerable<IParser<char>> charsToParsers(IEnumerable<char> chars)
        {
            List<IParser<char>> parsers = new List<IParser<char>>();
            foreach (var c in chars)
            {
                parsers.Add(Parser.Char(c));
            }
            return parsers;
        }

        private static readonly IParser<char> charToEscape = Combinator.Choice(charsToParsers(toEscaped.Keys));
        private static readonly IParser<char> escapedChar =
            from esc in escape
            from escapedChar in charToEscape
            select toEscaped[escapedChar];

        private static readonly IParser<char> escaped = Combinator.Choice(hexEncoded, escapedChar);

        private static readonly IParser<char> stringChar = Combinator.Choice(escaped, nonQouteChar);
        private static readonly IParser<string> String = Combinator.Between(quote, stringChar.Many());

        public static readonly IParser<StringValue> StringValue =
            from str in String
            select new StringValue(str);

        ////////// VALUE //////////
        public static readonly IParser<JsonValue> JsonValue =
            Combinator.Between(whitespace, Combinator.Choice<JsonValue>(NullValue, BoolValue, NumberValue, StringValue, ArrayValue, ObjectValue));

        ////////// ARRAY //////////
        private static IParser<T> betweenBrackets<T>(IParser<T> betweenParser) =>
            Combinator.Between(Parser.Symbol("["), betweenParser, Parser.Symbol("]"));

        private static IParser<IEnumerable<T>> ListOfParser<T>(IParser<T> valueParser) =>
            betweenBrackets(Combinator.SepBy(valueParser, valueSeparator));

        public static readonly IParser<ArrayValue> ArrayValue =
            from values in ListOfParser(JsonValue)
            select new ArrayValue(values);

        ////////// OBJECT //////////
        private static IParser<T> betweenBraces<T>(IParser<T> betweenParser) =>
            Combinator.Between(Parser.Symbol("{"), betweenParser, Parser.Symbol("}"));

        private static IParser<IEnumerable<T>> ObjectOfParser<T>(IParser<T> valueParser) =>
            betweenBraces(Combinator.SepBy(valueParser, valueSeparator));

        private static readonly IParser<string> nameSeperator = Combinator.Between(whitespace, Parser.String(":"));
        private static readonly IParser<ObjectKeyValuePair> member =
            from key in Combinator.Between(whitespace, StringValue)
            from sep in nameSeperator
            from value in JsonValue
            select new ObjectKeyValuePair { Key = key, Value = value };

        public static readonly IParser<ObjectValue> ObjectValue =
            from members in ObjectOfParser(member)
            select new ObjectValue(members);
    }
}
