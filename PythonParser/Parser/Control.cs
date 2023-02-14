using ParsecCore;
using ParsecCore.Help;
using PythonParser.Structures;

namespace PythonParser.Parser
{
    internal static class Control
    {
        private static readonly Parser<char, char> EOL = Parsers.EOL;

        public static readonly Parser<None, char> Comment =
            from start in Parsers.Char('#')
            from content in Parsers.NoneOf('\n', '\r').Many()
            select None.Instance;

        private static readonly Parser<None, char> Whitespace =
            Parsers.Satisfy(c => c == ' ' || c == '\t' || c == '\f', "whitespace").Void()
            .Or(from escape in Parsers.Char('\\') from eol in EOL select None.Instance);

        private static readonly Parser<None, char> SpaceConsumer =
            Parsers.Satisfy(c => c == ' ' || c == '\f', "space").Many().Void();

        private static readonly Parser<None, char> TabConsumer =
            Parsers.Satisfy(c => c == '\t' || c == '\f', "tab").Many().Void();

        //public static readonly Parser<None, char> WhitespaceConsumer =
        //    from feeds in Parsers.Char('\f').Many()
        //    from nonMixedRest in SpaceConsumer.Or(TabConsumer)
        //    select None.Instance;

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
                => from parsed in parser from _ in _spaceConsumer select parsed;

            private Parser<None, char> _spaceConsumer;
        }

        public static readonly LexemeFactory Lexeme = new LexemeImpl(Whitespace);

        public static readonly Parser<char, char> Plus = Lexeme.Create(Parsers.Char('+'));
        public static readonly Parser<char, char> Minus = Lexeme.Create(Parsers.Char('-'));
        public static readonly Parser<char, char> Asterisk = Lexeme.Create(Parsers.Char('*'));
        public static readonly Parser<char, char> Slash = Lexeme.Create(Parsers.Char('/'));
        public static readonly Parser<char, char> Modulo = Lexeme.Create(Parsers.Char('%'));
        public static readonly Parser<string, char> DoubleAsterisk = Lexeme.Create(Parsers.String("**")).Try();
        public static readonly Parser<string, char> DoubleSlash = Lexeme.Create(Parsers.String("//")).Try();

        public static readonly Parser<char, char> OpenParan = Lexeme.Create(Parsers.Char('('));
        public static readonly Parser<char, char> CloseParan = Lexeme.Create(Parsers.Char(')'));

        public static readonly Parser<char, char> OpenBracket = Lexeme.Create(Parsers.Char('['));
        public static readonly Parser<char, char> CloseBracket = Lexeme.Create(Parsers.Char(']'));

        public static readonly Parser<char, char> OpenBrace = Lexeme.Create(Parsers.Char('{'));
        public static readonly Parser<char, char> CloseBrace = Lexeme.Create(Parsers.Char('}'));

        public static readonly Parser<char, char> Comma = Lexeme.Create(Parsers.Char(','));
        public static readonly Parser<char, char> Colon = Lexeme.Create(Parsers.Char(':'));
        public static readonly Parser<char, char> Dot = Lexeme.Create(Parsers.Char('.'));
        public static readonly Parser<char, char> Semicolon = Lexeme.Create(Parsers.Char(';'));
        public static readonly Parser<char, char> At = Lexeme.Create(Parsers.Char('@'));
        public static readonly Parser<char, char> Assign = Lexeme.Create(Parsers.Char('='));
 
        public static readonly Parser<string, char> Equal = Lexeme.Create(Parsers.String("==")).Try();
        public static readonly Parser<string, char> NotEqual = Lexeme.Create(Parsers.String("!=")).Try();
        public static readonly Parser<string, char> LE = Lexeme.Create(Parsers.String("<=")).Try();
        public static readonly Parser<string, char> GE = Lexeme.Create(Parsers.String(">=")).Try();
        public static readonly Parser<char, char> LT = Lexeme.Create(Parsers.Char('<'));
        public static readonly Parser<char, char> GT = Lexeme.Create(Parsers.Char('>'));

        public static Parser<string, char> Keyword(string keyword) =>
            Lexeme.Create(from word in Parsers.String(keyword)
            from _ in Combinators.NotFollowedBy(Literals.IdentifierContinue, $"keyword {keyword} expected")
            select word).Try();

        public static readonly Parser<string, char> Not = Keyword("not").Try();

        public static readonly Parser<BinaryOperator, char> Is = Keyword("is").Try().Map(_ => BinaryOperator.Is);
        public static readonly Parser<BinaryOperator, char> IsNot =
            (from _ in Is
            from not in Not
            select BinaryOperator.IsNot).Try();

        public static readonly Parser<BinaryOperator, char> In = Keyword("in").Try().Map(_ => BinaryOperator.Is);
        public static readonly Parser<BinaryOperator, char> NotIn =
            (from not in Not
            from _ in In
            select BinaryOperator.IsNot).Try();

        public static readonly Parser<string, char> And = Keyword("and").Try();
        public static readonly Parser<string, char> Or = Keyword("or").Try();
    }
}
