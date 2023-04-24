using ParsecCore;
using ParsecCore.Help;
using PythonParser.Structures;
using System.Globalization;

namespace PythonParser.Parser
{
    internal static class Literals
    {
        #region STRINGS

        private static Parser<string, char> stringPrefix =
            Parsers.Choice(
                from r in Parsers.Char('r')
                from second in Parsers.OneOf('f', 'F').Map(c => c.ToString()).Or(Parsers.Return<string, char>(""))
                select r + second,
                from R in Parsers.Char('R')
                from second in Parsers.OneOf('f', 'F').Map(c => c.ToString()).Or(Parsers.Return<string, char>(""))
                select R + second,
                from f in Parsers.Char('f')
                from second in Parsers.OneOf('r', 'R').Map(c => c.ToString()).Or(Parsers.Return<string, char>(""))
                select f + second,
                from F in Parsers.Char('F')
                from second in Parsers.OneOf('r', 'R').Map(c => c.ToString()).Or(Parsers.Return<string, char>(""))
                select F + second,
                Parsers.Char('u').Map(c => c.ToString()),
                Parsers.Char('U').Map(c => c.ToString())
            );

        private static readonly Parser<char, char> hexadecimalDigit = Parsers.Satisfy(
            c => char.IsDigit(c) || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f'),
            "hexadecimal digit"
        );
        private static readonly Parser<char, char> bit16HexEncoded =
            from _ in Parsers.Char('u')
            from first in hexadecimalDigit
            from second in hexadecimalDigit
            from third in hexadecimalDigit
            from fourth in hexadecimalDigit
            select (char)int.Parse(
                new string(new char[] { first, second, third, fourth }),
                NumberStyles.AllowHexSpecifier
            );
        private static readonly Parser<char, char> bit8HexEncoded =
            from _ in Parsers.Char('x')
            from first in hexadecimalDigit
            from second in hexadecimalDigit
            select (char)int.Parse(
                new string(new char[] { first, second }),
                NumberStyles.AllowHexSpecifier
            );
        private static readonly Dictionary<char, char> toEscaped = new Dictionary<char, char>
        {
            { '"', '"' },
            { '\'', '\'' },
            { '\\', '\\' },
            { 'a', '\a' },
            { 'b', '\b' },
            { 'f', '\f' },
            { 'n', '\n' },
            { 'r', '\r' },
            { 't', '\t' },
            { 'v', '\v' },
        };
        private static readonly Parser<char, char> charToEscape =
            Parsers.Choice(toEscaped.Keys.Select(c => Parsers.Char(c)));
        private static readonly Parser<char, char> escape = Parsers.Char('\\');
        private static readonly Parser<char, char> escapedChar =
            from escapedChar in charToEscape
            select toEscaped[escapedChar];

        private static readonly Parser<char, char> escaped =
            escape.Then(Parsers.Choice(escapedChar, bit8HexEncoded, bit16HexEncoded));

        private static readonly Parser<char, char> insideDoubleQuoteShortStringChar =
            Parsers.Satisfy(
                c => c != '"' && c != '\n' && c != '\r' && c != '\\',
                "non-quote/non-CRLF/non-escape character"
            );
        private static readonly Parser<char, char> insideSingleQuoteShortStringChar =
            Parsers.Satisfy(
                c => c != '\'' && c != '\n' && c != '\r' && c != '\\',
                "non-quote/non-CRLF/non-escape character"
            );

        private static readonly Parser<char, char> doubleQuote = Parsers.Char('\"');
        private static readonly Parser<char, char> singleQuote = Parsers.Char('\'');

        private static readonly Parser<string, char> shortString =
            Parsers.Choice(
                Parsers.Between(singleQuote, insideSingleQuoteShortStringChar.Or(escaped).Many()),
                Parsers.Between(doubleQuote, insideDoubleQuoteShortStringChar.Or(escaped).Many())
            );

        internal static Parser<StringLiteral, char> String =
            from prefix in stringPrefix.Option("")
            from value in shortString
            select new StringLiteral(prefix, value);

        ////
        // Left out long strings, formated strings, byte literals, some escape sequences and string literal concatenation
        ////

        #endregion

        #region NUMBERS

        private static Parser<char, char> biDigit = Parsers.Satisfy(c => c == '0' || c == '1', "binary digit");
        private static Parser<char, char> octDigit = Parsers.Satisfy(c => c >= '0' && c <= '7', "octal digit");
        private static Parser<char, char> digit = Parsers.Satisfy(c => c >= '0' && c <= '9', "digit");
        private static Parser<char, char> nonZeroDigit = Parsers.Satisfy(c => c >= '1' && c <= '9', "non-zero digit");
        private static Parser<char, char> hexDigit = Parsers.Satisfy(
            c => (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'),
            "hexadecimal digit"
        );

        private static string StripUnderscores(string str) => str.Replace("_", "");

        private static Parser<int, char> biInteger =
            from prefix in Parsers.Char('b').Or(Parsers.Char('B'))
            from integer in biDigit.Or(Parsers.Char('_')).Many1()
            select Convert.ToInt32(StripUnderscores(integer), 2);
        private static Parser<int, char> octInteger =
            from prefix in Parsers.Char('o').Or(Parsers.Char('O'))
            from integer in octDigit.Or(Parsers.Char('_')).Many1()
            select Convert.ToInt32(StripUnderscores(integer), 8);
        private static Parser<int, char> hexInteger =
            from prefix in Parsers.Char('x').Or(Parsers.Char('X'))
            from integer in hexDigit.Or(Parsers.Char('_')).Many1()
            select Convert.ToInt32(StripUnderscores(integer), 16);
        private static Parser<int, char> otherBaseIntegers =
            from integer in Parsers.Choice(biInteger, octInteger, hexInteger)
            select integer;

        private static Parser<int, char> nonZeroInteger =
            from first in nonZeroDigit
            from rest in digit.Or(Parsers.Char('_')).Many()
            select int.Parse(StripUnderscores(first + rest));
        private static Parser<int, char> zero =
            from _ in Parsers.Char('0').Or(Parsers.Char('_')).Many()
            select 0;
        // Factor out common leading zero from parser for zero and the other bases => do not have to backtrack
        private static Parser<int, char> factoredZeroAndOtherBase =
            from leadingZero in Parsers.Char('0')
            from integer in otherBaseIntegers.Or(zero)
            select integer;

        internal static Parser<IntegerLiteral, char> Integer =
            from num in nonZeroInteger.Or(factoredZeroAndOtherBase)
            from alphaNumGuard in Parsers.NotFollowedBy(Parsers.AlphaNum, "Invalid character in number")
            select new IntegerLiteral(num);

        private static Parser<string, char> digitPart =
            from leadingDigit in digit
            from rest in digit.Or(Parsers.Char('_')).Many()
            select leadingDigit + rest;
        private static Parser<string, char> fraction =
            from dot in Parsers.Char('.')
            from digits in digitPart
            select dot + digits;
        private static Parser<string, char> exponent =
            from e in Parsers.Char('e').Or(Parsers.Char('E'))
            from sign in Parsers.Char('+').Or(Parsers.Char('-')).Option('+')
            from digits in digitPart
            select "E" + sign + digits;
        private static Parser<string, char> isFractionPartOptional(Maybe<string> digits)
            => digits.IsEmpty ? fraction : (from dot in Parsers.Char('.')
                                            from fractionDigits in digitPart.Option("0")
                                            select dot + fractionDigits);
        private static Parser<string, char> pointFloat =
            from digits in digitPart.Optional()
            from fractionPart in isFractionPartOptional(digits)
            select digits.Else("0") + fractionPart;
        private static Parser<string, char> exponentFloat =
            from basePart in pointFloat.Try().Or(digitPart)
            from exponentPart in exponent
            select basePart + exponentPart;

        internal static Parser<FloatLiteral, char> Float =
            from floatNumber in exponentFloat.Try().Or(pointFloat)
            from alphaNumGuard in Parsers.NotFollowedBy(Parsers.AlphaNum, "Invalid character in number")
            select new FloatLiteral(
                double.Parse(StripUnderscores(floatNumber),
                CultureInfo.InvariantCulture.NumberFormat)
            );

        ////
        // Does not support imaginary and thus complex numbers
        ////

        #endregion

        #region IDENTIFIERS

        public static Parser<char, char> IdentifierStart =
            Parsers.Satisfy(c => char.IsLetter(c) || c == '_', "identifier character");
        public static Parser<char, char> IdentifierContinue =
            Parsers.Satisfy(c => char.IsLetterOrDigit(c) || c == '_', "identifier character");

        public static Parser<IdentifierLiteral, char> Identifier(Control.LexemeFactory lexeme)
        {
            var isKeyword = (string id) => Control.Keywords.Contains(id);
            return lexeme.Create(
                from start in IdentifierStart
                from rest in IdentifierContinue.Many()
                from id in Parsers.Return<string, char>(start.ToString() + rest)
                    .Assert(id => !isKeyword(id), "Identifier expected, got keyword")
                select new IdentifierLiteral(id)
            );
        }

        #endregion

        public static Parser<Expr, char> Literal(Control.LexemeFactory lexeme)
            => lexeme.Create(
                Parsers.Choice<Expr, char>(
                    String,
                    Integer.Try(),
                    Float
                )
            );
    }
}
