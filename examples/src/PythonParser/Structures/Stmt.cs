using ParsecCore;

namespace PythonParser.Structures
{
    internal interface StmtVisitor<T, A>
    {
        T VisitExpression(ExpressionStmt expression, A arg);
        T VisitAssignment(Assignment assignment, A arg);
        T VisitPass(Pass pass, A arg);
        T VisitReturn(Return @return, A arg);
        T VisitBreak(Break @break, A arg);
        T VisitContinue(Continue @continue, A arg);
        T VisitImport(ImportModule import, A arg);
        T VisitImportSpecific(ImportSpecific import, A arg);
        T VisitImportSpecificAll(ImportSpecificAll import, A arg);
        T VisitSuite(Suite suite, A arg);
        T VisitIf(If ifStatement, A arg);
        T VisitWhile(While whileStatement, A arg);
        T VisitFor(For forStatement, A arg);
        T VisitFunction(Function function, A arg);
    }

    internal abstract class Stmt
    {
        public abstract T Accept<T, A>(StmtVisitor<T, A> visitor, A arg);
    }

    #region Simple Statements

    internal class ExpressionStmt : Stmt, IEquatable<ExpressionStmt>
    {
        public ExpressionStmt(IReadOnlyList<Expr> expressions)
        {
            Expressions = expressions;
        }

        public override T Accept<T, A>(StmtVisitor<T, A> visitor, A arg)
        {
            return visitor.VisitExpression(this, arg);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as ExpressionStmt);
        }

        public bool Equals(ExpressionStmt? other)
        {
            return other != null && Enumerable.SequenceEqual(Expressions, other.Expressions);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Expressions);
        }

        public IReadOnlyList<Expr> Expressions { get; init; }
    }

    internal class Assignment : Stmt, IEquatable<Assignment>
    {
        public Assignment(IReadOnlyList<IReadOnlyList<Expr>> targetList, IReadOnlyList<Expr> expressions)
        {
            TargetList = targetList;
            Expressions = expressions;
        }

        public override T Accept<T, A>(StmtVisitor<T, A> visitor, A arg)
        {
            return visitor.VisitAssignment(this, arg);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Assignment);
        }

        public bool Equals(Assignment? other)
        {
            return other != null
                && TargetList.Zip(other.TargetList).All(tuple => Enumerable.SequenceEqual(tuple.First, tuple.Second))
                && Enumerable.SequenceEqual(Expressions, other.Expressions);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TargetList, Expressions);
        }

        public IReadOnlyList<IReadOnlyList<Expr>> TargetList { get; init; }
        public IReadOnlyList<Expr> Expressions { get; init; }
    }

    internal class Pass : Stmt, IEquatable<Pass>
    {
        public Pass()
        {
        }

        public override T Accept<T, A>(StmtVisitor<T, A> visitor, A arg)
        {
            return visitor.VisitPass(this, arg);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Pass);
        }

        public bool Equals(Pass? other)
        {
            return other != null;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    internal class Return : Stmt, IEquatable<Return>
    {
        public Return(Maybe<IReadOnlyList<Expr>> expressions)
        {
            Expressions = expressions;
        }

        public override T Accept<T, A>(StmtVisitor<T, A> visitor, A arg)
        {
            return visitor.VisitReturn(this, arg);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Return);
        }

        public bool Equals(Return? other)
        {
            return other != null && Expressions.Match(
                just: (list) => !other.Expressions.IsEmpty && Enumerable.SequenceEqual(list, other.Expressions.Value),
                nothing: () => other.Expressions.IsEmpty
            );
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Expressions);
        }

        public Maybe<IReadOnlyList<Expr>> Expressions { get; init; }
    }

    internal class Break : Stmt, IEquatable<Break>
    {
        public Break()
        {
        }

        public override T Accept<T, A>(StmtVisitor<T, A> visitor, A arg)
        {
            return visitor.VisitBreak(this, arg);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Break);
        }

        public bool Equals(Break? other)
        {
            return other != null;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    internal class Continue : Stmt, IEquatable<Continue>
    {
        public Continue()
        {
        }

        public override T Accept<T, A>(StmtVisitor<T, A> visitor, A arg)
        {
            return visitor.VisitContinue(this, arg);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Continue);
        }

        public bool Equals(Continue? other)
        {
            return other != null;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    internal class ImportModule : Stmt, IEquatable<ImportModule>
    {
        public ImportModule(IReadOnlyList<IdentifierLiteral> modulePath, Maybe<IdentifierLiteral> alias)
        {
            ModulePath = modulePath;
            Alias = alias;
        }

        public override T Accept<T, A>(StmtVisitor<T, A> visitor, A arg)
        {
            return visitor.VisitImport(this, arg);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as ImportModule);
        }

        public bool Equals(ImportModule? other)
        {
            return other != null
                && Enumerable.SequenceEqual(ModulePath, other.ModulePath)
                && Alias.Equals(other.Alias);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ModulePath, Alias);
        }

        public IReadOnlyList<IdentifierLiteral> ModulePath { get; init; }
        public Maybe<IdentifierLiteral> Alias { get; init; }
    }

    internal class ImportSpecific : Stmt, IEquatable<ImportSpecific>
    {
        public ImportSpecific(
            IReadOnlyList<IdentifierLiteral> modulePath,
            IdentifierLiteral specific,
            Maybe<IdentifierLiteral> alias
        )
        {
            ModulePath = modulePath;
            Specific = specific;
            Alias = alias;
        }

        public override T Accept<T, A>(StmtVisitor<T, A> visitor, A arg)
        {
            return visitor.VisitImportSpecific(this, arg);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as ImportSpecific);
        }

        public bool Equals(ImportSpecific? other)
        {
            return other != null
                && Enumerable.SequenceEqual(ModulePath, other.ModulePath)
                && Specific.Equals(other.Specific)
                && Alias.Equals(other.Alias);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ModulePath, Alias);
        }

        public IReadOnlyList<IdentifierLiteral> ModulePath { get; init; }
        public IdentifierLiteral Specific { get; init; }
        public Maybe<IdentifierLiteral> Alias { get; init; }
    }

    internal class ImportSpecificAll : Stmt, IEquatable<ImportSpecificAll>
    {
        public ImportSpecificAll(IReadOnlyList<IdentifierLiteral> modulePath)
        {
            ModulePath = modulePath;
        }

        public override T Accept<T, A>(StmtVisitor<T, A> visitor, A arg)
        {
            return visitor.VisitImportSpecificAll(this, arg);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as ImportSpecificAll);
        }

        public bool Equals(ImportSpecificAll? other)
        {
            return other != null && Enumerable.SequenceEqual(ModulePath, other.ModulePath);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ModulePath);
        }

        public IReadOnlyList<IdentifierLiteral> ModulePath { get; init; }
    }

    #endregion

    #region Compound Statement

    internal class Suite : Stmt, IEquatable<Suite>
    {
        public Suite(IReadOnlyList<Stmt> statements)
        {
            Statements = statements;
        }

        public override T Accept<T, A>(StmtVisitor<T, A> visitor, A arg)
        {
            return visitor.VisitSuite(this, arg);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Suite);
        }

        public bool Equals(Suite? other)
        {
            return other != null && Enumerable.SequenceEqual(Statements, other.Statements);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Statements);
        }

        public IReadOnlyList<Stmt> Statements { get; init; }
    }

    internal class If : Stmt, IEquatable<If>
    {
        public If(Expr test, Suite thenBranch, IReadOnlyList<(Expr, Suite)> elifs, Maybe<Suite> elseBranch)
        {
            Test = test;
            ThenBranch = thenBranch;
            Elifs = elifs;
            ElseBranch = elseBranch;
        }

        public override T Accept<T, A>(StmtVisitor<T, A> visitor, A arg)
        {
            return visitor.VisitIf(this, arg);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as If);
        }

        public bool Equals(If? other)
        {
            return other != null
                && Test.Equals(other.Test)
                && ThenBranch.Equals(other.ThenBranch)
                && Enumerable.SequenceEqual(Elifs, other.Elifs)
                && ElseBranch.Equals(other.ElseBranch);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Test, ThenBranch, Elifs, ElseBranch);
        }

        public Expr Test { get; init; }
        public Suite ThenBranch { get; init; }
        public IReadOnlyList<(Expr, Suite)> Elifs { get; init; }
        public Maybe<Suite> ElseBranch { get; init; }
    }

    internal class While : Stmt, IEquatable<While>
    {
        public While(Expr test, Suite body, Maybe<Suite> elseBranch)
        {
            Test = test;
            Body = body;
            ElseBranch = elseBranch;
        }

        public override T Accept<T, A>(StmtVisitor<T, A> visitor, A arg)
        {
            return visitor.VisitWhile(this, arg);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as While);
        }

        public bool Equals(While? other)
        {
            return other != null
                && Test.Equals(other.Test)
                && Body.Equals(other.Body)
                && ElseBranch.Equals(other.ElseBranch);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Test, Body, ElseBranch);
        }

        public Expr Test { get; init; }
        public Suite Body { get; init; }
        public Maybe<Suite> ElseBranch { get; init; }
    }

    internal class For : Stmt, IEquatable<For>
    {
        public For(
            IReadOnlyList<Expr> targets,
            IReadOnlyList<Expr> expressions,
            Suite body,
            Maybe<Suite> elseBranch
        )
        {
            Targets = targets;
            Expressions = expressions;
            Body = body;
            ElseBranch = elseBranch;
        }

        public override T Accept<T, A>(StmtVisitor<T, A> visitor, A arg)
        {
            return visitor.VisitFor(this, arg);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as For);
        }

        public bool Equals(For? other)
        {
            return other != null
                && Enumerable.SequenceEqual(Targets, other.Targets)
                && Enumerable.SequenceEqual(Expressions, other.Expressions)
                && Body.Equals(other.Body)
                && ElseBranch.Equals(other.ElseBranch);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Targets, Expressions, Body, ElseBranch);
        }

        public IReadOnlyList<Expr> Targets { get; init; }
        public IReadOnlyList<Expr> Expressions { get; init; }
        public Suite Body { get; init; }
        public Maybe<Suite> ElseBranch { get; init; }
    }

    internal class Function : Stmt, IEquatable<Function>
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

        public override T Accept<T, A>(StmtVisitor<T, A> visitor, A arg)
        {
            return visitor.VisitFunction(this, arg);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Function);
        }

        public bool Equals(Function? other)
        {
            return other != null
                && Name.Equals(other.Name)
                && Enumerable.SequenceEqual(Parameters, other.Parameters)
                && Enumerable.SequenceEqual(DefaultParameters, other.DefaultParameters)
                && Body.Equals(other.Body);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Parameters, DefaultParameters, Body);
        }


        public IdentifierLiteral Name { get; init; }
        public IReadOnlyList<IdentifierLiteral> Parameters { get; init; }
        public IReadOnlyList<(IdentifierLiteral, Expr)> DefaultParameters { get; init; }
        public Suite Body { get; init; }

    }

    #endregion
}
