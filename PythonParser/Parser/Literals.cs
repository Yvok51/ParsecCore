﻿using ParsecCore;
using ParsecCore.MaybeNS;
using PythonParser.Structures;
using System.Globalization;

namespace PythonParser.Parser
{
    internal static class Literals
    {
        #region STRINGS

        private static Parser<string, char> stringPrefix =
            Combinators.Choice(
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
            from __ in Parsers.Char('u')
            from first in hexadecimalDigit
            from second in hexadecimalDigit
            from third in hexadecimalDigit
            from fourth in hexadecimalDigit
            select (char)int.Parse(
                new string(new char[] { first, second, third, fourth }),
                NumberStyles.AllowHexSpecifier
            );
        private static readonly Parser<char, char> bit8HexEncoded =
            from __ in Parsers.Char('x')
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
            Combinators.Choice(toEscaped.Keys.Select(c => Parsers.Char(c)));
        private static readonly Parser<char, char> escape = Parsers.Char('\\');
        private static readonly Parser<char, char> escapedChar =
            from escapedChar in charToEscape
            select toEscaped[escapedChar];

        private static readonly Parser<char, char> escaped =
            from _ in escape
            from escaped in Combinators.Choice(escapedChar, bit8HexEncoded, bit16HexEncoded)
            select escaped;

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
        //private static readonly Parser<char, char> insideLongStringChar =
        //    Parsers.Satisfy(
        //        c => c != '\\',
        //        "non-escape character"
        //    );

        //private static readonly Parser<char, char> insideLongDoubleQuoteStringChar =
        //    from _ in Parsers.String("\"\"\"").Try()


        private static readonly Parser<char, char> doubleQuote = Parsers.Char('\"');
        private static readonly Parser<char, char> singleQuote = Parsers.Char('\'');

        private static readonly Parser<string, char> shortString =
            Combinators.Choice(
                Combinators.Between(singleQuote, insideSingleQuoteShortStringChar.Or(escaped).Many()),
                Combinators.Between(doubleQuote, insideDoubleQuoteShortStringChar.Or(escaped).Many())
            );

        //private static readonly Parser<string, char> longString =
        //    Combinators.Choice(
        //        (from tripleQuote in Parsers.String("\"\"\"")
        //         from 
        //        )
        //    );

        private static Parser<StringLiteral, char> String = 
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

        private static Parser<int, char> biInteger =
            from prefix in Parsers.Char('b').Or(Parsers.Char('B'))
            from integer in biDigit.Or(Parsers.Char('_')).Many1()
            select Convert.ToInt32(integer.Replace("_", ""), 2);
        private static Parser<int, char> octInteger =
            from prefix in Parsers.Char('o').Or(Parsers.Char('O'))
            from integer in octDigit.Or(Parsers.Char('_')).Many1()
            select Convert.ToInt32(integer.Replace("_", ""), 8);
        private static Parser<int, char> hexInteger =
            from prefix in Parsers.Char('x').Or(Parsers.Char('X'))
            from integer in biDigit.Or(Parsers.Char('_')).Many1()
            select Convert.ToInt32(integer.Replace("_", ""), 16);
        private static Parser<int, char> otherBaseIntegers =
            from integer in Combinators.Choice(biInteger, octInteger, hexInteger)
            select integer;

        private static Parser<int, char> nonZeroInteger =
            from first in nonZeroDigit
            from rest in digit.Or(Parsers.Char('_')).Many()
            select int.Parse((first + rest).Replace("_", ""));
        private static Parser<int, char> zero =
            from _ in Parsers.Char('0').Or(Parsers.Char('_')).Many()
            select 0;
        // Factor out common leading zero from parser for zero and the other bases => do not have to backtrack
        private static Parser<int, char> factoredZeroAndOtherBase =
            from leadingZero in Parsers.Char('0')
            from integer in zero.Or(otherBaseIntegers)
            select integer;

        private static Parser<IntegerLiteral, char> Integer = 
            from num in nonZeroInteger.Or(factoredZeroAndOtherBase)
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
            select e + sign + digits;
        private static Parser<string, char> isFractionPartOptional(IMaybe<string> digits)
            => digits.IsEmpty ? fraction : (from dot in Parsers.Char('.')
                                            from fractionDigits in digitPart.Option("0")
                                            select dot + fractionDigits);
        private static Parser<string, char> pointFloat =
            from digits in digitPart.Optional()
            from fractionPart in isFractionPartOptional(digits)
            select digits.Else("0") + fractionPart;
        private static Parser<string, char> exponentFloat =
            from basePart in digitPart.Try().Or(pointFloat)
            from exponentPart in exponent
            select basePart + exponentPart;

        private static Parser<FloatLiteral, char> Float =
            from floatNumber in pointFloat.Try().Or(exponentFloat)
            select new FloatLiteral(double.Parse(floatNumber, CultureInfo.InvariantCulture.NumberFormat));

        ////
        // Does not support imaginary and thus complex numbers
        ////

        #endregion

        #region IDENTIFIERS

        public static Parser<char, char> IdentifierStart =
            Parsers.Satisfy(c => char.IsLetter(c) || c == '_', "identifier character");
        public static Parser<char, char> IdentifierContinue =
            Parsers.Satisfy(c => char.IsLetterOrDigit(c) || c == '_', "identifier character");

        public static Parser<IdentifierLiteral, char> Identifier =
            Control.Lexeme.Create(
                from start in IdentifierStart
                from rest in IdentifierContinue.Many()
                select new IdentifierLiteral(start + rest)
            );

        #endregion

        public static Parser<Expr, char> Literal =
            Control.Lexeme.Create(
                Combinators.Choice<Expr, char>(
                    String,
                    Integer.Try(),
                    Float
                )
            );
    }
}
