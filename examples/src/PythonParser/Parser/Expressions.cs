using ParsecCore;
using PythonParser.Structures;

namespace PythonParser.Parser
{
    internal static class Expressions
    {
        public static Parser<Expr, char> Expression(Control.LexemeFactory lexeme)
            => lexeme.Create(Parsers.Indirect(() => OrTest(lexeme)));

        /// <summary>
        /// Parses a list of expressions seperated by a comma
        /// </summary>
        /// <param name="lexeme"> How to consume whitespace </param>
        /// <returns></returns>
        public static Parser<IReadOnlyList<Expr>, char> ExpressionList(Control.LexemeFactory lexeme)
            => Parsers.SepEndBy1(Expression(lexeme), Control.Comma(lexeme));

        /// <summary>
        /// Parses a list of expressions in parentheses 
        /// </summary>
        /// <param name="lexeme"> How to consume whitespace </param>
        /// <returns></returns>
        private static Parser<ParenthForm, char> ParenthForm(Control.LexemeFactory lexeme)
            => Parsers.Between(
                Control.OpenParan(Control.EOLLexeme),
                ExpressionList(Control.EOLLexeme),
                Control.CloseParan(lexeme)
            ).Map(list => new ParenthForm(list));

        // Does not include comprehensions

        /// <summary>
        /// Parses a list of expressions in branckets. Used for list literals
        /// </summary>
        /// <param name="lexeme"> How to consume whitespace </param>
        /// <returns></returns>
        private static Parser<ListDisplay, char> ListDisplay(Control.LexemeFactory lexeme)
            => Parsers.Between(
                Control.OpenBracket(Control.EOLLexeme),
                ExpressionList(Control.EOLLexeme).Option(Array.Empty<Expr>()),
                Control.CloseBracket(lexeme)
            ).Map(list => new ListDisplay(list));

        /// <summary>
        /// Parses a list of expressions in braces. Used for set literals
        /// </summary>
        /// <param name="lexeme"> How to consume whitespace </param>
        /// <returns></returns>
        private static Parser<SetDisplay, char> SetDisplay(Control.LexemeFactory lexeme)
            => Parsers.Between(
                Control.OpenBrace(Control.EOLLexeme),
                ExpressionList(Control.EOLLexeme),
                Control.CloseBrace(lexeme)
            ).Map(list => new SetDisplay(list));

        private static Parser<KeyDatum, char> KeyDatum(Control.LexemeFactory lexeme)
            => from key in Expression(lexeme)
               from sep in Control.Colon(lexeme)
               from val in Expression(lexeme)
               select new KeyDatum(key, val);

        private static Parser<IReadOnlyList<KeyDatum>, char> KeyDatumList(Control.LexemeFactory lexeme)
            => Parsers.SepEndBy1(KeyDatum(lexeme), Control.Comma(lexeme));

        /// <summary>
        /// Parses a list of key value pairs in braces. Used for dictionary literals
        /// </summary>
        /// <param name="lexeme"> How to consume whitespace </param>
        /// <returns></returns>
        private static Parser<DictDisplay, char> DictDisplay(Control.LexemeFactory lexeme)
            => Parsers.Between(
                Control.OpenBrace(Control.EOLLexeme),
                KeyDatumList(Control.EOLLexeme).Option(Array.Empty<KeyDatum>()),
                Control.CloseBrace(lexeme)
            ).Map(data => new DictDisplay(data));

        // Does not include yield and generator expressions

        private static Parser<Expr, char> Enclosure(Control.LexemeFactory lexeme)
            => Parsers.Choice<Expr, char>(
                ParenthForm(lexeme),
                ListDisplay(lexeme),
                DictDisplay(lexeme).Try(),
                SetDisplay(lexeme)
            );

        public static Parser<Expr, char> Atom(Control.LexemeFactory lexeme)
            => Parsers.Choice(
                Control.Keyword("True", lexeme).MapConstant(new BooleanLiteral(true)).Try(),
                Control.Keyword("False", lexeme).MapConstant(new BooleanLiteral(false)).Try(),
                Control.Keyword("None", lexeme).MapConstant(new NoneLiteral()).Try(),
                Literals.Identifier(lexeme),
                Literals.Literal(lexeme),
                Enclosure(lexeme)
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

        /// <summary>
        /// Slices used in array access. For example <c>arr[0:10]</c>
        /// </summary>
        private static readonly Parser<SliceItem, char> ProperSlice =
            from lower in Expression(Control.EOLLexeme).Optional()
            from colon in Control.Colon(Control.EOLLexeme)
            from upper in Expression(Control.EOLLexeme).Optional()
            from stride in Control.Colon(Control.EOLLexeme).Then(Expression(Control.EOLLexeme)).Optional()
            select new SliceItem(lower, upper, stride);

        /// <summary>
        /// List of array slices seperated by commas
        /// </summary>
        internal static readonly Parser<IReadOnlyList<Expr>, char> SliceList =
            Parsers.SepEndBy1(ProperSlice.Try().Or(Expression(Control.EOLLexeme)), Control.Comma(Control.EOLLexeme));

        private static readonly Parser<KeywordArgument, char> keywordItem =
            from id in Literals.Identifier(Control.EOLLexeme)
            from assign in Control.Assign(Control.EOLLexeme)
            from exp in Expression(Control.EOLLexeme)
            select new KeywordArgument(id, exp);

        /// <summary>
        /// Keyword arguments used in the parameters for function definition
        /// </summary>
        private static readonly Parser<IReadOnlyList<KeywordArgument>, char> KeywordArguments =
            Parsers.SepBy1(keywordItem, Control.Comma(Control.EOLLexeme));

        /// <summary>
        /// Positional arguments used as the parameters for funtion definition
        /// </summary>
        private static readonly Parser<IReadOnlyList<Expr>, char> PositionalArguments =
            Parsers.SepBy1(Expression(Control.EOLLexeme), Control.Comma(Control.EOLLexeme));

        private static Parser<Maybe<T>, char> OptionalArgument<T>(Parser<T, char> p) =>
            Control.Comma(Control.EOLLexeme).Then(p).Try().Optional();

        private static readonly Parser<Maybe<Expr>, char> OptionalSequenceArg =
            OptionalArgument(Control.Asterisk(Control.EOLLexeme).Then(Expression(Control.EOLLexeme)));

        private static readonly Parser<Maybe<Expr>, char> OptionalMappingArg =
            OptionalArgument(Control.DoubleAsterisk(Control.EOLLexeme).Then(Expression(Control.EOLLexeme)));

        private static readonly Parser<Maybe<IReadOnlyList<KeywordArgument>>, char> OptionalKeywords =
            OptionalArgument(KeywordArguments);

        /// <summary>
        /// Parser for function parameters. So far parses only positional arguments.
        /// </summary>
        private static readonly Parser<
            (
                Maybe<IReadOnlyList<Expr>>,
                Maybe<IReadOnlyList<KeywordArgument>>,
                Maybe<Expr>,
                Maybe<Expr>
            ), char> ArgumentList =
                Parsers.Choice<
            (
                Maybe<IReadOnlyList<Expr>>,
                Maybe<IReadOnlyList<KeywordArgument>>,
                Maybe<Expr>,
                Maybe<Expr>
            ), char>(
                    from args in PositionalArguments
                    select (
                        Maybe.FromValue(args),
                        Maybe.Nothing<IReadOnlyList<KeywordArgument>>(),
                        Maybe.Nothing<Expr>(),
                        Maybe.Nothing<Expr>()
                    )
                );


        public static Parser<Expr, char> Primary(Control.LexemeFactory lexeme)
            => from atom in Atom(lexeme)
               from rest in Parsers.Choice<Func<Expr, Expr>, char>
               (
                   // attribute access
                   from dot in Control.Dot(lexeme) 
                   from id in Literals.Identifier(lexeme)
                   select new Func<Expr, Expr>((Expr expr) => new AttributeRef(expr, id)),
                   // subscript
                   (from subscript in Parsers.Between(
                       Control.OpenBracket(Control.EOLLexeme),
                       ExpressionList(Control.EOLLexeme),
                       Control.CloseBracket(lexeme)
                   )
                    select new Func<Expr, Expr>((Expr expr) => new Subscription(expr, subscript))
                   ).Try(),
                   // array slice
                   from slice in Parsers.Between(
                       Control.OpenBracket(Control.EOLLexeme),
                       SliceList,
                       Control.CloseBracket(lexeme)
                   )
                   select new Func<Expr, Expr>((Expr expr) => new Slice(expr, slice)),
                   // function call
                   from args in Parsers.Between(
                       Control.OpenParan(Control.EOLLexeme),
                       ArgumentList.Option((
                           Maybe.Nothing<IReadOnlyList<Expr>>(),
                           Maybe.Nothing<IReadOnlyList<KeywordArgument>>(),
                           Maybe.Nothing<Expr>(),
                           Maybe.Nothing<Expr>())
                       ),
                       Control.CloseParan(lexeme)
                   )
                   select new Func<Expr, Expr>((callee) => new Call(callee, args.Item1, args.Item2, args.Item3, args.Item4))
               ).Many()
               select Foldl(rest, atom);

        /// <summary>
        /// Parses exponentiation
        /// </summary>
        /// <param name="lexeme"> How to consume whitespace </param>
        /// <returns></returns>
        private static Parser<Expr, char> Power(Control.LexemeFactory lexeme)
            => from left in Primary(lexeme)
               from right in (from op in Control.DoubleAsterisk(lexeme)
                              from right in Parsers.Indirect(() => UExpr(lexeme))
                              select right).Optional()
               select right.Match(
                   just: (val) => new Binary(left, BinaryOperator.DoubleStar, val),
                   nothing: () => left
               );

        /// <summary>
        /// Parses unary operators '-' and '+'
        /// </summary>
        /// <param name="lexeme"> How to consume whitespace </param>
        /// <returns></returns>
        private static Parser<Expr, char> UExpr(Control.LexemeFactory lexeme)
            => Parsers.Choice<Expr, char>(
                Power(lexeme),
                from expr in Control.Minus(lexeme).Then(Parsers.Indirect(() => UExpr(lexeme)))
                select new Unary(expr, UnaryOperator.Minus),
                from expr in Control.Plus(lexeme).Then(Parsers.Indirect(() => UExpr(lexeme)))
                select new Unary(expr, UnaryOperator.Plus)
            );

        private static Parser<Func<Expr, Expr>, char> RightBinary<Op>(Parser<Expr, char> right, Parser<Op, char> operatorParser, BinaryOperator op)
        {
            return from rightExpr in operatorParser.Then(right)
                   select new Func<Expr, Expr>(left => new Binary(left, op, rightExpr));
        }

        /// <summary>
        /// Parses binary operators '*', '//', '/', '%'
        /// </summary>
        /// <param name="lexeme"> How to consume whitespace </param>
        /// <returns></returns>
        private static Parser<Expr, char> MExpr(Control.LexemeFactory lexeme)
            => from unary in UExpr(lexeme)
               from rest in Parsers.Choice(
                   RightBinary(UExpr(lexeme), Control.Asterisk(lexeme), BinaryOperator.Star),
                   RightBinary(UExpr(lexeme), Control.DoubleSlash(lexeme), BinaryOperator.DoubleSlash),
                   RightBinary(UExpr(lexeme), Control.Slash(lexeme), BinaryOperator.Slash),
                   RightBinary(UExpr(lexeme), Control.Modulo(lexeme), BinaryOperator.Modulo)
               ).Many()
               select Foldl(rest, unary);

        /// <summary>
        /// Parses binary operators '+', '-'
        /// </summary>
        /// <param name="lexeme"> How to consume whitespace </param>
        /// <returns></returns>
        private static Parser<Expr, char> AExpr(Control.LexemeFactory lexeme)
            => from multiplative in MExpr(lexeme)
               from rest in Parsers.Choice(
                   RightBinary(MExpr(lexeme), Control.Plus(lexeme), BinaryOperator.Plus),
                   RightBinary(MExpr(lexeme), Control.Minus(lexeme), BinaryOperator.Minus)
               ).Many()
               select Foldl(rest, multiplative);

        // Leaves out shifting and bitwise operations

        private static Parser<Expr, char> OrExpr(Control.LexemeFactory lexeme) => AExpr(lexeme);

        /// <summary>
        /// Parses comparison operators
        /// </summary>
        /// <param name="lexeme"> How to consume whitespace </param>
        /// <returns></returns>
        private static Parser<Expr, char> Comparison(Control.LexemeFactory lexeme)
            => from or in OrExpr(lexeme)
               from rest in Parsers.Choice(
                   RightBinary(OrExpr(lexeme), Control.IsNot(lexeme), BinaryOperator.IsNot),
                   RightBinary(OrExpr(lexeme), Control.Is(lexeme), BinaryOperator.Is),
                   RightBinary(OrExpr(lexeme), Control.In(lexeme), BinaryOperator.In),
                   RightBinary(OrExpr(lexeme), Control.NotIn(lexeme), BinaryOperator.NotIn),
                   RightBinary(OrExpr(lexeme), Control.Equal(lexeme), BinaryOperator.Equal),
                   RightBinary(OrExpr(lexeme), Control.NotEqual(lexeme), BinaryOperator.NotEqual),
                   RightBinary(OrExpr(lexeme), Control.LE(lexeme), BinaryOperator.LE),
                   RightBinary(OrExpr(lexeme), Control.GE(lexeme), BinaryOperator.GE),
                   RightBinary(OrExpr(lexeme), Control.LT(lexeme), BinaryOperator.LT),
                   RightBinary(OrExpr(lexeme), Control.GT(lexeme), BinaryOperator.GT)
               ).Many()
               select Foldl(rest, or);

        /// <summary>
        /// Parses 'not' boolean operator
        /// </summary>
        /// <param name="lexeme">How to consume whitespace </param>
        /// <returns></returns>
        private static Parser<Expr, char> NotTest(Control.LexemeFactory lexeme)
            => Comparison(lexeme).Or(
                Control.Not(lexeme)
                    .Then(Parsers.Indirect(() => NotTest(lexeme)))
                    .Map(comparison => new Unary(comparison, UnaryOperator.Not))
            );

        /// <summary>
        /// Parses 'and' binary operator
        /// </summary>
        /// <param name="lexeme"> How to consume whitespace </param>
        /// <returns></returns>
        private static Parser<Expr, char> AndTest(Control.LexemeFactory lexeme)
            => Parsers.ChainL1(
                NotTest(lexeme),
                Control.And(lexeme).MapConstant(
                    new Func<Expr, Expr, Expr>((left, right) => new Binary(left, BinaryOperator.And, right))
                )
            );

        /// <summary>
        /// Parses 'or' binary operator
        /// </summary>
        /// <param name="lexeme"> How to consume whitespace </param>
        /// <returns></returns>
        private static Parser<Expr, char> OrTest(Control.LexemeFactory lexeme)
            => Parsers.ChainL1(
                AndTest(lexeme),
                Control.Or(lexeme).MapConstant(
                    new Func<Expr, Expr, Expr>((left, right) => new Binary(left, BinaryOperator.Or, right))
                )
            );

        // Does not have conditional expression and lambdas
    }
}
