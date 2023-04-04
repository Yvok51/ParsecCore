using System;
using System.Collections.Generic;
using ParsecCore;
using ParsecCore.Help;

namespace ParsecCore.Expression
{
    public static class Expression
    {
        public static Parser<T, TInput> Build<T, TInput>(
            IReadOnlyList<IReadOnlyList<Operator<T, TInput>>> operators,
            Parser<T, TInput> termParser
        )
        {
            Parser<T, TInput> expressionParser = termParser;
            OperatorTable<T, TInput> table = OperatorTable<T, TInput>.Create(operators);

            foreach (var priorityRow in table.Table)
            {
                expressionParser = BuildRow(priorityRow, expressionParser);
            }

            return expressionParser;
        }

        private static Parser<T, TInput> BuildRow<T, TInput>(
            OperatorRow<T, TInput> row,
            Parser<T, TInput> termParser
        )
        {
            var returnId = Parsers.Return<Func<T, T>, TInput>(x => x);

            var rAssocOp = Parsers.Choice(row.RightAssoc.Map(op => op.Parser));
            var lAssocOp = Parsers.Choice(row.LeftAssoc.Map(op => op.Parser));
            var noAssocOp = Parsers.Choice(row.NoAssoc.Map(op => op.Parser));
            var prefixOp = Parsers.Choice(row.Prefix.Map(op => op.Parser));
            var postfixOp = Parsers.Choice(row.Postfix.Map(op => op.Parser));

            var termP = from pre in prefixOp
                        from term in termParser
                        from post in postfixOp
                        select post(pre(term));

            var postfixP = postfixOp.Or(returnId);
            var prefixP = prefixOp.Or(returnId);

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
