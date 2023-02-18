using ParsecCore.MaybeNS;
using PythonParser.Parser;
using static System.Net.Mime.MediaTypeNames;

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

    internal class ExpressionStmt : Stmt, IEquatable<ExpressionStmt>
    {
        public ExpressionStmt(IReadOnlyList<Expr> expressions)
        {
            Expressions = expressions;
        }

        public override T Accept<T>(StmtVisitor<T> visitor)
        {
            return visitor.VisitExpression(this);
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

        public override T Accept<T>(StmtVisitor<T> visitor)
        {
            return visitor.VisitAssignment(this);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Assignment);
        }

        public bool Equals(Assignment? other)
        {
            return other != null 
                && Enumerable.SequenceEqual(TargetList, other.TargetList)
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

        public override T Accept<T>(StmtVisitor<T> visitor)
        {
            return visitor.VisitPass(this);
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
        public Return(IMaybe<IReadOnlyList<Expr>> expressions) 
        {
            Expressions = expressions;
        }

        public override T Accept<T>(StmtVisitor<T> visitor)
        {
            return visitor.VisitReturn(this);
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

        public IMaybe<IReadOnlyList<Expr>> Expressions { get; init; }
    }

    internal class Break : Stmt, IEquatable<Break>
    {
        public Break()
        {
        }

        public override T Accept<T>(StmtVisitor<T> visitor)
        {
            return visitor.VisitBreak(this);
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

        public override T Accept<T>(StmtVisitor<T> visitor)
        {
            return visitor.VisitContinue(this);
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
        public ImportModule(IReadOnlyList<IdentifierLiteral> modulePath, IMaybe<IdentifierLiteral> alias)
        {
            ModulePath = modulePath;
            Alias = alias;
        }

        public override T Accept<T>(StmtVisitor<T> visitor)
        {
            return visitor.VisitImport(this);
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
        public IMaybe<IdentifierLiteral> Alias { get; init; }
    }

    internal class ImportSpecific : Stmt, IEquatable<ImportSpecific>
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
        public IMaybe<IdentifierLiteral> Alias { get; init; }
    }

    internal class ImportSpecificAll : Stmt, IEquatable<ImportSpecificAll>
    {
        public ImportSpecificAll(IReadOnlyList<IdentifierLiteral> modulePath)
        {
            ModulePath = modulePath;
        }

        public override T Accept<T>(StmtVisitor<T> visitor)
        {
            return visitor.VisitImportSpecificAll(this);
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

        public override T Accept<T>(StmtVisitor<T> visitor)
        {
            return visitor.VisitSuite(this);
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
        public IMaybe<Suite> ElseBranch { get; init; }
    }

    internal class While : Stmt, IEquatable<While>
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
        public IMaybe<Suite> ElseBranch { get; init; }
    }

    internal class For : Stmt, IEquatable<For>
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
        public IMaybe<Suite> ElseBranch { get; init; }
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

        public override T Accept<T>(StmtVisitor<T> visitor)
        {
            return visitor.VisitFunction(this);
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
