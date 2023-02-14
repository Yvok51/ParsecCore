
using ParsecCore;
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

        #endregion

        #region Compound Statement

        #endregion
    }
}
