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
                from id in Control.Dot(Control.Lexeme).Then(Literals.Identifier(Control.Lexeme))
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
            Control.Lexeme.Create(Control.Keyword("pass")).MapConstant(new Pass());

        private static readonly Parser<Return, char> Return =
            from ret in Control.Lexeme.Create(Control.Keyword("return"))
            from exprs in Expressions.ExpressionList(Control.Lexeme).Optional()
            select new Return(exprs);

        private static readonly Parser<Break, char> Break =
            Control.Keyword("break").MapConstant(new Break());

        private static readonly Parser<Continue, char> Continue =
            Control.Keyword("continue").MapConstant(new Continue());

        private static readonly Parser<IReadOnlyList<IdentifierLiteral>, char> ModulePath =
            Parsers.SepBy1(Literals.Identifier(Control.Lexeme), Control.Dot(Control.Lexeme));

        private static readonly Parser<ImportModule, char> ImportModule =
            from import in Control.Keyword("import")
            from modulePath in ModulePath
            from alias in Control.Keyword("as").Then(Literals.Identifier(Control.Lexeme)).Optional()
            select new ImportModule(modulePath, alias);

        private static readonly Parser<ImportSpecific, char> ImportSpecific =
            from modulePath in Parsers.Between(Control.Keyword("from"), ModulePath, Control.Keyword("import"))
            from name in Literals.Identifier(Control.Lexeme)
            from alias in Control.Keyword("as").Then(Literals.Identifier(Control.Lexeme)).Optional()
            select new ImportSpecific(modulePath, name, alias);

        private static readonly Parser<ImportSpecificAll, char> ImportSpecificAll =
            Parsers.Between(
                Control.Keyword("from"),
                ModulePath,
                Control.Keyword("import")
            ).FollowedBy(Control.Asterisk(Control.Lexeme)
            ).Map(modulePath => new ImportSpecificAll(modulePath));

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
                from eol in Control.EOL.Or(Parsers.EOF<char>().MapConstant('\n'))
                let stmt = list.Count == 1 ? list[0] : new Suite(list)
                select stmt
            );

        private static Parser<Expr, char> ConditionHead(string keyword) =>
            Parsers.Between(Control.Keyword(keyword), Expressions.Expression(Control.Lexeme), Control.Colon(Control.Lexeme));

        private static readonly Parser<(Expr cond, Suite body), char> If =
            Indentation.BlockMany1(
                Control.EOLWhitespace,
                ConditionHead("if"),
                Statement,
                (test, stmts) => (test, new Suite(new List<Stmt>(stmts)))
            );

        private static readonly Parser<(Expr, Suite), char> Elif =
            Indentation.BlockMany1(
                Control.EOLWhitespace,
                ConditionHead("elif"),
                Statement,
                (test, stmts) => (test, new Suite(new List<Stmt>(stmts)))
            );

        private static readonly Parser<Suite, char> Else =
            Indentation.BlockMany1(
                Control.EOLWhitespace,
                Control.Keyword("else").Then(Control.Colon(Control.Lexeme)).Void(),
                Statement,
                (_, stmts) => new Suite(new List<Stmt>(stmts))
            );

        // TODO: add support for If on one line
        private static readonly Parser<If, char> IfStatement =
            from referenceLvl in Indentation.Level<char>()
            from @if in If
            from elifs in Indentation.Many(referenceLvl, Relation.EQ, Control.EOLWhitespace, Elif)
            from @else in Indentation.Optional(referenceLvl, Relation.EQ, Else)
            select new If(@if.cond, @if.body, elifs, @else);

        private static readonly Parser<(Expr cond, Suite body), char> While =
            Indentation.BlockMany1(
                Control.EOLWhitespace,
                ConditionHead("while"),
                Statement,
                (test, stmts) => (test, new Suite(new List<Stmt>(stmts)))
            );

        // TODO: add support for While on one line
        private static readonly Parser<While, char> WhileStatement =
            from referenceLvl in Indentation.Level<char>()
            from @while in While
            from @else in Indentation.Optional(referenceLvl, Relation.EQ, Else)
            select new While(@while.cond, @while.body, @else);

        private static readonly Parser<(IReadOnlyList<Expr> targets, IReadOnlyList<Expr> exprs), char> ForHead =
            from targets in Parsers.Between(Control.Keyword("for"), TargetList, Control.Keyword("in"))
            from exprs in Expressions.ExpressionList(Control.Lexeme).FollowedBy(Control.Colon(Control.Lexeme))
            select (targets, exprs);

        private static readonly 
            Parser<(IReadOnlyList<Expr> targets, IReadOnlyList<Expr> exprs, Suite body), char> For =
                Indentation.BlockMany1(
                    Control.EOLWhitespace,
                    ForHead,
                    Statement,
                    (head, stmts) => (head.targets, head.exprs, new Suite(new List<Stmt>(stmts)))
                );

        private static readonly Parser<For, char> ForStatement =
            from referenceLvl in Indentation.Level<char>()
            from @for in For
            from @else in Indentation.Optional(referenceLvl, Relation.EQ, Else)
            select new For(@for.targets, @for.exprs, @for.body, @else);

        private static readonly Parser<IReadOnlyList<IdentifierLiteral>, char> ParameterList =
            Parsers.SepBy(Literals.Identifier(Control.EOLLexeme), Control.Comma(Control.EOLLexeme));

        private static readonly 
            Parser<(IdentifierLiteral name, IReadOnlyList<IdentifierLiteral> parameters), char> FuncHead =
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
            Indentation.BlockMany1(
                Control.EOLWhitespace,
                FuncHead,
                Statement,
                (head, body) => new Function(
                    head.name,
                    head.parameters,
                    Array.Empty<(IdentifierLiteral, Expr)>(), new Suite(new List<Stmt>(body))
                )
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
