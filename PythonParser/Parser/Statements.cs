
using ParsecCore;
using ParsecCore.Help;
using ParsecCore.Indentation;
using ParsecCore.MaybeNS;
using PythonParser.Structures;

namespace PythonParser.Parser
{
    internal static class Statements
    {
        #region Simple Statement

        private static readonly Parser<ExpressionStmt, char> ExpressionStatement =
            Expressions.ExpressionList.Map(exprs => new ExpressionStmt(exprs));

        private static readonly Parser<Expr, char> ComplexTarget =
            from primary in Expressions.Primary
            from rest in Combinators.Choice(
                from dot in Control.Dot
                from id in Literals.Identifier
                select new Func<Expr, Expr>((Expr expr) => new AttributeRef(expr, id)),
                (from subscript in Combinators.Between(Control.OpenBracket, Expressions.ExpressionList, Control.CloseBracket)
                 select new Func<Expr, Expr>((Expr expr) => new Subscription(expr, subscript))
                ).Try(),
                from slice in Combinators.Between(Control.OpenBracket, Expressions.SliceList, Control.CloseBracket)
                select new Func<Expr, Expr>((Expr expr) => new Slice(expr, slice))
            )
            select rest(primary);

        private static readonly Parser<Expr, char> Target =
            Combinators.Choice(
                    ComplexTarget.Try(),
                    Literals.Identifier
                );

        private static readonly Parser<IReadOnlyList<Expr>, char> TargetList =
            Combinators.SepEndBy1(Target, Control.Comma);

        private static readonly Parser<Assignment, char> Assignment =
            from targetLists in (from targets in TargetList
                                 from eq in Control.Equal
                                 select targets).Many1()
            from exprs in Expressions.ExpressionList
            select new Assignment(targetLists, exprs);

        private static readonly Parser<Pass, char> Pass =
            Control.Lexeme.Create(Control.Keyword("pass")).Map(_ => new Pass());

        private static readonly Parser<Return, char> Return =
            from ret in Control.Lexeme.Create(Control.Keyword("return"))
            from exprs in Expressions.ExpressionList.Optional()
            select new Return(exprs);

        private static readonly Parser<Pass, char> Break =
            Control.Lexeme.Create(Control.Keyword("break")).Map(_ => new Pass());

        private static readonly Parser<Pass, char> Continue =
            Control.Lexeme.Create(Control.Keyword("continue")).Map(_ => new Pass());

        private static readonly Parser<IReadOnlyList<IdentifierLiteral>, char> ModulePath =
            Combinators.SepBy1(Literals.Identifier, Control.Dot);

        private static readonly Parser<ImportModule, char> ImportModule =
            from import in Control.Keyword("import")
            from modulePath in ModulePath
            from alias in (from _ in Control.Keyword("as")
                           from name in Literals.Identifier
                           select name).Optional()
            select new ImportModule(modulePath, alias);

        private static readonly Parser<ImportSpecific, char> ImportSpecific =
            from _ in Control.Keyword("from")
            from modulePath in ModulePath
            from __ in Control.Keyword("import")
            from name in Literals.Identifier
            from alias in (from _ in Control.Keyword("as")
                           from name in Literals.Identifier
                           select name).Optional()
            select new ImportSpecific(modulePath, name, alias);

        private static readonly Parser<ImportSpecificAll, char> ImportSpecificAll =
            from _ in Control.Keyword("from")
            from modulePath in ModulePath
            from __ in Control.Keyword("import")
            from star in Control.Asterisk
            select new ImportSpecificAll(modulePath);

        private static readonly Parser<Stmt, char> Import =
            Combinators.Choice<Stmt, char>(
                ImportModule,
                ImportSpecificAll.Try(),
                ImportSpecific
            );

        private static readonly Parser<Stmt, char> SimpleStatement =
            Combinators.Choice<Stmt, char>(
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
            Combinators.SepEndBy1(SimpleStatement, Control.Semicolon);

        private static readonly Parser<Stmt, char> Statement =
            (from list in StatementList
             from eol in Control.EOL
             select new Suite(list)).Try()
            .Or(Parsers.Indirect(() => CompoundStatement));

        private static Parser<Expr, char> ConditionHead(string keyword) =>
            from _ in Control.Keyword(keyword)
            from test in Expressions.Expression
            from __ in Control.Colon
            select test;

        private static readonly Parser<(Expr, Suite), char> If =
            Indentation.IndentBlockSome(
                Control.EOLWhitespace,
                ConditionHead("if"),
                Maybe.Nothing<IndentLevel>(),
                (test, stmts) => (test, new Suite(new List<Stmt>(stmts))),
                Statement
            );

        private static readonly Parser<(Expr, Suite), char> Elif =
            Indentation.IndentBlockSome(
                Control.EOLWhitespace,
                ConditionHead("elif"),
                Maybe.Nothing<IndentLevel>(),
                (test, stmts) => (test, new Suite(new List<Stmt>(stmts))),
                Statement
            );

        private static readonly Parser<Suite, char> Else =
            Indentation.IndentBlockSome(
                Control.EOLWhitespace,
                from _ in Control.Keyword("else") from __ in Control.Colon select None.Instance,
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
            Indentation.IndentBlockSome(
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
            from exprs in Expressions.ExpressionList
            from ___ in Control.Colon
            select (targets, exprs);

        private static readonly Parser<(IReadOnlyList<Expr>, IReadOnlyList<Expr>, Suite), char> For =
            Indentation.IndentBlockSome(
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
            Combinators.SepBy(Literals.Identifier, Control.Comma);

        private static readonly Parser<(IdentifierLiteral, IReadOnlyList<IdentifierLiteral>), char> FuncHead =
            from _ in Control.Keyword("def")
            from funcname in Literals.Identifier
            from parameterList in Combinators.Between(Control.OpenParan, ParameterList, Control.CloseParan)
            from __ in Control.Colon
            select (funcname, parameterList);

        private static readonly Parser<Function, char> FuncDefStatement =
            Indentation.IndentBlockSome(
                Control.EOLWhitespace,
                FuncHead,
                Maybe.Nothing<IndentLevel>(),
                (head, body) => new Function(head.Item1, head.Item2, Array.Empty<(IdentifierLiteral, Expr)>(), new Suite(new List<Stmt>(body))),
                Statement
            );

        private static readonly Parser<Stmt, char> CompoundStatement =
            Combinators.Choice<Stmt, char>(
                IfStatement,
                WhileStatement,
                ForStatement,
                FuncDefStatement
            );

        #endregion
    }
}
