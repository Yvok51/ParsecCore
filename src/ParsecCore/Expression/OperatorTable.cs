using ParsecCore.Help;
using System.Collections.Generic;

namespace ParsecCore.Expression
{
    internal class OperatorTable<T, TInput>
    {
        public static OperatorTable<T, TInput> Create(IReadOnlyList<IReadOnlyList<Operator<T, TInput>>> operators)
        {
            return new OperatorTable<T, TInput>(operators.Map(row => OperatorRow<T, TInput>.Create(row)));
        }

        private OperatorTable(IReadOnlyList<OperatorRow<T, TInput>> table)
        {
            Table = table;
        }

        public IReadOnlyList<OperatorRow<T, TInput>> Table { get; init; }
    }

    internal class OperatorRow<T, TInput>
    {
        public static OperatorRow<T, TInput> Create(IReadOnlyList<Operator<T, TInput>> row)
        {
            List<PrefixUnary<T, TInput>> prefix = new();
            List<PostfixUnary<T, TInput>> postfix = new();
            List<InfixBinary<T, TInput>> lAssoc = new();
            List<InfixBinary<T, TInput>> noAssoc = new();
            List<InfixBinary<T, TInput>> rAssoc = new();
            OperatorSorter sorter = new();
            foreach (var @operator in row)
            {
                @operator.Accept(sorter, (prefix, postfix, lAssoc, noAssoc, rAssoc));
            }

            return new OperatorRow<T, TInput>(prefix, postfix, rAssoc, lAssoc, noAssoc);
        }

        private class OperatorSorter : IOperatorVisitor<T, TInput, None, 
            (List<PrefixUnary<T, TInput>>,
             List<PostfixUnary<T, TInput>>,
             List<InfixBinary<T, TInput>>,
             List<InfixBinary<T, TInput>>,
             List<InfixBinary<T, TInput>>)
        >
        {
            public None VisitBinary(
                InfixBinary<T, TInput> infix,
                (List<PrefixUnary<T, TInput>>,
                List<PostfixUnary<T, TInput>>,
                List<InfixBinary<T, TInput>>,
                List<InfixBinary<T, TInput>>,
                List<InfixBinary<T, TInput>>) operators
            )
            {
                switch (infix.Associativity)
                {
                    case Associativity.Left: operators.Item3.Add(infix); break;
                    case Associativity.None: operators.Item4.Add(infix); break;
                    case Associativity.Right: operators.Item5.Add(infix); break;
                };
                return None.Instance;
            }

            public None VisitPostfix(
                PostfixUnary<T, TInput> postfix,
                (List<PrefixUnary<T, TInput>>,
                List<PostfixUnary<T, TInput>>,
                List<InfixBinary<T, TInput>>,
                List<InfixBinary<T, TInput>>,
                List<InfixBinary<T, TInput>>) operators
            )
            {
                operators.Item2.Add(postfix);
                return None.Instance;
            }

            public None VisitPrefix(
                PrefixUnary<T, TInput> prefix,
                (List<PrefixUnary<T, TInput>>,
                List<PostfixUnary<T, TInput>>,
                List<InfixBinary<T, TInput>>,
                List<InfixBinary<T, TInput>>,
                List<InfixBinary<T, TInput>>) operators
            )
            {
                operators.Item1.Add(prefix);
                return None.Instance;
            }
        }

        private OperatorRow(
            IReadOnlyList<PrefixUnary<T, TInput>> prefix,
            IReadOnlyList<PostfixUnary<T, TInput>> postfix,
            IReadOnlyList<InfixBinary<T, TInput>> rightAssoc,
            IReadOnlyList<InfixBinary<T, TInput>> leftAssoc,
            IReadOnlyList<InfixBinary<T, TInput>> noAssoc
        )
        {
            Prefix = prefix;
            Postfix = postfix;
            RightAssoc = rightAssoc;
            LeftAssoc = leftAssoc;
            NoAssoc = noAssoc;
        }

        public IReadOnlyList<PrefixUnary<T, TInput>> Prefix { get; init; }
        public IReadOnlyList<PostfixUnary<T, TInput>> Postfix { get; init; }
        public IReadOnlyList<InfixBinary<T, TInput>> RightAssoc { get; init; }
        public IReadOnlyList<InfixBinary<T, TInput>> LeftAssoc { get; init; }
        public IReadOnlyList<InfixBinary<T, TInput>> NoAssoc { get; init; }
    }
}
