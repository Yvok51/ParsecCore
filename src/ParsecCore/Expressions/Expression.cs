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

        public static PrefixUnary<T, char> PrefixOperator<T>(string op, Func<T, T> func)
            => new PrefixUnary<T, char>(Parsers.Symbol(op).MapConstant(func));

        public static PrefixUnary<T, char> PrefixOperator<T, TSpace>(
            string op,
            Parser<TSpace, char> spaceConsumer,
            Func<T, T> func
        )
            => new PrefixUnary<T, char>(Parsers.Symbol(op, spaceConsumer).MapConstant(func));

        public static PostfixUnary<T, char> PostfixOperator<T>(string op, Func<T, T> func)
            => new PostfixUnary<T, char>(Parsers.Symbol(op).MapConstant(func));

        public static PostfixUnary<T, char> PostfixOperator<T, TSpace>(
            string op,
            Parser<TSpace, char> spaceConsumer,
            Func<T, T> func
        )
            => new PostfixUnary<T, char>(Parsers.Symbol(op, spaceConsumer).MapConstant(func));

        public static InfixBinary<T, char> BinaryOperator<T>(string op, Func<T, T, T> func, Associativity assoc)
            => new InfixBinary<T, char>(Parsers.Symbol(op).MapConstant(func), assoc);

        public static InfixBinary<T, char> BinaryOperator<T, TSpace>(
            string op,
            Parser<TSpace, char> spaceConsumer,
            Func<T, T, T> func,
            Associativity assoc
        )
            => new InfixBinary<T, char>(Parsers.Symbol(op, spaceConsumer).MapConstant(func), assoc);

        private static Parser<T, TInput> BuildRow<T, TInput>(
            OperatorRow<T, TInput> row,
            Parser<T, TInput> termParser
        )
        {
            var returnId = Parsers.Return<Func<T, T>, TInput>(x => x);

            var rAssocOp = Parsers.Choice(row.RightAssoc.Map(op => op.Parser));
            var lAssocOp = Parsers.Choice(row.LeftAssoc.Map(op => op.Parser));
            var noAssocOp = Parsers.Choice(row.NoAssoc.Map(op => op.Parser));
            var prefixOp = Parsers.Choice(row.Prefix.Map(op => op.Parser).Append(returnId));
            var postfixOp = Parsers.Choice(row.Postfix.Map(op => op.Parser).Append(returnId));

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
            return opParser.Then(Parsers.Fail<T, TInput>($"ambiguous use of a {associativity} associative operator"));
        }

        private static Parser<T, TInput> CreateRightAssocParser<T, TInput>(
            T value,
            Parser<T, TInput> termParser,
            Parser<Func<T, T, T>, TInput> rAssocParser,
            Parser<T, TInput> ambiguousLeft,
            Parser<T, TInput> ambiguousNone
        )
        {
            var operandParser = from z in termParser
                                from rest in CreateRightAssocParser1(
                                    z, termParser, rAssocParser, ambiguousLeft, ambiguousNone
                                )
                                select rest;

            var parser = from f in rAssocParser
                         from y in operandParser
                         select f(value, y);

            return parser.Or(ambiguousLeft).Or(ambiguousNone);
        }

        private static Parser<T, TInput> CreateRightAssocParser1<T, TInput>(
            T value,
            Parser<T, TInput> termParser,
            Parser<Func<T, T, T>, TInput> rAssocParser,
            Parser<T, TInput> ambiguousLeft,
            Parser<T, TInput> ambiguousNone
        )
        {
            return CreateRightAssocParser(value, termParser, rAssocParser, ambiguousLeft, ambiguousNone)
                .Or(Parsers.Return<T, TInput>(value));
        }

        private static Parser<T, TInput> CreateLeftAssocParser<T, TInput>(
            T value,
            Parser<T, TInput> termParser,
            Parser<Func<T, T, T>, TInput> lAssocParser,
            Parser<T, TInput> ambiguousRight,
            Parser<T, TInput> ambiguousNone
        )
        {

            var parser = from f in lAssocParser
                         from y in termParser
                         from res in CreateLeftAssocParser1(
                             f(value, y), termParser, lAssocParser, ambiguousRight, ambiguousNone
                         )
                         select res;

            return parser.Or(ambiguousRight).Or(ambiguousNone);
        }

        private static Parser<T, TInput> CreateLeftAssocParser1<T, TInput>(
            T value,
            Parser<T, TInput> termParser,
            Parser<Func<T, T, T>, TInput> lAssocParser,
            Parser<T, TInput> ambiguousRight,
            Parser<T, TInput> ambiguousNone
        )
        {
            return CreateLeftAssocParser(value, termParser, lAssocParser, ambiguousRight, ambiguousNone)
                .Or(Parsers.Return<T, TInput>(value));
        }

        private static Parser<T, TInput> CreateNoAssocParser<T, TInput>(
            T value,
            Parser<T, TInput> termParser,
            Parser<Func<T, T, T>, TInput> noAssocParser,
            Parser<T, TInput> ambiguousLeft,
            Parser<T, TInput> ambiguousNone,
            Parser<T, TInput> ambiguousRight
        )
        {

            var parser = from f in noAssocParser
                         from y in termParser
                         from res in Parsers.Choice(
                             ambiguousRight, ambiguousLeft, ambiguousNone, Parsers.Return<T, TInput>(f(value, y))
                         )
                         select res;

            return parser.Or(ambiguousRight).Or(ambiguousNone);
        }
    }
}
