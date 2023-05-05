using ParsecCore;
using ParsecCore.Help;
using PythonParser.Structures;

namespace PythonParser.Parser
{
    internal static class Control
    {
        public static readonly Parser<char, char> EOL = Parsers.EOL;

        public static readonly Parser<char, char> Comment =
            Parsers.Char('#').Then(Parsers.NoneOf('\n', '\r').Many()).MapConstant(' ');

        private static readonly Parser<char, char> WhitespaceChar =
            Parsers.Satisfy(c => c == ' ' || c == '\t' || c == '\f', "whitespace")
            .Or(from escape in Parsers.Char('\\') from eol in EOL select ' ');

        private static readonly Parser<string, char> Whitespace =
            WhitespaceChar.Or(Comment).Many();

        public static readonly Parser<string, char> EOLWhitespace =
            WhitespaceChar.Or(EOL).Or(Comment).Many();


        public static IReadOnlySet<string> Keywords = new HashSet<string>()
        {
            "False", "class", "finally", "is", "return",
            "None", "continue", "for", "lambda", "try",
            "True", "def", "from", "nonlocal", "while",
            "and", "del", "global", "not", "with",
            "as", "elif", "if", "or", "yield",
            "assert", "else", "import", "pass",
            "break", "except", "in", "raise"
        };

        /// <summary>
        /// Creates a lexeme - consumes a string and any whitespace after it and returns the parsed string
        /// Interface for multiple different implementations of Tokens without needing to pass a space consumer.
        /// </summary>
        public interface LexemeFactory
        {
            public Parser<T, char> Create<T>(Parser<T, char> parser);
        }
        private class LexemeImpl : LexemeFactory
        {
            public LexemeImpl(Parser<None, char> spaceConsumer)
            {
                _spaceConsumer = spaceConsumer;
            }

            public Parser<T, char> Create<T>(Parser<T, char> parser)
                => Parsers.Token(parser, _spaceConsumer);

            private Parser<None, char> _spaceConsumer;
        }

        public static readonly LexemeFactory Lexeme = new LexemeImpl(Whitespace.Void());
        public static readonly LexemeFactory EOLLexeme = new LexemeImpl(EOLWhitespace.Void());

        public static Parser<char, char> Plus(LexemeFactory lexeme) => lexeme.Create(Parsers.Char('+'));
        public static Parser<char, char> Minus(LexemeFactory lexeme) => lexeme.Create(Parsers.Char('-'));
        public static Parser<char, char> Asterisk(LexemeFactory lexeme) => lexeme.Create(Parsers.Char('*'));
        public static Parser<char, char> Slash(LexemeFactory lexeme) => lexeme.Create(Parsers.Char('/'));
        public static Parser<char, char> Modulo(LexemeFactory lexeme) => lexeme.Create(Parsers.Char('%'));
        public static Parser<string, char> DoubleAsterisk(LexemeFactory lexeme)
            => lexeme.Create(Parsers.String("**")).Try();
        public static Parser<string, char> DoubleSlash(LexemeFactory lexeme)
            => lexeme.Create(Parsers.String("//")).Try();

        public static Parser<char, char> OpenParan(LexemeFactory lexeme) => lexeme.Create(Parsers.Char('('));
        public static Parser<char, char> CloseParan(LexemeFactory lexeme) => lexeme.Create(Parsers.Char(')'));

        public static Parser<char, char> OpenBracket(LexemeFactory lexeme) => lexeme.Create(Parsers.Char('['));
        public static Parser<char, char> CloseBracket(LexemeFactory lexeme) => lexeme.Create(Parsers.Char(']'));

        public static Parser<char, char> OpenBrace(LexemeFactory lexeme) => lexeme.Create(Parsers.Char('{'));
        public static Parser<char, char> CloseBrace(LexemeFactory lexeme) => lexeme.Create(Parsers.Char('}'));

        public static Parser<char, char> Comma(LexemeFactory lexeme) => lexeme.Create(Parsers.Char(','));
        public static Parser<char, char> Colon(LexemeFactory lexeme) => lexeme.Create(Parsers.Char(':'));
        public static Parser<char, char> Dot(LexemeFactory lexeme) => lexeme.Create(Parsers.Char('.'));
        public static Parser<char, char> Semicolon(LexemeFactory lexeme) => lexeme.Create(Parsers.Char(';'));
        public static Parser<char, char> At(LexemeFactory lexeme) => lexeme.Create(Parsers.Char('@'));
        public static Parser<char, char> Assign(LexemeFactory lexeme) => lexeme.Create(Parsers.Char('='));

        public static Parser<string, char> Equal(LexemeFactory lexeme) => lexeme.Create(Parsers.String("==")).Try();
        public static Parser<string, char> NotEqual(LexemeFactory lexeme)
            => lexeme.Create(Parsers.String("!=")).Try();
        public static Parser<string, char> LE(LexemeFactory lexeme) => lexeme.Create(Parsers.String("<=")).Try();
        public static Parser<string, char> GE(LexemeFactory lexeme) => lexeme.Create(Parsers.String(">=")).Try();
        public static Parser<char, char> LT(LexemeFactory lexeme) => lexeme.Create(Parsers.Char('<'));
        public static Parser<char, char> GT(LexemeFactory lexeme) => lexeme.Create(Parsers.Char('>'));

        public static Parser<string, char> Keyword(string keyword) => Keyword(keyword, Lexeme);

        public static Parser<string, char> Keyword(string keyword, LexemeFactory lexeme)
            => lexeme.Create(
                Parsers.String(keyword).FollowedBy(
                    Parsers.NotFollowedBy(Literals.IdentifierContinue, $"keyword {keyword} expected")
                )
               ).Try().FailWith(keyword);

        public static Parser<string, char> Not(LexemeFactory lexeme) => Keyword("not", lexeme).Try();

        public static Parser<BinaryOperator, char> Is(LexemeFactory lexeme)
            => Keyword("is", lexeme).Try().Map(_ => BinaryOperator.Is);
        public static Parser<BinaryOperator, char> IsNot(LexemeFactory lexeme)
            => Is(lexeme).Then(Not(lexeme)).MapConstant(BinaryOperator.IsNot).Try();

        public static Parser<BinaryOperator, char> In(LexemeFactory lexeme)
            => Keyword("in", lexeme).Try().Map(_ => BinaryOperator.Is);
        public static Parser<BinaryOperator, char> NotIn(LexemeFactory lexeme)
            => Not(lexeme).Then(In(lexeme)).MapConstant(BinaryOperator.NotIn).Try();

        public static Parser<string, char> And(LexemeFactory lexeme) => Keyword("and", lexeme).Try();
        public static Parser<string, char> Or(LexemeFactory lexeme) => Keyword("or", lexeme).Try();
    }
}
