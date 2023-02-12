using ParsecCore;
using ParsecCore.Help;
using PythonParser.Structures;

namespace PythonParser.Parser
{
    internal static class Control
    {
        public static Parser<char, char> EOL = Parsers.EOL;

        public static Parser<None, char> Comment =
            from start in Parsers.Char('#')
            from content in Parsers.NoneOf('\n', '\r')
            from eol in EOL
            select None.Instance;

        public static Parser<None, char> Whitespace =
            Parsers.Satisfy(c => c == ' ' || c == '\t' || c == '\f', "whitespace").Void()
            .Or(from escape in Parsers.Char('\\') from eol in EOL select None.Instance);

        private static Parser<None, char> spaceConsumer =
            Parsers.Satisfy(c => c == ' ' || c == '\f', "space").Many().Void();

        private static Parser<None, char> tabConsumer =
            Parsers.Satisfy(c => c == '\t' || c == '\f', "tab").Many().Void();

        public static Parser<None, char> WhitespaceConsumer =
            from feeds in Parsers.Char('\f').Many()
            from nonMixedRest in spaceConsumer.Or(tabConsumer)
            select None.Instance;

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

        public static LexemeFactory Lexeme = new LexemeImpl(WhitespaceConsumer);

        public static Parser<char, char> Plus = Lexeme.Create(Parsers.Char('+'));
        public static Parser<char, char> Minus = Lexeme.Create(Parsers.Char('-'));
        public static Parser<char, char> Asterisk = Lexeme.Create(Parsers.Char('*'));
        public static Parser<char, char> Slash = Lexeme.Create(Parsers.Char('/'));
        public static Parser<char, char> Modulo = Lexeme.Create(Parsers.Char('%'));
        public static Parser<string, char> DoubleAsterisk = Lexeme.Create(Parsers.String("**")).Try();
        public static Parser<string, char> DoubleSlash = Lexeme.Create(Parsers.String("//")).Try();

        public static Parser<char, char> OpenParan = Lexeme.Create(Parsers.Char('('));
        public static Parser<char, char> CloseParan = Lexeme.Create(Parsers.Char(')'));

        public static Parser<char, char> OpenBracket = Lexeme.Create(Parsers.Char('['));
        public static Parser<char, char> CloseBracket = Lexeme.Create(Parsers.Char(']'));

        public static Parser<char, char> OpenBrace = Lexeme.Create(Parsers.Char('{'));
        public static Parser<char, char> CloseBrace = Lexeme.Create(Parsers.Char('}'));

        public static Parser<char, char> Comma = Lexeme.Create(Parsers.Char(','));
        public static Parser<char, char> Colon = Lexeme.Create(Parsers.Char(':'));
        public static Parser<char, char> Dot = Lexeme.Create(Parsers.Char('.'));
        public static Parser<char, char> Semicolon = Lexeme.Create(Parsers.Char(';'));
        public static Parser<char, char> At = Lexeme.Create(Parsers.Char('@'));
        public static Parser<char, char> Assign = Lexeme.Create(Parsers.Char('='));

        public static Parser<string, char> Equal = Lexeme.Create(Parsers.String("==")).Try();
        public static Parser<string, char> NotEqual = Lexeme.Create(Parsers.String("!=")).Try();
        public static Parser<string, char> LE = Lexeme.Create(Parsers.String("<=")).Try();
        public static Parser<string, char> GE = Lexeme.Create(Parsers.String(">=")).Try();
        public static Parser<char, char> LT = Lexeme.Create(Parsers.Char('<'));
        public static Parser<char, char> GT = Lexeme.Create(Parsers.Char('>'));

        public static Parser<string, char> Keyword(string keyword) =>
            from word in Parsers.String(keyword)
            from _ in Combinators.NotFollowedBy(Literals.IdentifierContinue, $"keyword {keyword} expected")
            select word;

        public static Parser<string, char> Not = Lexeme.Create(Keyword("not")).Try();

        public static Parser<BinaryOperator, char> Is = Lexeme.Create(Keyword("is")).Try().Map(_ => BinaryOperator.Is);
        public static Parser<BinaryOperator, char> IsNot =
            (from _ in Is
            from not in Not
            select BinaryOperator.IsNot).Try();

        public static Parser<BinaryOperator, char> In = Lexeme.Create(Keyword("in")).Try().Map(_ => BinaryOperator.Is);
        public static Parser<BinaryOperator, char> NotIn =
            (from not in Not
            from _ in In
            select BinaryOperator.IsNot).Try();

        public static Parser<string, char> And = Lexeme.Create(Keyword("and")).Try();
        public static Parser<string, char> Or = Lexeme.Create(Keyword("or")).Try();
    }
}
