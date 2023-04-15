using ParsecCore;

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

    internal interface ExprVisitor<T, A>
    {
        T VisitStringLiteral(StringLiteral literal, A arg);
        T VisitIntegerLiteral(IntegerLiteral literal, A arg);
        T VisitFloatLiteral(FloatLiteral literal, A arg);
        T VisitBooleanLiteral(BooleanLiteral literal, A arg);
        T VisitNoneLiteral(NoneLiteral literal, A arg);
        T VisitIdentifierLiteral(IdentifierLiteral literal, A arg);
        T VisitParenthForm(ParenthForm parenthForm, A arg);
        T VisitListDisplay(ListDisplay listDisplay, A arg);
        T VisitSetDisplay(SetDisplay setDisplay, A arg);
        T VisitKeyDatum(KeyDatum keyDatum, A arg);
        T VisitDictDisplay(DictDisplay dictDisplay, A arg);
        T VisitAtrributeRef(AttributeRef attributeRef, A arg);
        T VisitSubscription(Subscription subscription, A arg);
        T VisitSliceItem(SliceItem sliceItem, A arg);
        T VisitSlice(Slice slice, A arg);
        T VisitKeywordArgument(KeywordArgument keywordArgument, A arg);
        T VisitCall(Call call, A arg);
        T VisitUnary(Unary unary, A arg);
        T VisitBinary(Binary binary, A arg);
    }

    internal abstract class Expr
    {
        public abstract T Accept<T, A>(ExprVisitor<T, A> visitor, A arg);
    }

    internal class StringLiteral : Expr, IEquatable<StringLiteral>
    {
        public StringLiteral(string prefix, string value)
        {
            Prefix = prefix;
            Value = value;
        }

        public override T Accept<T, A>(ExprVisitor<T, A> visitor, A arg)
        {
            return visitor.VisitStringLiteral(this, arg);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as StringLiteral);
        }

        public bool Equals(StringLiteral? other)
        {
            return other != null && Prefix == other.Prefix && Value == other.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Prefix, Value);
        }

        public string Prefix { get; init; }
        public string Value { get; init; }
    }

    internal class IntegerLiteral : Expr, IEquatable<IntegerLiteral>
    {
        public IntegerLiteral(int value)
        {
            Value = value;
        }

        public override T Accept<T, A>(ExprVisitor<T, A> visitor, A arg)
        {
            return visitor.VisitIntegerLiteral(this, arg);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as IntegerLiteral);
        }

        public bool Equals(IntegerLiteral? other)
        {
            return other != null && Value == other.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }

        public int Value { get; init; }
    }

    internal class FloatLiteral : Expr, IEquatable<FloatLiteral>
    {
        public FloatLiteral(double value)
        {
            Value = value;
        }

        public override T Accept<T, A>(ExprVisitor<T, A> visitor, A arg)
        {
            return visitor.VisitFloatLiteral(this, arg);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as FloatLiteral);
        }

        public bool Equals(FloatLiteral? other)
        {
            return other != null && Value == other.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }

        public double Value { get; init; }
    }

    internal class BooleanLiteral : Expr
    {
        public BooleanLiteral(bool value)
        {
            Value = value;
        }

        public override T Accept<T, A>(ExprVisitor<T, A> visitor, A arg)
        {
            return visitor.VisitBooleanLiteral(this, arg);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as BooleanLiteral);
        }

        public bool Equals(BooleanLiteral? other)
        {
            return other != null && Value == other.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }

        public bool Value { get; init; }
    }

    internal class NoneLiteral : Expr
    {
        public NoneLiteral()
        {
        }

        public override T Accept<T, A>(ExprVisitor<T, A> visitor, A arg)
        {
            return visitor.VisitNoneLiteral(this, arg);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as NoneLiteral);
        }

        public bool Equals(NoneLiteral? other)
        {
            return other != null;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    internal class IdentifierLiteral : Expr, IEquatable<IdentifierLiteral>
    {
        public IdentifierLiteral(string name)
        {
            Name = name;
        }

        public override T Accept<T, A>(ExprVisitor<T, A> visitor, A arg)
        {
            return visitor.VisitIdentifierLiteral(this, arg);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as IdentifierLiteral);
        }

        public bool Equals(IdentifierLiteral? other)
        {
            return other != null && Name == other.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }

        public string Name { get; init; }
    }

    internal class ParenthForm : Expr, IEquatable<ParenthForm>
    {
        public ParenthForm(IReadOnlyList<Expr> expressions)
        {
            Expressions = expressions;
        }

        public override T Accept<T, A>(ExprVisitor<T, A> visitor, A arg)
        {
            return visitor.VisitParenthForm(this, arg);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as ParenthForm);
        }

        public bool Equals(ParenthForm? other)
        {
            return other != null && Enumerable.SequenceEqual(Expressions, other.Expressions);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Expressions);
        }

        public IReadOnlyList<Expr> Expressions { get; init; }
    }

    internal class ListDisplay : Expr, IEquatable<ListDisplay>
    {
        public ListDisplay(IReadOnlyList<Expr> expressions)
        {
            Expressions = expressions;
        }

        public override T Accept<T, A>(ExprVisitor<T, A> visitor, A arg)
        {
            return visitor.VisitListDisplay(this, arg);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as ListDisplay);
        }

        public bool Equals(ListDisplay? other)
        {
            return other != null && Enumerable.SequenceEqual(Expressions, other.Expressions);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Expressions);
        }

        public IReadOnlyList<Expr> Expressions { get; init; }
    }

    internal class SetDisplay : Expr, IEquatable<SetDisplay>
    {
        public SetDisplay(IReadOnlyList<Expr> expressions)
        {
            Expressions = expressions;
        }

        public override T Accept<T, A>(ExprVisitor<T, A> visitor, A arg)
        {
            return visitor.VisitSetDisplay(this, arg);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as SetDisplay);
        }

        public bool Equals(SetDisplay? other)
        {
            return other != null && Enumerable.SequenceEqual(Expressions, other.Expressions);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Expressions);
        }

        public IReadOnlyList<Expr> Expressions { get; init; }
    }

    internal class KeyDatum : Expr, IEquatable<KeyDatum>
    {
        public KeyDatum(Expr key, Expr value)
        {
            Key = key;
            Value = value;
        }

        public override T Accept<T, A>(ExprVisitor<T, A> visitor, A arg)
        {
            return visitor.VisitKeyDatum(this, arg);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as KeyDatum);
        }

        public bool Equals(KeyDatum? other)
        {
            return other != null && Key.Equals(other.Key) && Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Key, Value);
        }

        public Expr Key { get; init; }
        public Expr Value { get; init; }
    }

    internal class DictDisplay : Expr, IEquatable<DictDisplay>
    {
        public DictDisplay(IReadOnlyList<KeyDatum> data)
        {
            Data = data;
        }

        public override T Accept<T, A>(ExprVisitor<T, A> visitor, A arg)
        {
            return visitor.VisitDictDisplay(this, arg);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as DictDisplay);
        }

        public bool Equals(DictDisplay? other)
        {
            return other != null && Enumerable.SequenceEqual(Data, other.Data);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Data);
        }

        public IReadOnlyList<KeyDatum> Data { get; init; }
    }

    internal class AttributeRef : Expr, IEquatable<AttributeRef>
    {
        public AttributeRef(Expr obj, IdentifierLiteral attribute)
        {
            Obj = obj;
            Attribute = attribute;
        }

        public override T Accept<T, A>(ExprVisitor<T, A> visitor, A arg)
        {
            return visitor.VisitAtrributeRef(this, arg);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as AttributeRef);
        }

        public bool Equals(AttributeRef? other)
        {
            return other != null && Obj.Equals(other.Obj) && Attribute.Equals(other.Attribute);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Obj, Attribute);
        }

        public Expr Obj { get; init; }
        public IdentifierLiteral Attribute { get; init; }
    }

    internal class Subscription : Expr, IEquatable<Subscription>
    {
        public Subscription(Expr subscribable, IReadOnlyList<Expr> expressions)
        {
            Subscribable = subscribable;
            Expressions = expressions;
        }

        public override T Accept<T, A>(ExprVisitor<T, A> visitor, A arg)
        {
            return visitor.VisitSubscription(this, arg);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Subscription);
        }

        public bool Equals(Subscription? other)
        {
            return other != null
                && Subscribable.Equals(other.Subscribable)
                && Enumerable.SequenceEqual(Expressions, other.Expressions);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Subscribable, Expressions);
        }

        public Expr Subscribable { get; init; }
        public IReadOnlyList<Expr> Expressions { get; init; }
    }

    internal class SliceItem : Expr, IEquatable<SliceItem>
    {
        public SliceItem(Maybe<Expr> lowerBound, Maybe<Expr> upperBound, Maybe<Expr> stride)
        {
            UpperBound = upperBound;
            LowerBound = lowerBound;
            Stride = stride;
        }

        public override T Accept<T, A>(ExprVisitor<T, A> visitor, A arg)
        {
            return visitor.VisitSliceItem(this, arg);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as SliceItem);
        }

        public bool Equals(SliceItem? other)
        {
            return other != null
                && UpperBound.Equals(other.UpperBound)
                && LowerBound.Equals(other.LowerBound)
                && Stride.Equals(other.Stride);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(UpperBound, LowerBound, Stride);
        }

        public Maybe<Expr> UpperBound { get; init; }
        public Maybe<Expr> LowerBound { get; init; }
        public Maybe<Expr> Stride { get; init; }
    }

    internal class Slice : Expr, IEquatable<Slice>
    {
        public Slice(Expr slicable, IReadOnlyList<Expr> slices)
        {
            Slicable = slicable;
            Slices = slices;
        }

        public override T Accept<T, A>(ExprVisitor<T, A> visitor, A arg)
        {
            return visitor.VisitSlice(this, arg);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Slice);
        }

        public bool Equals(Slice? other)
        {
            return other != null && Slicable.Equals(other.Slicable) && Enumerable.SequenceEqual(Slices, other.Slices);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Slicable, Slices);
        }

        public Expr Slicable { get; init; }
        public IReadOnlyList<Expr> Slices { get; init; }
    }

    internal class KeywordArgument : Expr, IEquatable<KeywordArgument>
    {
        public KeywordArgument(IdentifierLiteral name, Expr value)
        {
            Name = name;
            Value = value;
        }

        public override T Accept<T, A>(ExprVisitor<T, A> visitor, A arg)
        {
            return visitor.VisitKeywordArgument(this, arg);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as KeywordArgument);
        }

        public bool Equals(KeywordArgument? other)
        {
            return other != null && Name.Equals(other.Name) && Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Value);
        }

        public IdentifierLiteral Name { get; init; }
        public Expr Value { get; init; }
    }

    internal class Call : Expr, IEquatable<Call>
    {
        public Call(
            Expr calledExpr,
            Maybe<IReadOnlyList<Expr>> argumentList,
            Maybe<IReadOnlyList<KeywordArgument>> keywordArguments,
            Maybe<Expr> sequenceExpr,
            Maybe<Expr> mappingExpr
        )
        {
            CalledExpr = calledExpr;
            ArgumentList = argumentList;
            KeywordArguments = keywordArguments;
            SequenceExpr = sequenceExpr;
            MappingExpr = mappingExpr;
        }

        public override T Accept<T, A>(ExprVisitor<T, A> visitor, A arg)
        {
            return visitor.VisitCall(this, arg);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Call);
        }

        public bool Equals(Call? other)
        {
            return other != null
                && CalledExpr.Equals(other.CalledExpr)
                && ArgumentList.Match(
                    just: (list)
                        => !other.ArgumentList.IsEmpty && Enumerable.SequenceEqual(list, other.ArgumentList.Value),
                    nothing: () => other.ArgumentList.IsEmpty
                )
                && KeywordArguments.Match(
                    just: (list)
                        => !other.KeywordArguments.IsEmpty
                        && Enumerable.SequenceEqual(list, other.KeywordArguments.Value),
                    nothing: () => other.KeywordArguments.IsEmpty
                )
                && SequenceExpr.Equals(other.SequenceExpr)
                && MappingExpr.Equals(other.MappingExpr);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CalledExpr, ArgumentList, KeywordArguments, SequenceExpr, MappingExpr);
        }

        public Expr CalledExpr { get; init; }
        public Maybe<IReadOnlyList<Expr>> ArgumentList { get; init; }
        public Maybe<IReadOnlyList<KeywordArgument>> KeywordArguments { get; init; }
        public Maybe<Expr> SequenceExpr { get; init; }
        public Maybe<Expr> MappingExpr { get; init; }
    }

    internal class Unary : Expr, IEquatable<Unary>
    {
        public Unary(Expr expression, UnaryOperator op)
        {
            Expression = expression;
            Operator = op;
        }

        public override T Accept<T, A>(ExprVisitor<T, A> visitor, A arg)
        {
            return visitor.VisitUnary(this, arg);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Unary);
        }

        public bool Equals(Unary? other)
        {
            return other != null && Expression.Equals(other.Expression) && Operator == other.Operator;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Expression, Operator);
        }

        public Expr Expression { get; init; }
        public UnaryOperator Operator { get; init; }
    }

    internal class Binary : Expr, IEquatable<Binary>
    {
        public Binary(Expr left, BinaryOperator op, Expr right)
        {
            Left = left;
            Right = right;
            Operator = op;
        }

        public override T Accept<T, A>(ExprVisitor<T, A> visitor, A arg)
        {
            return visitor.VisitBinary(this, arg);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Binary);
        }

        public bool Equals(Binary? other)
        {
            return other != null
                && Left.Equals(other.Left)
                && Right.Equals(other.Right)
                && Operator == other.Operator;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Left, Right, Operator);
        }

        public Expr Left { get; init; }
        public Expr Right { get; init; }
        public BinaryOperator Operator { get; init; }
    }
}
