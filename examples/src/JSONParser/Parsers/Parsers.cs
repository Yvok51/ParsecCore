using ParsecCore;
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
    internal static class JSONParsers
    {
        // We don't use Parser.Spaces since the its definition of whitespace is different
        // to the definition found in the JSON RFC.
        // Subsequently we also don't use Token and Symbol but instead Between and custom Symbol
        private static readonly Parser<IReadOnlyList<char>, char> whitespace =
            Parsers.Satisfy(c => c == ' ' || c == '\n' || c == '\t' || c == '\r', "whitespace").Many<char, char>();

        private static Parser<T, char> Token<T>(Parser<T, char> parser) =>
            parser.FollowedBy(whitespace);

        private static Parser<string, char> Symbol(string s) =>
            Token(Parsers.String(s));

        private static Parser<string, char> CharString(char c) =>
            Parsers.Char(c).MapConstant(c.ToString());

        private static readonly Parser<string, char> valueSeparator = Token(CharString(','));

        ////////// NULL //////////
        public static readonly Parser<NullValue, char> NullValue = Symbol("null").MapConstant(new NullValue());

        ////////// BOOLEAN //////////
        private static readonly Parser<bool, char> trueParser = Symbol("true").MapConstant(true);
        private static readonly Parser<bool, char> falseParser = Symbol("false").MapConstant(false);

        private static readonly Parser<bool, char> Boolean = trueParser.Or(falseParser);
        public static Parser<BoolValue, char> BoolValue =
            from b in Boolean
            select new BoolValue(b);

        ////////// NUMBER //////////
        private static readonly Parser<string, char> zero = CharString('0');
        private static readonly Parser<string, char> digits = Parsers.Digit.Many();
        private static readonly Parser<string, char> nonZeroInteger =
            from firstDigit in Parsers.NonZeroDigit
            from nextDigits in digits
            select firstDigit + nextDigits;
        private static readonly Parser<string, char> integer = nonZeroInteger.Or(zero);

        private static readonly Parser<string, char> minus = CharString('-');
        private static readonly Parser<string, char> plus = CharString('+');
        private static readonly Parser<string, char> plusOrMinus = plus.Or(minus);

        private static readonly Parser<string, char> decimalPoint = CharString('.');
        private static readonly Parser<string, char> fractionalPart =
            from point in decimalPoint
            from digits in Parsers.Digits
            select point + digits;

        private static readonly Parser<string, char> exponentSymbol =
            CharString('e').Or(CharString('E'));
        private static readonly Parser<string, char> exponent =
            from symbol in exponentSymbol
            from sign in plusOrMinus.Option(string.Empty)
            from digits in Parsers.Digits
            select string.Concat(symbol, sign, digits);

        private static readonly CultureInfo USCulture = new CultureInfo("en-US");
        private static readonly Parser<double, char> Number =
            from minus in minus.Option(string.Empty)
            from integer in integer
            from frac in fractionalPart.Option(string.Empty)
            from exp in exponent.Option(string.Empty)
            select Double.Parse(string.Concat(minus, integer, frac, exp), USCulture);

        public static readonly Parser<NumberValue, char> NumberValue =
            from n in Number
            select new NumberValue(n);

        ////////// STRING //////////
        private static readonly Parser<char, char> quote = Parsers.Char('\"');
        private static readonly Parser<char, char> escape = Parsers.Char('\\');

        private static readonly Parser<char, char> hexadecimalDigit = Parsers.Satisfy(
            c => (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f'),
            "hexadecimal digit"
        );

        private static readonly Parser<char, char> hexEncoded =
            from _ in Parsers.Char('u')
            from first in hexadecimalDigit
            from second in hexadecimalDigit
            from third in hexadecimalDigit
            from fourth in hexadecimalDigit
            select (char)int.Parse(
                new string(new char[] { first, second, third, fourth }),
                NumberStyles.AllowHexSpecifier
            );

        private static readonly Parser<char, char> insideStringChar =
            Parsers.Satisfy(c => c != '"' && c != '\\' && c != '\n' && c != '\r', "non-quote/non-CRLF character");

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
        private static IReadOnlyList<Parser<char, char>> charsToParsers(IEnumerable<char> chars)
        {
            List<Parser<char, char>> parsers = new List<Parser<char, char>>();
            foreach (var c in chars)
            {
                parsers.Add(Parsers.Char(c));
            }
            return parsers;
        }

        private static readonly Parser<char, char> charToEscape = Parsers.Choice(charsToParsers(toEscaped.Keys));
        private static readonly Parser<char, char> escapedChar =
            from escapedChar in charToEscape
            select toEscaped[escapedChar];

        private static readonly Parser<char, char> escaped = escape.Then(hexEncoded.Or(escapedChar));

        private static readonly Parser<char, char> stringChar = insideStringChar.Or(escaped);
        private static readonly Parser<string, char> String = Parsers.Between(quote, stringChar.Many());

        public static readonly Parser<StringValue, char> StringValue =
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
        public static readonly Parser<JsonValue, char> JsonValue =
            Parsers.Indirect(() => Token(
                    Parsers.Choice<JsonValue, char>(
                        NullValue,
                        BoolValue,
                        NumberValue,
                        StringValue,
                        ArrayValue,
                        ObjectValue
                    )
                ));

        ////////// ARRAY //////////
        private static Parser<T, char> betweenBrackets<T>(Parser<T, char> betweenParser) =>
            Parsers.Between(Symbol("["), betweenParser, Symbol("]"));

        private static Parser<IReadOnlyList<T>, char> ListOfParser<T>(Parser<T, char> valueParser) =>
            betweenBrackets(Parsers.SepBy(valueParser, valueSeparator));

        public static readonly Parser<ArrayValue, char> ArrayValue =
            ListOfParser(JsonValue).Map(values => new ArrayValue(values));

        ////////// OBJECT //////////
        private static Parser<T, char> betweenBraces<T>(Parser<T, char> betweenParser) =>
            Parsers.Between(Symbol("{"), betweenParser, Symbol("}"));

        private static Parser<IReadOnlyList<T>, char> ObjectOfParser<T>(Parser<T, char> valueParser) =>
            betweenBraces(Parsers.SepBy(valueParser, valueSeparator));

        private static readonly Parser<string, char> nameSeperator = Token(Parsers.String(":"));
        private static readonly Parser<ObjectKeyValuePair, char> member =
            from key in Token(StringValue)
            from sep in nameSeperator
            from value in JsonValue
            select new ObjectKeyValuePair { Key = key, Value = value };

        public static readonly Parser<ObjectValue, char> ObjectValue =
            ObjectOfParser(member).Map(members => new ObjectValue(members));

        ////////// JSON DOCUMENT //////////

        public static readonly Parser<JsonValue, char> JsonDocument =
            whitespace.Then(JsonValue).FollowedBy(Parsers.EOF<char>());
    }
}
