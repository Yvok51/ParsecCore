using ParsecCore;
using ParsecCore.Indentation;
using PythonParser.Structures;

namespace PythonParser.Parser
{
    internal static class TopLevelParser
    {
        public static Parser<IReadOnlyList<Stmt>, char> PythonFile =
            from leadingWhitespace in Control.EOLWhitespace
            from stmts in Control.EOLLexeme.Create(Indentation.NonIndented(Control.EOLWhitespace, Statements.Statement)).Many()
            from _ in Parsers.EOF<char>()
            select stmts;
    }
}
