using ParsecCore.MaybeNS;

namespace PythonParser.Structures
{
    internal interface StmtVisitor<T>
    {
        T VisitExpression(ExpressionStmt expression);
        T VisitAssignment(Assignment assignment);
        T VisitPass(Pass pass);
        T VisitReturn(Return @return);
        T VisitBreak(Break @break);
        T VisitContinue(Continue @continue);
        T VisitImport(ImportModule import);
        T VisitImportSpecific(ImportSpecific import);
        T VisitImportSpecificAll(ImportSpecificAll import);
        T VisitSuite(Suite suite);
        T VisitIf(If ifStatement);
        T VisitWhile(While ifStatement);
        T VisitFor(For forStatement);
        T VisitFunction(Function function);
    }

    internal abstract class Stmt
    {
        public abstract T Accept<T>(StmtVisitor<T> visitor);
    }

    #region Simple Statements

    internal class ExpressionStmt : Stmt
    {
        public ExpressionStmt(IReadOnlyList<Expr> expressions)
        {
            Expressions = expressions;
        }

        public override T Accept<T>(StmtVisitor<T> visitor)
        {
            return visitor.VisitExpression(this);
        }

        public IReadOnlyList<Expr> Expressions { get; init; }
    }

    internal class Assignment : Stmt
    {
        public Assignment(IReadOnlyList<IReadOnlyList<Expr>> targetList, IReadOnlyList<Expr> expressions)
        {
            TargetList = targetList;
            Expressions = expressions;
        }

        public override T Accept<T>(StmtVisitor<T> visitor)
        {
            return visitor.VisitAssignment(this);
        }

        public IReadOnlyList<IReadOnlyList<Expr>> TargetList { get; init; }
        public IReadOnlyList<Expr> Expressions { get; init; }
    }

    internal class Pass : Stmt
    {
        public Pass()
        {
        }

        public override T Accept<T>(StmtVisitor<T> visitor)
        {
            return visitor.VisitPass(this);
        }
    }

    internal class Return : Stmt
    {
        public Return(IMaybe<IReadOnlyList<Expr>> expressions) 
        {
            Expressions = expressions;
        }

        public override T Accept<T>(StmtVisitor<T> visitor)
        {
            return visitor.VisitReturn(this);
        }

        public IMaybe<IReadOnlyList<Expr>> Expressions { get; init; }
    }

    internal class Break : Stmt
    {
        public Break()
        {
        }

        public override T Accept<T>(StmtVisitor<T> visitor)
        {
            return visitor.VisitBreak(this);
        }
    }

    internal class Continue : Stmt
    {
        public Continue()
        {
        }

        public override T Accept<T>(StmtVisitor<T> visitor)
        {
            return visitor.VisitContinue(this);
        }
    }

    internal class ImportModule : Stmt
    {
        public ImportModule(IReadOnlyList<IdentifierLiteral> modulePath, IMaybe<IdentifierLiteral> alias)
        {
            ModulePath = modulePath;
            Alias = alias;
        }

        public override T Accept<T>(StmtVisitor<T> visitor)
        {
            return visitor.VisitImport(this);
        }

        public IReadOnlyList<IdentifierLiteral> ModulePath { get; init; }
        public IMaybe<IdentifierLiteral> Alias { get; init; }
    }

    internal class ImportSpecific : Stmt
    {
        public ImportSpecific(
            IReadOnlyList<IdentifierLiteral> modulePath,
            IdentifierLiteral specific,
            IMaybe<IdentifierLiteral> alias
        )
        {
            ModulePath = modulePath;
            Specific = specific;
            Alias = alias;
        }

        public override T Accept<T>(StmtVisitor<T> visitor)
        {
            return visitor.VisitImportSpecific(this);
        }

        public IReadOnlyList<IdentifierLiteral> ModulePath { get; init; }
        public IdentifierLiteral Specific { get; init; }
        public IMaybe<IdentifierLiteral> Alias { get; init; }
    }

    internal class ImportSpecificAll : Stmt
    {
        public ImportSpecificAll(IReadOnlyList<IdentifierLiteral> modulePath)
        {
            ModulePath = modulePath;
        }

        public override T Accept<T>(StmtVisitor<T> visitor)
        {
            return visitor.VisitImportSpecificAll(this);
        }

        public IReadOnlyList<IdentifierLiteral> ModulePath { get; init; }
    }

    #endregion

    #region Compound Statement

    internal class Suite : Stmt
    {
        public Suite(IReadOnlyList<Stmt> statements)
        {
            Statements = statements;
        }

        public override T Accept<T>(StmtVisitor<T> visitor)
        {
            return visitor.VisitSuite(this);
        }

        public IReadOnlyList<Stmt> Statements { get; init; }
    }

    internal class If : Stmt
    {
        public If(Expr test, Suite thenBranch, IReadOnlyList<(Expr, Suite)> elifs, IMaybe<Suite> elseBranch)
        {
            Test = test;
            ThenBranch = thenBranch;
            Elifs = elifs;
            ElseBranch = elseBranch;
        }

        public override T Accept<T>(StmtVisitor<T> visitor)
        {
            return visitor.VisitIf(this);
        }

        public Expr Test { get; init; }
        public Suite ThenBranch { get; init; }
        public IReadOnlyList<(Expr, Suite)> Elifs { get; init; }
        public IMaybe<Suite> ElseBranch { get; init; }
    }

    internal class While : Stmt
    {
        public While(Expr test, Suite body, IMaybe<Suite> elseBranch)
        {
            Test = test;
            Body = body;
            ElseBranch = elseBranch;
        }

        public override T Accept<T>(StmtVisitor<T> visitor)
        {
            return visitor.VisitWhile(this);
        }

        public Expr Test { get; init; }
        public Suite Body { get; init; }
        public IMaybe<Suite> ElseBranch { get; init; }
    }

    internal class For : Stmt
    {
        public For(
            IReadOnlyList<Expr> targets,
            IReadOnlyList<Expr> expressions,
            Suite body,
            IMaybe<Suite> elseBranch
        )
        {
            Targets = targets;
            Expressions = expressions;
            Body = body;
            ElseBranch = elseBranch;
        }

        public override T Accept<T>(StmtVisitor<T> visitor)
        {
            return visitor.VisitFor(this);
        }

        public IReadOnlyList<Expr> Targets { get; init; }
        public IReadOnlyList<Expr> Expressions { get; init; }
        public Suite Body { get; init; }
        public IMaybe<Suite> ElseBranch { get; init; }
    }

    internal class Function : Stmt
    {
        public Function(
            IdentifierLiteral name,
            IReadOnlyList<IdentifierLiteral> parameters,
            IReadOnlyList<(IdentifierLiteral, Expr)> defaultParameters,
            Suite body
        )
        {
            Name = name;
            Parameters = parameters;
            DefaultParameters = defaultParameters;
            Body = body;
        }

        public override T Accept<T>(StmtVisitor<T> visitor)
        {
            return visitor.VisitFunction(this);
        }

        public IdentifierLiteral Name { get; init; }
        public IReadOnlyList<IdentifierLiteral> Parameters { get; init; }
        public IReadOnlyList<(IdentifierLiteral, Expr)> DefaultParameters { get; init; }
        public Suite Body { get; init; }

    }

    #endregion
}
