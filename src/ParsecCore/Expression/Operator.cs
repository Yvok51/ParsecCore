using System;

namespace ParsecCore.Expression
{

    public interface IOperatorVisitor<T, TInput, V, U>
    {
        public V VisitPrefix(PrefixUnary<T, TInput> prefix, U arg);
        public V VisitPostfix(PostfixUnary<T, TInput> postfix, U arg);
        public V VisitBinary(InfixBinary<T, TInput> infix, U arg);
    }

    public abstract class Operator<T, TInput>
    {
        public abstract V Accept<U, V>(IOperatorVisitor<T, TInput, V, U> visitor, U arg);
    }

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
