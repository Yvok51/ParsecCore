using ParsecCore;
using ParsecCore.MaybeNS;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace JSONtoXML
{
    /// <summary>
    /// Parsers for JSON.
    /// Based on this RFC: https://datatracker.ietf.org/doc/html/rfc8259
    /// </summary>
    public static class JSONParsers
    {
        // We don't use Parser.Spaces since the its definition of whitespace is different
        // to the definition found in the JSON RFC.
        // Subsequently we also don't use Token and Symbol but instead Between and custom Symbol
        private static readonly Parser<string> whitespace =
            Parsers.Satisfy(c => c == ' ' || c == '\n' || c == '\t' || c == '\r', "whitespace").Many();

        private static Parser<T> Token<T>(Parser<T> parser) =>
            from result in parser
            from _ in whitespace
            select result;

        private static Parser<string> Symbol(string s) =>
            Token(Parsers.String(s));

        private static readonly Parser<string> valueSeparator = Symbol(",");

        ////////// NULL //////////
        public static readonly Parser<NullValue> NullValue =
            from _ in Symbol("null")
            select new NullValue();

        ////////// BOOLEAN //////////
        private static readonly Parser<bool> trueParser =
            from _ in Symbol("true")
            select true;
        private static readonly Parser<bool> falseParser =
            from _ in Symbol("false")
            select false;
        private static readonly Parser<bool> Boolean = Combinators.Choice(trueParser, falseParser);
        public static Parser<BoolValue> BoolValue =
            from b in Boolean
            select new BoolValue(b);

        ////////// NUMBER //////////
        private static readonly Parser<string> zero = Parsers.String("0");
        private static readonly Parser<string> nonZeroInteger =
            from firstDigit in Parsers.Satisfy(c => char.IsDigit(c) && c != '0', "non-zero digit")
            from nextDigits in Parsers.Digit.Many()
            select firstDigit.ToString() + nextDigits;
        private static readonly Parser<string> integer = Combinators.Choice(zero, nonZeroInteger);

        private static readonly Parser<string> minus = Parsers.String("-");
        private static readonly Parser<string> plus = Parsers.String("+");
        private static readonly Parser<string> plusOrMinus = Combinators.Choice(plus, minus);

        private static readonly Parser<string> decimalPoint = Parsers.String(".");
        private static readonly Parser<string> fractionalPart =
            from point in decimalPoint
            from digits in Parsers.Digits
            select point + digits;

        private static readonly Parser<string> exponentSymbol =
            Combinators.Choice(Parsers.String("e"), Parsers.String("E"));
        private static readonly Parser<string> exponent =
            from symbol in exponentSymbol
            from sign in plusOrMinus.Option(string.Empty)
            from digits in Parsers.Digits
            select symbol + sign + digits;

        private static readonly CultureInfo USCulture = new CultureInfo("en-US");
        private static readonly Parser<double> Number =
            from minus in minus.Option(string.Empty)
            from integer in integer
            from frac in fractionalPart.Option(string.Empty)
            from exp in exponent.Option(string.Empty)
            select Double.Parse(minus + integer + frac + exp, USCulture);

        public static readonly Parser<NumberValue> NumberValue =
            from n in Number
            select new NumberValue(n);

        ////////// STRING //////////
        private static readonly Parser<char> quote = Parsers.Char('\"');
        private static readonly Parser<char> escape = Parsers.Char('\\');

        private static readonly Parser<char> hexadecimalDigit = Parsers.Satisfy(
            c => char.IsDigit(c) || c == 'A' || c == 'B' || c == 'C' || c == 'D' || c == 'E' || c == 'F',
            "hexadecimal digit"
        );
        private static readonly Parser<char> hexEncoded =
            from _ in escape
            from __ in Parsers.Char('u')
            from first in hexadecimalDigit
            from second in hexadecimalDigit
            from third in hexadecimalDigit
            from fourth in hexadecimalDigit
            select (char)int.Parse(
                new string(new char[] { first, second, third, fourth }),
                NumberStyles.AllowHexSpecifier
            );

        private static readonly Parser<char> insideStringChar =
            Parsers.Satisfy(c => c != '"' && c != '\n', "non-quote/non-CRLF character");

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
        private static IReadOnlyList<Parser<char>> charsToParsers(IEnumerable<char> chars)
        {
            List<Parser<char>> parsers = new List<Parser<char>>();
            foreach (var c in chars)
            {
                parsers.Add(Parsers.Char(c));
            }
            return parsers;
        }

        private static readonly Parser<char> charToEscape = Combinators.Choice(charsToParsers(toEscaped.Keys));
        private static readonly Parser<char> escapedChar =
            from esc in escape
            from escapedChar in charToEscape
            select toEscaped[escapedChar];

        private static readonly Parser<char> escaped = Combinators.Choice(escapedChar.Try(), hexEncoded);

        private static readonly Parser<char> stringChar = Combinators.Choice(escaped, insideStringChar);
        private static readonly Parser<string> String = Combinators.Between(quote, stringChar.Many());

        public static readonly Parser<StringValue> StringValue =
            from str in String
            select new StringValue(str);

        ////////// VALUE //////////

        /** 
         * Create a wrapper parser as a hack to circumnavigate the issue
         * with JsonValue and ArrayValue/ObjectValue being dependent on each
         * other.
         * With the wrapper lambda the variables ArrayValue and ObjectValue 
         * are captured and their value is added later on -> when the parser
         * is invoked the values are correct and not null
         */
        public static readonly Parser<JsonValue> JsonValue =
            (input) =>
            {
                return Combinators.Between(
                    whitespace,
                    Combinators.Choice<JsonValue>(
                        NullValue,
                        BoolValue,
                        NumberValue,
                        StringValue,
                        ArrayValue,
                        ObjectValue
                    )
                )(input);
            };

        ////////// ARRAY //////////
        private static Parser<T> betweenBrackets<T>(Parser<T> betweenParser) =>
            Combinators.Between(Symbol("["), betweenParser, Symbol("]"));

        private static Parser<IReadOnlyList<T>> ListOfParser<T>(Parser<T> valueParser) =>
            betweenBrackets(Combinators.SepBy(valueParser, valueSeparator));

        public static readonly Parser<ArrayValue> ArrayValue =
            from values in ListOfParser(JsonValue)
            select new ArrayValue(values);

        ////////// OBJECT //////////
        private static Parser<T> betweenBraces<T>(Parser<T> betweenParser) =>
            Combinators.Between(Symbol("{"), betweenParser, Symbol("}"));

        private static Parser<IReadOnlyList<T>> ObjectOfParser<T>(Parser<T> valueParser) =>
            betweenBraces(Combinators.SepBy(valueParser, valueSeparator));

        private static readonly Parser<string> nameSeperator = Combinators.Between(whitespace, Parsers.String(":"));
        private static readonly Parser<ObjectKeyValuePair> member =
            from key in Combinators.Between(whitespace, StringValue)
            from sep in nameSeperator
            from value in JsonValue
            select new ObjectKeyValuePair { Key = key, Value = value };

        public static readonly Parser<ObjectValue> ObjectValue =
            from members in ObjectOfParser(member)
            select new ObjectValue(members);

        ////////// JSON DOCUMENT //////////

        public static readonly Parser<JsonValue> JsonDocument =
            from value in JsonValue
            from _ in Parsers.EOF
            select value;
    }
}
