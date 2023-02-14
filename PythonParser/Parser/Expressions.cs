using ParsecCore;
using ParsecCore.MaybeNS;
using PythonParser.Structures;

namespace PythonParser.Parser
{
    internal static class Expressions
    {
        public static readonly Parser<Expr, char> Expression = Control.Lexeme.Create(Parsers.Indirect(() => OrTest));

        public static readonly Parser<IReadOnlyList<Expr>, char> ExpressionList =
            Combinators.SepEndBy1(Expression, Control.Comma);

        private static Parser<ParenthForm, char> ParenthForm =
            Combinators.Between(Control.OpenParan, ExpressionList, Control.CloseParan)
            .Map(list => new ParenthForm(list));

        // Does not include comprehensions

        private static readonly Parser<ListDisplay, char> ListDisplay =
            Combinators.Between(Control.OpenBracket, ExpressionList, Control.CloseBracket)
            .Map(list => new ListDisplay(list));

        private static readonly Parser<SetDisplay, char> SetDisplay =
            Combinators.Between(Control.OpenBracket, ExpressionList, Control.CloseBracket)
            .Map(list => new SetDisplay(list));

        private static readonly Parser<KeyDatum, char> KeyDatum = 
            from key in Expression
            from sep in Control.Colon
            from val in Expression
            select new KeyDatum(key, val);

        private static readonly Parser<IReadOnlyList<KeyDatum>, char> KeyDatumList =
            Combinators.SepEndBy1(KeyDatum, Control.Comma);

        private static readonly Parser<DictDisplay, char> DictDisplay =
            Combinators.Between(Control.OpenBrace, KeyDatumList, Control.CloseBrace)
            .Map(data => new DictDisplay(data));

        // Does not include yield and generator expressions

        private static readonly Parser<Expr, char> Enclosure =
            Combinators.Choice<Expr, char>(
                ParenthForm,
                ListDisplay,
                DictDisplay.Try(),
                SetDisplay
            );

        public static readonly Parser<Expr, char> Atom = 
            Combinators.Choice(
                Literals.Identifier,
                Literals.Literal,
                Enclosure
            );

        private static T Foldl<T>(IReadOnlyList<Func<T, T>> ts, T start)
        {
            T curr = start;
            foreach (var t in ts)
            {
                curr = t(curr);
            }
            return curr;
        }

        private static readonly Parser<SliceItem, char> ProperSlice =
            from lower in Expression.Optional()
            from colon in Control.Colon
            from upper in Expression.Optional()
            from stride in (from _ in Control.Colon from stride in Expression select stride).Optional()
            select new SliceItem(upper, lower, stride);

        internal static readonly Parser<IReadOnlyList<Expr>, char> SliceList =
            Combinators.SepEndBy1(ProperSlice.Try().Or(Expression), Control.Comma);

        private static readonly Parser<KeywordArgument, char> keywordItem =
            from id in Literals.Identifier
            from assign in Control.Assign
            from exp in Expression
            select new KeywordArgument(id, exp);

        private static readonly Parser<IReadOnlyList<KeywordArgument>, char> KeywordArguments =
            Combinators.SepBy1(keywordItem, Control.Comma);

        private static readonly Parser<IReadOnlyList<Expr>, char> PositionalArguments =
            Combinators.SepBy1(Expression, Control.Comma);

        private static Parser<IMaybe<T>, char> OptionalArgument<T>(Parser<T, char> p) =>
            (from _ in Control.Comma
            from parsed in p
            select parsed).Try().Optional();

        private static readonly Parser<IMaybe<Expr>, char> OptionalSequenceArg =
            OptionalArgument(from sa in Control.Asterisk from seq in Expression select seq);
        private static readonly Parser<IMaybe<Expr>, char> OptionalMappingArg =
            OptionalArgument(from da in Control.DoubleAsterisk from map in Expression select map);
        private static readonly Parser<IMaybe<IReadOnlyList<KeywordArgument>>, char> OptionalKeywords =
            OptionalArgument(KeywordArguments);


        private static readonly Parser<
            (
                IMaybe<IReadOnlyList<Expr>>,
                IMaybe<IReadOnlyList<KeywordArgument>>,
                IMaybe<Expr>,
                IMaybe<Expr>
            ), char> ArgumentList =
                Combinators.Choice<
            (
                IMaybe<IReadOnlyList<Expr>>,
                IMaybe<IReadOnlyList<KeywordArgument>>,
                IMaybe<Expr>,
                IMaybe<Expr>
            ), char>(
                    (from da in Control.DoubleAsterisk
                    from map in Expression
                    select (
                        Maybe.Nothing<IReadOnlyList<Expr>>(),
                        Maybe.Nothing<IReadOnlyList<KeywordArgument>>(),
                        Maybe.Nothing<Expr>(),
                        Maybe.FromValue(map)
                    )).Try(),
                    (from sa in Control.Asterisk
                    from sequence in Expression
                    from keywordArgs in OptionalKeywords
                    from map in OptionalMappingArg
                    select (
                        Maybe.Nothing<IReadOnlyList<Expr>>(),
                        keywordArgs,
                        Maybe.FromValue(sequence),
                        map
                    )).Try(),
                    (from keywordArgs in KeywordArguments
                    from sequence in OptionalSequenceArg
                    from restKeywordArgs in OptionalKeywords
                    from map in OptionalMappingArg
                    select (
                        Maybe.Nothing<IReadOnlyList<Expr>>(),
                        Maybe.FromValue(restKeywordArgs.Match(
                            just: (rest) => keywordArgs.Concat(rest).ToList(),
                            nothing: () => keywordArgs
                        )),
                        sequence,
                        map
                    )).Try(),
                    (from args in PositionalArguments
                    from keywordArgs in OptionalKeywords
                    from sequence in OptionalSequenceArg
                    from restKeywordArgs in OptionalKeywords
                    from map in OptionalMappingArg
                    select (
                        Maybe.FromValue(args),
                        keywordArgs.Match(
                            just: (kwargs) => Maybe.FromValue<IReadOnlyList<KeywordArgument>>(
                                kwargs.Concat(restKeywordArgs.Else(Array.Empty<KeywordArgument>())).ToList()
                            ),
                            nothing: () => restKeywordArgs
                        ),
                        sequence,
                        map
                    )).Try()
                );


        public static readonly Parser<Expr, char> Primary =
            from atom in Atom
            from rest in Combinators.Choice<Func<Expr, Expr>, char>
            (
                from dot in Control.Dot
                from id in Literals.Identifier
                select new Func<Expr, Expr>((Expr expr) => new AttributeRef(expr, id)),
                (from subscript in Combinators.Between(Control.OpenBracket, ExpressionList, Control.CloseBracket)
                select new Func<Expr, Expr>((Expr expr) => new Subscription(expr, subscript))
                ).Try(),
                from slice in Combinators.Between(Control.OpenBracket, SliceList, Control.CloseBracket)
                select new Func<Expr, Expr>((Expr expr) => new Slice(expr, slice)),
                from args in Combinators.Between(
                    Control.OpenParan,
                    ArgumentList.Option((
                        Maybe.Nothing<IReadOnlyList<Expr>>(),
                        Maybe.Nothing<IReadOnlyList<KeywordArgument>>(),
                        Maybe.Nothing<Expr>(),
                        Maybe.Nothing<Expr>())
                    ),
                    Control.CloseParan
                )
                select new Func<Expr, Expr>((callee) => new Call(callee, args.Item1, args.Item2, args.Item3, args.Item4))
            ).Many()
            select Foldl(rest, atom);

        private static readonly Parser<Binary, char> Power =
            from left in Primary
            from op in Control.DoubleAsterisk
            from right in Parsers.Indirect(() => UExpr)
            select new Binary(left, BinaryOperator.DoubleStar, right);

        private static readonly Parser<Expr, char> UExpr =
            Combinators.Choice<Expr, char>(
                Power,
                from op in Control.Minus
                from expr in Parsers.Indirect(() => UExpr)
                select new Unary(expr, UnaryOperator.Minus),
                from op in Control.Plus
                from expr in Parsers.Indirect(() => UExpr)
                select new Unary(expr, UnaryOperator.Plus)
            );

        private static Parser<Func<Expr, Expr>, char> RightBinary<Op>(Parser<Expr, char> right, Parser<Op, char> operatorParser, BinaryOperator op)
        {
            return from _ in operatorParser
                   from rightExpr in right
                   select new Func<Expr, Expr>(left => new Binary(left, op, rightExpr));
        }

        private static readonly Parser<Expr, char> MExpr =
            from unary in UExpr
            from rest in Combinators.Choice(
                RightBinary(UExpr, Control.Asterisk, BinaryOperator.Star),
                RightBinary(UExpr, Control.DoubleSlash, BinaryOperator.DoubleSlash),
                RightBinary(UExpr, Control.Slash, BinaryOperator.Slash),
                RightBinary(UExpr, Control.Modulo, BinaryOperator.Modulo)
            ).Many()
            select Foldl(rest, unary);

        private static readonly Parser<Expr, char> AExpr =
            from multiplative in MExpr
            from rest in Combinators.Choice(
                RightBinary(MExpr, Control.Plus, BinaryOperator.Plus),
                RightBinary(MExpr, Control.Minus, BinaryOperator.Minus)
            ).Many()
            select Foldl(rest, multiplative);

        // Leaves out shifting and bitwise operations

        private static readonly Parser<Expr, char> OrExpr = AExpr;

        private static readonly Parser<Expr, char> Comparison =
            from or in OrExpr
            from rest in Combinators.Choice(
                RightBinary(OrExpr, Control.IsNot, BinaryOperator.IsNot),
                RightBinary(OrExpr, Control.Is, BinaryOperator.Is),
                RightBinary(OrExpr, Control.In, BinaryOperator.In),
                RightBinary(OrExpr, Control.NotIn, BinaryOperator.NotIn),
                RightBinary(OrExpr, Control.Equal, BinaryOperator.Equal),
                RightBinary(OrExpr, Control.NotEqual, BinaryOperator.NotEqual),
                RightBinary(OrExpr, Control.LE, BinaryOperator.LE),
                RightBinary(OrExpr, Control.GE, BinaryOperator.GE),
                RightBinary(OrExpr, Control.LT, BinaryOperator.LT),
                RightBinary(OrExpr, Control.GT, BinaryOperator.GT)
            ).Many()
            select Foldl(rest, or);

        private static readonly Parser<Expr, char> NotTest =
            Comparison.Or(
                from not in Control.Not
                from comp in Parsers.Indirect(() => NotTest)
                select new Unary(comp, UnaryOperator.Not)
            );

        private static readonly Parser<Expr, char> AndTest =
            Combinators.ChainL1(
                NotTest,
                from op in Control.And
                select new Func<Expr, Expr, Expr>((left, right) => new Binary(left, BinaryOperator.And, right))
            );

        private static readonly Parser<Expr, char> OrTest =
            Combinators.ChainL1(
                AndTest,
                from op in Control.Or
                select new Func<Expr, Expr, Expr>((left, right) => new Binary(left, BinaryOperator.Or, right))
            );

        // Does not have conditional expression and lambdas
    }
}
