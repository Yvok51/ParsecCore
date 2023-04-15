
using ParsecCore;
using ParsecCore.Help;
using ParsecCore.Indentation;
using PythonParser.Structures;

namespace PythonParser.Parser
{
    internal static class Statements
    {
        #region Simple Statement

        private static readonly Parser<ExpressionStmt, char> ExpressionStatement =
            Expressions.ExpressionList(Control.Lexeme).Map(exprs => new ExpressionStmt(exprs));

        private static readonly Parser<Expr, char> ComplexTarget =
            from primary in Expressions.Primary(Control.Lexeme)
            from rest in Parsers.Choice(
                from dot in Control.Dot(Control.Lexeme)
                from id in Literals.Identifier(Control.Lexeme)
                select new Func<Expr, Expr>((Expr expr) => new AttributeRef(expr, id)),
                (from subscript in Parsers.Between(
                    Control.OpenBracket(Control.EOLLexeme),
                    Expressions.ExpressionList(Control.EOLLexeme),
                    Control.CloseBracket(Control.Lexeme)
                 )
                 select new Func<Expr, Expr>((Expr expr) => new Subscription(expr, subscript))
                ).Try(),
                from slice in Parsers.Between(
                    Control.OpenBracket(Control.EOLLexeme),
                    Expressions.SliceList,
                    Control.CloseBracket(Control.Lexeme)
                )
                select new Func<Expr, Expr>((Expr expr) => new Slice(expr, slice))
            )
            select rest(primary);

        private static readonly Parser<Expr, char> Target =
            Parsers.Choice(
                    ComplexTarget.Try(),
                    Literals.Identifier(Control.Lexeme)
                );

        private static readonly Parser<IReadOnlyList<Expr>, char> TargetList =
            Parsers.SepEndBy1(Target, Control.Comma(Control.Lexeme));

        private static readonly Parser<Assignment, char> Assignment =
            from targets in TargetList
            from eq in Control.Assign(Control.Lexeme)
            from exprs in Expressions.ExpressionList(Control.Lexeme)
            select new Assignment(new List<IReadOnlyList<Expr>>() { targets }, exprs);

        private static readonly Parser<Pass, char> Pass =
            Control.Lexeme.Create(Control.Keyword("pass")).Map(_ => new Pass());

        private static readonly Parser<Return, char> Return =
            from ret in Control.Lexeme.Create(Control.Keyword("return"))
            from exprs in Expressions.ExpressionList(Control.Lexeme).Optional()
            select new Return(exprs);

        private static readonly Parser<Break, char> Break =
            Control.Keyword("break").Map(_ => new Break());

        private static readonly Parser<Continue, char> Continue =
            Control.Keyword("continue").Map(_ => new Continue());

        private static readonly Parser<IReadOnlyList<IdentifierLiteral>, char> ModulePath =
            Parsers.SepBy1(Literals.Identifier(Control.Lexeme), Control.Dot(Control.Lexeme));

        private static readonly Parser<ImportModule, char> ImportModule =
            from import in Control.Keyword("import")
            from modulePath in ModulePath
            from alias in (from _ in Control.Keyword("as")
                           from name in Literals.Identifier(Control.Lexeme)
                           select name).Optional()
            select new ImportModule(modulePath, alias);

        private static readonly Parser<ImportSpecific, char> ImportSpecific =
            from _ in Control.Keyword("from")
            from modulePath in ModulePath
            from __ in Control.Keyword("import")
            from name in Literals.Identifier(Control.Lexeme)
            from alias in (from _ in Control.Keyword("as")
                           from name in Literals.Identifier(Control.Lexeme)
                           select name).Optional()
            select new ImportSpecific(modulePath, name, alias);

        private static readonly Parser<ImportSpecificAll, char> ImportSpecificAll =
            from _ in Control.Keyword("from")
            from modulePath in ModulePath
            from __ in Control.Keyword("import")
            from star in Control.Asterisk(Control.Lexeme)
            select new ImportSpecificAll(modulePath);

        private static readonly Parser<Stmt, char> Import =
            Parsers.Choice<Stmt, char>(
                ImportModule,
                ImportSpecificAll.Try(),
                ImportSpecific
            );

        private static readonly Parser<Stmt, char> SimpleStatement =
            Parsers.Choice<Stmt, char>(
                Import.Try(),
                Continue.Try(),
                Break.Try(),
                Return.Try(),
                Pass.Try(),
                Assignment.Try(),
                ExpressionStatement
            );

        #endregion

        #region Compound Statement

        private static readonly Parser<IReadOnlyList<Stmt>, char> StatementList =
            Parsers.SepEndBy1(SimpleStatement, Control.Semicolon(Control.Lexeme));

        public static readonly Parser<Stmt, char> Statement =
            Parsers.Indirect(() => CompoundStatement!).Try().Or(
                from list in StatementList
                from eol in Control.EOL.Or(Parsers.EOF<char>().Map(_ => '\n'))
                select new Suite(list)
            );

        private static Parser<Expr, char> ConditionHead(string keyword) =>
            from _ in Control.Keyword(keyword)
            from test in Expressions.Expression(Control.Lexeme)
            from __ in Control.Colon(Control.Lexeme)
            select test;

        private static readonly Parser<(Expr, Suite), char> If =
            Indentation.IndentationBlockMany1(
                Control.EOLWhitespace,
                ConditionHead("if"),
                Maybe.Nothing<IndentLevel>(),
                (test, stmts) => (test, new Suite(new List<Stmt>(stmts))),
                Statement
            );

        private static readonly Parser<(Expr, Suite), char> Elif =
            Indentation.IndentationBlockMany1(
                Control.EOLWhitespace,
                ConditionHead("elif"),
                Maybe.Nothing<IndentLevel>(),
                (test, stmts) => (test, new Suite(new List<Stmt>(stmts))),
                Statement
            );

        private static readonly Parser<Suite, char> Else =
            Indentation.IndentationBlockMany1(
                Control.EOLWhitespace,
                from _ in Control.Keyword("else") from __ in Control.Colon(Control.Lexeme) select None.Instance,
                Maybe.Nothing<IndentLevel>(),
                (_, stmts) => new Suite(new List<Stmt>(stmts)),
                Statement
            );

        // TODO: add support for If on one line
        private static readonly Parser<If, char> IfStatement =
            from @if in If
            from elifs in Elif.Many()
            from @else in Else.Optional()
            select new If(@if.Item1, @if.Item2, elifs, @else);

        private static readonly Parser<(Expr, Suite), char> While =
            Indentation.IndentationBlockMany1(
                Control.EOLWhitespace,
                ConditionHead("while"),
                Maybe.Nothing<IndentLevel>(),
                (test, stmts) => (test, new Suite(new List<Stmt>(stmts))),
                Statement
            );

        // TODO: add support for While on one line
        private static readonly Parser<While, char> WhileStatement =
            from @while in While
            from @else in Else.Optional()
            select new While(@while.Item1, @while.Item2, @else);

        private static readonly Parser<(IReadOnlyList<Expr>, IReadOnlyList<Expr>), char> ForHead =
            from _ in Control.Keyword("for")
            from targets in TargetList
            from __ in Control.Keyword("in")
            from exprs in Expressions.ExpressionList(Control.Lexeme)
            from ___ in Control.Colon(Control.Lexeme)
            select (targets, exprs);

        private static readonly Parser<(IReadOnlyList<Expr>, IReadOnlyList<Expr>, Suite), char> For =
            Indentation.IndentationBlockMany1(
                Control.EOLWhitespace,
                ForHead,
                Maybe.Nothing<IndentLevel>(),
                (head, stmts) => (head.Item1, head.Item2, new Suite(new List<Stmt>(stmts))),
                Statement
            );

        private static readonly Parser<For, char> ForStatement =
            from @for in For
            from @else in Else.Optional()
            select new For(@for.Item1, @for.Item2, @for.Item3, @else);

        private static readonly Parser<IReadOnlyList<IdentifierLiteral>, char> ParameterList =
            Parsers.SepBy(Literals.Identifier(Control.EOLLexeme), Control.Comma(Control.EOLLexeme));

        private static readonly Parser<(IdentifierLiteral, IReadOnlyList<IdentifierLiteral>), char> FuncHead =
            from _ in Control.Keyword("def")
            from funcname in Literals.Identifier(Control.Lexeme)
            from parameterList in Parsers.Between(
                Control.OpenParan(Control.EOLLexeme),
                ParameterList,
                Control.CloseParan(Control.Lexeme)
            )
            from __ in Control.Colon(Control.Lexeme)
            select (funcname, parameterList);

        private static readonly Parser<Function, char> FuncDefStatement =
            Indentation.IndentationBlockMany1(
                Control.EOLWhitespace,
                FuncHead,
                Maybe.Nothing<IndentLevel>(),
                (head, body) => new Function(head.Item1, head.Item2, Array.Empty<(IdentifierLiteral, Expr)>(), new Suite(new List<Stmt>(body))),
                Statement
            );

        private static readonly Parser<Stmt, char> CompoundStatement =
            Parsers.Choice<Stmt, char>(
                IfStatement,
                WhileStatement,
                ForStatement,
                FuncDefStatement
            );

        #endregion
    }
}
