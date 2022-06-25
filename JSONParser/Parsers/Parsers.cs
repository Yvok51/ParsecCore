﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

using ParsecCore;
using ParsecCore.MaybeNS;

namespace JSONParser
{
    static class Parsers
    {
        private static IParser<string> whitespace =
            Parser.Satisfy(c => c == ' ' || c == '\n' || c == '\t' || c == '\r', "whitespace").Many();

        private static IParser<string> valueSeparator = Parser.Symbol(",");

        ////////// NULL //////////
        private static IParser<NullValue> NullValue =
            from _ in Parser.Symbol("null")
            select new NullValue();

        ////////// BOOLEAN //////////
        private static IParser<bool> trueParser =
            from _ in Parser.Symbol("true")
            select true;
        private static IParser<bool> falseParser =
            from _ in Parser.Symbol("false")
            select false;
        private static IParser<bool> Boolean = Parser.Choice(trueParser, falseParser);
        public static IParser<BoolValue> BoolValue =
            from b in Boolean
            select new BoolValue(b);

        ////////// NUMBER //////////
        private static IParser<string> zero = Parser.String("0");
        private static IParser<string> nonZeroInteger =
            from firstDigit in Parser.Satisfy(c => Char.IsDigit(c) && c != '0', "non-zero digit")
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
            select symbol + sign.Else("") + digits;

        private static CultureInfo USCulture = new CultureInfo("en-US");
        private static IParser<double> Number =
            from minus in minus.Optional()
            from integer in integer
            from frac in fractionalPart.Optional()
            from exp in exponent.Optional()
            select Double.Parse(minus.Else("") + integer + frac.Else("") + exp.Else(""), USCulture);

        public static IParser<NumberValue> NumberValue =
            from n in Number
            select new NumberValue(n);

        ////////// STRING //////////
        private static IParser<char> quote = Parser.Char('\"');
        private static IParser<char> escape = Parser.Char('\\');

        private static IParser<char> hexadecimalDigit = Parser.Satisfy(
            c => Char.IsDigit(c) || c == 'A' || c == 'B' || c == 'C' || c == 'D' || c == 'E' || c == 'F',
            "hexadecimal digit"
        );
        private static IParser<char> hexEncoded =
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

        private static IParser<char> nonQouteChar = Parser.Satisfy(c => c != '"', "non-quote character");

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
        private static IEnumerable<IParser<char>> charsToParsers(IEnumerable<char> chars)
        {
            List<IParser<char>> parsers = new List<IParser<char>>();
            foreach (var c in chars)
            {
                parsers.Add(Parser.Char(c));
            }
            return parsers;
        }

        private static IParser<char> charToEscape = Parser.Choice(charsToParsers(toEscaped.Keys));
        private static IParser<char> escapedChar =
            from esc in escape
            from escapedChar in charToEscape
            select toEscaped[escapedChar];

        private static IParser<char> escaped = Parser.Choice(hexEncoded, escapedChar);

        private static IParser<char> stringChar = Parser.Choice(escaped, nonQouteChar);
        private static IParser<string> String = Parser.Between(quote, stringChar.Many());

        public static IParser<StringValue> StringValue =
            from str in String
            select new StringValue(str);

        ////////// VALUE //////////
        public static IParser<JsonValue> JsonValue =
            from _ in whitespace
            from value in Parser.Choice<JsonValue>(NullValue, BoolValue, NumberValue, StringValue, ArrayValue, ObjectValue)
            from __ in whitespace
            select value;

        ////////// ARRAY //////////
        private static IParser<T> betweenBrackets<T>(IParser<T> betweenParser) =>
            Parser.Between(Parser.Symbol("["), betweenParser, Parser.Symbol("]"));

        private static IParser<IEnumerable<T>> ListOfParser<T>(IParser<T> valueParser) =>
            betweenBrackets(Parser.SepBy(valueParser, valueSeparator));

        public static IParser<ArrayValue> ArrayValue =
            from values in ListOfParser(JsonValue)
            select new ArrayValue(values);

        ////////// OBJECT //////////
        private static IParser<T> betweenBraces<T>(IParser<T> betweenParser) =>
            Parser.Between(Parser.Symbol("{"), betweenParser, Parser.Symbol("}"));

        private static IParser<IEnumerable<T>> ObjectOfParser<T>(IParser<T> valueParser) =>
            betweenBraces(Parser.SepBy(valueParser, valueSeparator));

        private static IParser<string> nameSeperator = Parser.Symbol(":");
        private static IParser<ObjectKeyValuePair> member =
            from _ in whitespace
            from key in StringValue
            from __ in whitespace
            from sep in nameSeperator
            from value in JsonValue
            select new ObjectKeyValuePair { Key = key, Value = value };

        public static IParser<ObjectValue> ObjectValue =
            from members in ObjectOfParser(member)
            select new ObjectValue(members);
    }
}
