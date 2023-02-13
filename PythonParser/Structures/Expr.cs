using ParsecCore.MaybeNS;

namespace PythonParser.Structures
{
    internal enum BinaryOperator
    {
        Minus, Plus, Slash, Star, DoubleStar, DoubleSlash, Modulo,
        LT, LE, GT, GE, Equal, NotEqual,
        Is, IsNot, In, NotIn,
        Or, And
    }

    internal enum UnaryOperator
    {
        Minus, Plus, Not
    }

    internal interface ExprVisitor<T>
    {
        T VisitStringLiteral(StringLiteral literal);
        T VisitIntegerLiteral(IntegerLiteral literal);
        T VisitFloatLiteral(FloatLiteral literal);
        T VisitIdentifierLiteral(IdentifierLiteral literal);
        T VisitParenthForm(ParenthForm parenthForm);
        T VisitListDisplay(ListDisplay listDisplay);
        T VisitSetDisplay(SetDisplay setDisplay);
        T VisitKeyDatum(KeyDatum setDisplay);
        T VisitDictDisplay(DictDisplay keyDisplay);
        T VisitAtrributeRef(AttributeRef attributeRef);
        T VisitSubscription(Subscription subscription);
        T VisitSliceItem(SliceItem sliceItem);
        T VisitSlice(Slice slice);
        T VisitKeywordArgument(KeywordArgument keywordArgument);
        T VisitCall(Call call);
        T VisitUnary(Unary unary);
        T VisitBinary(Binary binary);
    }

    internal abstract class Expr
    {
        public abstract T Accept<T>(ExprVisitor<T> visitor);
    }

    internal class StringLiteral : Expr
    {
        public StringLiteral(string prefix, string value)
        {
            Prefix = prefix;
            Value = value;
        }

        public override T Accept<T>(ExprVisitor<T> visitor)
        {
            return visitor.VisitStringLiteral(this);
        }

        public string Prefix { get; init; }
        public string Value { get; init; }
    }

    internal class IntegerLiteral : Expr
    {
        public IntegerLiteral(int value)
        {
            Value = value;
        }

        public override T Accept<T>(ExprVisitor<T> visitor)
        {
            return visitor.VisitIntegerLiteral(this);
        }

        public int Value { get; init; }
    }

    internal class FloatLiteral : Expr
    {
        public FloatLiteral(double value)
        {
            Value = value;
        }

        public override T Accept<T>(ExprVisitor<T> visitor)
        {
            return visitor.VisitFloatLiteral(this);
        }

        public double Value { get; init; }
    }

    internal class IdentifierLiteral : Expr
    {
        public IdentifierLiteral(string name)
        {
            Name = name;
        }

        public override T Accept<T>(ExprVisitor<T> visitor)
        {
            return visitor.VisitIdentifierLiteral(this);
        }

        public string Name { get; init; }
    }

    internal class ParenthForm : Expr
    {
        public ParenthForm(IReadOnlyList<Expr> expressions)
        {
            Expressions = expressions;
        }

        public override T Accept<T>(ExprVisitor<T> visitor)
        {
            return visitor.VisitParenthForm(this);
        }

        public IReadOnlyList<Expr> Expressions { get; init; }
    }

    internal class ListDisplay : Expr
    {
        public ListDisplay(IReadOnlyList<Expr> expressions)
        {
            Expressions = expressions;
        }

        public override T Accept<T>(ExprVisitor<T> visitor)
        {
            return visitor.VisitListDisplay(this);
        }

        public IReadOnlyList<Expr> Expressions { get; init; }
    }

    internal class SetDisplay : Expr
    {
        public SetDisplay(IReadOnlyList<Expr> expressions)
        {
            Expressions = expressions;
        }

        public override T Accept<T>(ExprVisitor<T> visitor)
        {
            return visitor.VisitSetDisplay(this);
        }

        public IReadOnlyList<Expr> Expressions { get; init; }
    }

    internal class KeyDatum : Expr
    {
        public KeyDatum(Expr key, Expr value)
        {
            Key = key;
            Value = value;
        }

        public override T Accept<T>(ExprVisitor<T> visitor)
        {
            return visitor.VisitKeyDatum(this);
        }

        public Expr Key { get; init; }
        public Expr Value { get; init; }
    }

    internal class DictDisplay : Expr
    {
        public DictDisplay(IReadOnlyList<KeyDatum> data)
        {
            Data = data;
        }

        public override T Accept<T>(ExprVisitor<T> visitor)
        {
            return visitor.VisitDictDisplay(this);
        }

        public IReadOnlyList<KeyDatum> Data { get; init; }
    }

    internal class AttributeRef : Expr
    {
        public AttributeRef(Expr obj, IdentifierLiteral attribute)
        {
            Obj = obj;
            Attribute = attribute;
        }

        public override T Accept<T>(ExprVisitor<T> visitor)
        {
            return visitor.VisitAtrributeRef(this);
        }

        public Expr Obj { get; init; }
        public IdentifierLiteral Attribute { get; init; }
    }

    internal class Subscription : Expr
    {
        public Subscription(Expr subscribable, IReadOnlyList<Expr> expressions)
        {
            Subscribable = subscribable;
            Expressions = expressions;
        }

        public override T Accept<T>(ExprVisitor<T> visitor)
        {
            return visitor.VisitSubscription(this);
        }

        public Expr Subscribable { get; init; }
        public IReadOnlyList<Expr> Expressions { get; init; }
    }

    internal class SliceItem : Expr
    {
        public SliceItem(IMaybe<Expr> upperBound, IMaybe<Expr> lowerBound, IMaybe<Expr> stride)
        {
            UpperBound = upperBound;
            LowerBound = lowerBound;
            Stride = stride;
        }

        public override T Accept<T>(ExprVisitor<T> visitor)
        {
            return visitor.VisitSliceItem(this);
        }

        public IMaybe<Expr> UpperBound { get; init; }
        public IMaybe<Expr> LowerBound { get; init; }
        public IMaybe<Expr> Stride { get; init; }
    }

    internal class Slice : Expr
    {
        public Slice(Expr slicable, IReadOnlyList<Expr> slices)
        {
            Slicable = slicable;
            Slices = slices;
        }

        public override T Accept<T>(ExprVisitor<T> visitor)
        {
            return visitor.VisitSlice(this);
        }

        public Expr Slicable { get; init; }
        public IReadOnlyList<Expr> Slices { get; init; }
    }

    internal class KeywordArgument : Expr
    {
        public KeywordArgument(IdentifierLiteral name, Expr value)
        {
            Name = name;
            Value = value;
        }

        public override T Accept<T>(ExprVisitor<T> visitor)
        {
            return visitor.VisitKeywordArgument(this);
        }

        public IdentifierLiteral Name { get; init; }
        public Expr Value { get; init; }
    }

    internal class Call : Expr
    {
        public Call(Expr calledExpr, IMaybe<IReadOnlyList<Expr>> argumentList, IMaybe<IReadOnlyList<KeywordArgument>> keywordArguments, IMaybe<Expr> sequenceExpr, IMaybe<Expr> mappingExpr)
        {
            CalledExpr = calledExpr;
            ArgumentList = argumentList;
            KeywordArguments = keywordArguments;
            SequenceExpr = sequenceExpr;
            MappingExpr = mappingExpr;
        }

        public override T Accept<T>(ExprVisitor<T> visitor)
        {
            return visitor.VisitCall(this);
        }

        public Expr CalledExpr { get; init; }
        public IMaybe<IReadOnlyList<Expr>> ArgumentList { get; init; }
        public IMaybe<IReadOnlyList<KeywordArgument>> KeywordArguments { get; init; }
        public IMaybe<Expr> SequenceExpr { get; init; }
        public IMaybe<Expr> MappingExpr { get; init; }
    }

    internal class Unary : Expr
    {
        public Unary(Expr expression, UnaryOperator op)
        {
            Expression = expression;
            Operator = op;
        }

        public override T Accept<T>(ExprVisitor<T> visitor)
        {
            return visitor.VisitUnary(this);
        }

        public Expr Expression { get; init; }
        public UnaryOperator Operator { get; init; }
    }

    internal class Binary : Expr
    {
        public Binary(Expr left, BinaryOperator op, Expr right)
        {
            Left = left;
            Right = right;
            Operator = op;
        }

        public override T Accept<T>(ExprVisitor<T> visitor)
        {
            return visitor.VisitBinary(this);
        }

        public Expr Left { get; init; }
        public Expr Right { get; init; }
        public BinaryOperator Operator { get; init; }
    }
}
