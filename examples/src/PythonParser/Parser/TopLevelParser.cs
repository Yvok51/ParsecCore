using ParsecCore;
using ParsecCore.Indentation;
using PythonParser.Structures;

namespace PythonParser.Parser
{
    internal static class TopLevelParser
    {
        public static Parser<IReadOnlyList<Stmt>, char> PythonFile =
            Parsers.Between(
                Control.EOLWhitespace,
                Control.EOLLexeme.Create(Indentation.NonIndented(Control.EOLWhitespace, Statements.Statement)).Many(),
                Parsers.EOF<char>()
            );
    }
}
