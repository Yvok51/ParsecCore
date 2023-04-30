using ParsecCore;
using ParsecCore.Help;
using System;
using System.Collections.Generic;

namespace ParsecCore.Expressions
{
    /// <summary>
    /// Class for quick creation of expression parser
    /// </summary>
    public static class Expression
    {
        /// <summary>
        /// Create a parser for arithmetic expressions.
        /// The allowed operators are to be listed in a 2D array with the highest priority operators listed first
        /// and lower priority operators listed lower.
        /// The operators listed on the same row have the same priority.
        /// On the same priority first prefix operators are applied, postfix after them and binary operators last.
        /// <para>
        /// If the given operators are ambigous and the parsed expression could not be decided,
        /// then an error is raised.
        /// </para>
        /// </summary>
        /// <typeparam name="T"> The type of the simple expression (term) parser </typeparam>
        /// <typeparam name="TInput"> The input type of the parsers of terms and operators </typeparam>
        /// <param name="operators"> The table of operators </param>
        /// <param name="termParser"> The parser of the basic expression </param>
        /// <returns> Arithmetic expression parser </returns>
        public static Parser<T, TInput> Build<T, TInput>(
            IReadOnlyList<IReadOnlyList<Operator<T, TInput>>> operators,
            Parser<T, TInput> termParser
        )
        {
            OperatorTable<T, TInput> table = OperatorTable<T, TInput>.Create(operators);
            return Build(table, termParser);
        }

        /// <summary>
        /// Create a parser for arithmetic expressions.
        /// The allowed operators are to be listed in a 2D table with the highest priority operators listed first
        /// and lower priority operators listed lower.
        /// The operators listed on the same row have the same priority.
        /// On the same priority first prefix operators are applied, postfix after them and binary operators last.
        /// <para>
        /// If the given operators are ambigous and the parsed expression could not be decided,
        /// then an error is raised.
        /// </para>
        /// </summary>
        /// <typeparam name="T"> The type of the simple expression (term) parser </typeparam>
        /// <typeparam name="TInput"> The input type of the parsers of terms and operators </typeparam>
        /// <param name="operators"> The table of operators </param>
        /// <param name="termParser"> The parser of the basic expression </param>
        /// <returns> Arithmetic expression parser </returns>
        public static Parser<T, TInput> Build<T, TInput>(
            OperatorTable<T, TInput> operators,
            Parser<T, TInput> termParser
        )
        {
            Parser<T, TInput> expressionParser = termParser;
            foreach (var priorityRow in operators.Table)
            {
                expressionParser = BuildRow(priorityRow, expressionParser);
            }

            return expressionParser;
        }

        /// <summary>
        /// Help function to define a prefix operator.
        /// </summary>
        /// <typeparam name="T"> The typed parsed by the simple term parser of the expression </typeparam>
        /// <param name="op"> The operator symbol </param>
        /// <param name="func"> The function which will be applied on the term </param>
        /// <returns> Prefix operator </returns>
        public static PrefixUnary<T, char> PrefixOperator<T>(string op, Func<T, T> func)
            => new PrefixUnary<T, char>(Parsers.Symbol(op).MapConstant(func).Try());

        /// <summary>
        /// Help function to define a prefix operator.
        /// </summary>
        /// <typeparam name="T"> The typed parsed by the simple term parser of the expression </typeparam>
        /// <typeparam name="TSpace"> The return type of the <paramref name="spaceConsumer"/> </typeparam>
        /// <param name="op"> The operator symbol </param>
        /// <param name="spaceConsumer"> The parser that will parse whitespace after the operator </param>
        /// <param name="func"> The function which will be applied on the term </param>
        /// <returns> Prefix operator </returns>
        public static PrefixUnary<T, char> PrefixOperator<T, TSpace, TInput>(
            string op,
            Parser<TSpace, char> spaceConsumer,
            Func<T, T> func
        )
            => new PrefixUnary<T, char>(Parsers.Symbol(op, spaceConsumer).MapConstant(func).Try());

        /// <summary>
        /// Help function to define a postfix operator.
        /// </summary>
        /// <typeparam name="T"> The typed parsed by the simple term parser of the expression </typeparam>
        /// <param name="op"> The operator symbol </param>
        /// <param name="func"> The function which will be applied on the term </param>
        /// <returns> Postfix operator </returns>
        public static PostfixUnary<T, char> PostfixOperator<T>(string op, Func<T, T> func)
            => new PostfixUnary<T, char>(Parsers.Symbol(op).MapConstant(func).Try());

        /// <summary>
        /// Help function to define a postfix operator.
        /// </summary>
        /// <typeparam name="T"> The typed parsed by the simple term parser of the expression </typeparam>
        /// <typeparam name="TSpace"> The return type of the <paramref name="spaceConsumer"/> </typeparam>
        /// <param name="op"> The operator symbol </param>
        /// <param name="spaceConsumer"> The parser that will parse whitespace after the operator </param>
        /// <param name="func"> The function which will be applied on the term </param>
        /// <returns> Postfix operator </returns>
        public static PostfixUnary<T, char> PostfixOperator<T, TSpace>(
            string op,
            Parser<TSpace, char> spaceConsumer,
            Func<T, T> func
        )
            => new PostfixUnary<T, char>(Parsers.Symbol(op, spaceConsumer).MapConstant(func).Try());

        /// <summary>
        /// Help function to define a binary operator.
        /// </summary>
        /// <typeparam name="T"> The typed parsed by the simple term parser of the expression </typeparam>
        /// <param name="op"> The operator symbol </param>
        /// <param name="func"> The function which will be applied on the terms </param>
        /// <param name="assoc"> The associativity of the operator </param>
        /// <returns> Binary operator </returns>
        public static InfixBinary<T, char> BinaryOperator<T>(string op, Func<T, T, T> func, Associativity assoc)
            => new InfixBinary<T, char>(Parsers.Symbol(op).MapConstant(func).Try(), assoc);

        /// <summary>
        /// Help function to define a binary operator.
        /// </summary>
        /// <typeparam name="T"> The typed parsed by the simple term parser of the expression </typeparam>
        /// <typeparam name="TSpace"> The return type of the <paramref name="spaceConsumer"/> </typeparam>
        /// <param name="op"> The operator symbol </param>
        /// <param name="spaceConsumer"> The parser that will parse whitespace after the operator </param>
        /// <param name="func"> The function which will be applied on the terms </param>
        /// <param name="assoc"> The associativity of the operator </param>
        /// <returns> Binary operator </returns>
        public static InfixBinary<T, char> BinaryOperator<T, TSpace>(
            string op,
            Parser<TSpace, char> spaceConsumer,
            Func<T, T, T> func,
            Associativity assoc
        )
            => new InfixBinary<T, char>(Parsers.Symbol(op, spaceConsumer).MapConstant(func).Try(), assoc);

        /// <summary>
        /// Build a single priority row of an expression parsers.
        /// The expression parser is tiered according to priority with the parser created from higher priority
        /// operators serving as our term (simple expression) parser.
        /// </summary>
        /// <typeparam name="T"> The type of the parsed simple expression </typeparam>
        /// <typeparam name="TInput"> The input type of the parsers </typeparam>
        /// <param name="row"> The priority row we are creating a parser for </param>
        /// <param name="termParser"> The simple expression term parser </param>
        /// <returns> Parser for this priority row </returns>
        private static Parser<T, TInput> BuildRow<T, TInput>(
            OperatorRow<T, TInput> row,
            Parser<T, TInput> termParser
        )
        {
            var returnId = Parsers.Return<Func<T, T>, TInput>(x => x);

            var rAssocOp = Parsers.Choice(row.RightAssoc.Map(op => op.Parser));
            var lAssocOp = Parsers.Choice(row.LeftAssoc.Map(op => op.Parser));
            var noAssocOp = Parsers.Choice(row.NoAssoc.Map(op => op.Parser));
            // add returnId in case no prefix/postfix operators are present in this row
            var prefixOp = Parsers.Choice(row.Prefix.Map(op => op.Parser).Append(returnId));
            var postfixOp = Parsers.Choice(row.Postfix.Map(op => op.Parser).Append(returnId));

            // Create a new term parser from the simple term parser and the prefix and postfix operators
            var termP = from pre in prefixOp
                        from term in termParser
                        from post in postfixOp
                        select post(pre(term));

            var ambiguousLeft = Ambiguous(Associativity.Left, lAssocOp);
            var ambiguousNone = Ambiguous(Associativity.None, noAssocOp);
            var ambiguousRight = Ambiguous(Associativity.Right, rAssocOp);

            return from x in termP
                   from res in Parsers.Choice(
                       CreateRightAssocParser(x, termP, rAssocOp, ambiguousLeft, ambiguousNone),
                       CreateLeftAssocParser(x, termP, lAssocOp, ambiguousRight, ambiguousNone),
                       CreateNoAssocParser(x, termP, noAssocOp, ambiguousLeft, ambiguousRight, ambiguousNone),
                       Parsers.Return<T, TInput>(x)
                    )
                   select res;
        }

        private static Parser<T, TInput> Ambiguous<T, TInput>(
            Associativity assoc,
            Parser<Func<T, T, T>, TInput> opParser
        )
        {
            string associativity = assoc switch
            {
                Associativity.None => "not",
                Associativity.Left => "right",
                Associativity.Right => "left",
                _ => throw new NotImplementedException(),
            };
            return opParser.Then(Parsers.Fail<T, TInput>($"ambiguous use of a {associativity} associative operator")).Try();
        }

        /// <summary>
        /// Create a parser for right-associative operators.
        /// It will first try to parse a right-associative operator
        /// and afterwards a list of terms separated by operators.
        /// </summary>
        /// <typeparam name="T"> The typed parsed by the term parser </typeparam>
        /// <typeparam name="TInput"> The input type of the parsers </typeparam>
        /// <param name="value"> The value we have parsed so far </param>
        /// <param name="termParser"> The parser for simple term </param>
        /// <param name="rAssocParser">
        /// The parser of all different right-associative operators on this priority row
        /// </param>
        /// <param name="ambiguousLeft">
        /// Parser that checks there isn't an ambiguous use of a left-associative operator after us
        /// </param>
        /// <param name="ambiguousNone">
        /// Parser that checks there isn't an ambiguous use of a non-associative operator after us
        /// </param>
        /// <returns> Parser for a right-associative operators on this priority row </returns>
        private static Parser<T, TInput> CreateRightAssocParser<T, TInput>(
            T value,
            Parser<T, TInput> termParser,
            Parser<Func<T, T, T>, TInput> rAssocParser,
            Parser<T, TInput> ambiguousLeft,
            Parser<T, TInput> ambiguousNone
        )
        {
            Parser<(Func<T, T, T> op, T right), TInput> opParser =
                from f in rAssocParser
                from right in termParser
                select (f, right);
            Parser<Maybe<(Func<T, T, T> op, T right)>, TInput> opParserOptional =
                opParser.Optional();

            Parser<T, TInput> parser = (input) =>
            {
                var firstResult = opParser(input);
                if (firstResult.IsError)
                {
                    return Result.RetypeError<(Func<T, T, T>, T), T, TInput>(firstResult);
                }
                List<(Func<T, T, T> op, T value)> parsedResults = new() { firstResult.Result };
                List<IResult<Maybe<(Func<T, T, T>, T)>, TInput>> results = new();

                var rightSideResult = opParserOptional(input);
                results.Add(rightSideResult);
                while (rightSideResult.IsResult && rightSideResult.Result.HasValue)
                {
                    parsedResults.Add(rightSideResult.Result.Value);
                    rightSideResult = opParserOptional(rightSideResult.UnconsumedInput);
                    results.Add(rightSideResult);
                }

                if (rightSideResult.IsError)
                {
                    return Result.Failure<T, Maybe<(Func<T, T, T>, T)>, (Func<T, T, T>, T), TInput>(
                        results,
                        firstResult,
                        rightSideResult.UnconsumedInput
                    );
                }

                if (parsedResults.Count == 0)
                {
                    return Result.Success(value, firstResult, rightSideResult);
                }
                else if (parsedResults.Count == 1)
                {
                    return Result.Success(
                        parsedResults[0].op(value, parsedResults[0].value),
                        results,
                        firstResult,
                        rightSideResult.UnconsumedInput
                    );
                }

                T accum = parsedResults[parsedResults.Count - 1].op(
                    parsedResults[parsedResults.Count - 2].value,
                    parsedResults[parsedResults.Count - 1].value
                );
                for (int i = parsedResults.Count - 2; i >= 1; i--)
                {
                    accum = parsedResults[i].op(parsedResults[i - 1].value, accum);
                }

                return Result.Success(
                    parsedResults[0].op(value, accum),
                    results,
                    firstResult,
                    rightSideResult.UnconsumedInput
                );
            };

            return parser.Or(ambiguousLeft).Or(ambiguousNone);
        }

        /// <summary>
        /// Create a parser for left-associative operators.
        /// It will first try to parse a left-associative operator
        /// and afterwards a list of terms separated by operators.
        /// </summary>
        /// <typeparam name="T"> The typed parsed by the term parser </typeparam>
        /// <typeparam name="TInput"> The input type of the parsers </typeparam>
        /// <param name="value"> The value we have parsed so far </param>
        /// <param name="termParser"> The parser for simple term </param>
        /// <param name="lAssocParser">
        /// The parser of all different left-associative operators on this priority row
        /// </param>
        /// <param name="ambiguousRight">
        /// Parser that checks there isn't an ambiguous use of a right-associative operator after us
        /// </param>
        /// <param name="ambiguousNone">
        /// Parser that checks there isn't an ambiguous use of a non-associative operator after us
        /// </param>
        /// <returns> Parser for a left-associative operators on this priority row </returns>
        private static Parser<T, TInput> CreateLeftAssocParser<T, TInput>(
            T value,
            Parser<T, TInput> termParser,
            Parser<Func<T, T, T>, TInput> lAssocParser,
            Parser<T, TInput> ambiguousRight,
            Parser<T, TInput> ambiguousNone
        )
        {
            Parser<(Func<T, T, T> op, T right), TInput> opParser =
                from f in lAssocParser
                from right in termParser
                select (f, right);
            Parser<Maybe<(Func<T, T, T> op, T right)>, TInput> opParserOptional =
                opParser.Optional();

            Parser<T, TInput> parser = (input) =>
            {
                var firstResult = opParser(input);
                if (firstResult.IsError)
                {
                    return Result.RetypeError<(Func<T, T, T> op, T right), T, TInput>(firstResult);
                }
                List<IResult<Maybe<(Func<T, T, T>, T)>, TInput>> results = new();
                T accum = firstResult.Result.op(value, firstResult.Result.right);

                var rightSideResult = opParserOptional(firstResult.UnconsumedInput);
                results.Add(rightSideResult);
                while (rightSideResult.IsResult && rightSideResult.Result.HasValue)
                {
                    accum = rightSideResult.Result.Value.op(accum, rightSideResult.Result.Value.right);
                    rightSideResult = opParserOptional(rightSideResult.UnconsumedInput);
                    results.Add(rightSideResult);
                }

                if (rightSideResult.IsError)
                {
                    return Result.Failure<T, Maybe<(Func<T, T, T>, T)>, (Func<T, T, T>, T) , TInput>(
                        results,
                        firstResult,
                        rightSideResult.UnconsumedInput
                    );
                }

                return Result.Success(accum, results, firstResult, rightSideResult.UnconsumedInput);
            };

            return parser.Or(ambiguousRight).Or(ambiguousNone);
        }

        /// <summary>
        /// Create a parser for non-associative operators.
        /// It will first try to parse a non-associative operator and a term afterward.
        /// Since this operator is non-associative only two terms can be parsed at once
        /// </summary>
        /// <typeparam name="T"> The typed parsed by the term parser </typeparam>
        /// <typeparam name="TInput"> The input type of the parsers </typeparam>
        /// <param name="value"> The value we have parsed so far </param>
        /// <param name="termParser"> The parser for simple term </param>
        /// <param name="noAssocParser">
        /// The parser of all different non-associative operators on this priority row
        /// </param>
        /// <param name="ambiguousLeft">
        /// Parser that checks there isn't an ambiguous use of a left-associative operator after us
        /// </param>
        /// <param name="ambiguousNone">
        /// Parser that checks there isn't an ambiguous use of a non-associative operator after us
        /// </param>
        /// <param name="ambiguousRight">
        /// Parser that checks there isn't an ambiguous use of a right-associative operator after us
        /// </param>
        /// <returns> Parser for a non-associative operators on this priority row </returns>
        private static Parser<T, TInput> CreateNoAssocParser<T, TInput>(
            T value,
            Parser<T, TInput> termParser,
            Parser<Func<T, T, T>, TInput> noAssocParser,
            Parser<T, TInput> ambiguousLeft,
            Parser<T, TInput> ambiguousNone,
            Parser<T, TInput> ambiguousRight
        )
        {
            var ambiguous = Parsers.Choice(
                ambiguousRight, ambiguousLeft, ambiguousNone
            );
            var parser = from f in noAssocParser
                         from y in termParser
                         from res in ambiguous.Or(Parsers.Return<T, TInput>(f(value, y)))
                         select res;

            return parser;
        }
    }
}
