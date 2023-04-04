using System;

namespace ParsecCore.Expression
{

    public interface IOperatorVisitor<T, TInput, V, U>
    {
        public V VisitPrefix(PrefixUnary<T, TInput> prefix, U arg);
        public V VisitPostfix(PostfixUnary<T, TInput> postfix, U arg);
        public V VisitBinary(InfixBinary<T, TInput> infix, U arg);
    }

    /// <summary>
    /// Operator for the definition of an expression parser.
    /// See 
    /// <see cref="Expression.Build{T, TInput}(System.Collections.Generic.IReadOnlyList{System.Collections.Generic.IReadOnlyList{Operator{T, TInput}}}, Parser{T, TInput})"/>
    /// for more details.
    /// </summary>
    /// <typeparam name="T"> The type of the simple expression (term) </typeparam>
    /// <typeparam name="TInput"> The type of the input the expression parser takes as input </typeparam>
    public abstract class Operator<T, TInput>
    {
        public abstract V Accept<U, V>(IOperatorVisitor<T, TInput, V, U> visitor, U arg);
    }

    /// <summary>
    /// Class for prefix operator parser 
    /// </summary>
    /// <typeparam name="T"> The type of the simple expression (term) </typeparam>
    /// <typeparam name="TInput"> The type of the input the expression parser takes as input </typeparam>
    public class PrefixUnary<T, TInput> : Operator<T, TInput>
    {
        public PrefixUnary(Parser<Func<T, T>, TInput> parser)
        {
            Parser = parser;
        }

        public Parser<Func<T, T>, TInput> Parser { get; init; }

        public override V Accept<U, V>(IOperatorVisitor<T, TInput, V, U> visitor, U arg)
        {
            return visitor.VisitPrefix(this, arg);
        }
    }

    /// <summary>
    /// Class for postfix operator parser 
    /// </summary>
    /// <typeparam name="T"> The type of the simple expression (term) </typeparam>
    /// <typeparam name="TInput"> The type of the input the expression parser takes as input </typeparam>
    public class PostfixUnary<T, TInput> : Operator<T, TInput>
    {
        public PostfixUnary(Parser<Func<T, T>, TInput> parser)
        {
            Parser = parser;
        }

        public Parser<Func<T, T>, TInput> Parser { get; init; }

        public override V Accept<U, V>(IOperatorVisitor<T, TInput, V, U> visitor, U arg)
        {
            return visitor.VisitPostfix(this, arg);
        }
    }

    /// <summary>
    /// Class for infix binary operator parser 
    /// </summary>
    /// <typeparam name="T"> The type of the simple expression (term) </typeparam>
    /// <typeparam name="TInput"> The type of the input the expression parser takes as input </typeparam>
    public class InfixBinary<T, TInput> : Operator<T, TInput>
    {
        public InfixBinary(Parser<Func<T, T, T>, TInput> parser, Associativity associativity)
        {
            Parser = parser;
            Associativity = associativity;
        }

        public Parser<Func<T, T, T>, TInput> Parser { get; init; }

        public Associativity Associativity { get; init; }

        public override V Accept<U, V>(IOperatorVisitor<T, TInput, V, U> visitor, U arg)
        {
            return visitor.VisitBinary(this, arg);
        }
    }
}
