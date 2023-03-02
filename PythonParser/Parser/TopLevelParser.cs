using ParsecCore;
using PythonParser.Parser;
using PythonParser.Structures;
using ParsecCore.Indentation;

namespace PythonParser.Parser
{
    internal static class TopLevelParser
    {
        public static Parser<IReadOnlyList<Stmt>, char> PythonFile =
            from leadingWhitespace in Control.EOLWhitespace
            from stmts in Control.EOLLexeme.Create(Indentation.NonIndented(Control.EOLWhitespace, Statements.Statement)).Many()
            from _ in Parsers.EOF
            select stmts;
    }
}
