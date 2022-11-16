using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParsecCore.Permutations
{
    internal static partial class Permutation
    {
        public static Parser<TR, TParserInput> Permute<TA, TParserInput, TR>(
            SplitParser<TA, TParserInput> splitParser,
            Func<TA, TR> f
        )
        {
            int branchCount = splitParser.IsOptional ? 2 : 1;
            List<Parser<TR, TParserInput>> branches = new(branchCount);

            branches.Add(
                from a in splitParser.Parser
                select f(a)
            );

            if (splitParser.IsOptional)
            {
                branches.Add(Parsers.Return<TR, TParserInput>(f(splitParser.DefaultValue)));
            }

            return Combinators.Choice(branches);
        }

        public static Parser<TR, TParserInput> Permute<TA, TB, TParserInput, TR>(
            SplitParser<TA, TParserInput> splitParserA,
            SplitParser<TB, TParserInput> splitParserB,
            Func<TA, TB, TR> f
        )
        {
            int branchCount = splitParserA.IsOptional && splitParserB.IsOptional ? 3 : 2;
            List<Parser<TR, TParserInput>> branches = new(branchCount);

            branches.Add(
                from a in splitParserA.Parser
                from r in Permute(splitParserB, f.Partial(a))
                select r
            );
            branches.Add(
                from b in splitParserB.Parser
                from r in Permute(splitParserA, f.Partial(b))
                select r
            );

            if (splitParserA.IsOptional && splitParserB.IsOptional)
            {
                branches.Add(Parsers.Return<TR, TParserInput>(f(splitParserA.DefaultValue, splitParserB.DefaultValue)));
            }

            return Combinators.Choice(branches);
        }

        public static Parser<TR, TParserInput> Permute<TA, TB, TC, TParserInput, TR>(
            SplitParser<TA, TParserInput> splitParserA,
            SplitParser<TB, TParserInput> splitParserB,
            SplitParser<TC, TParserInput> splitParserC,
            Func<TA, TB, TC, TR> f
        )
        {
            int branchCount = splitParserA.IsOptional && splitParserB.IsOptional && splitParserC.IsOptional ? 3 : 2;
            List<Parser<TR, TParserInput>> branches = new(branchCount);

            branches.Add(
                from a in splitParserA.Parser
                from r in Permute(splitParserB, splitParserC, f.Partial(a))
                select r
            );
            branches.Add(
                from b in splitParserB.Parser
                from r in Permute(splitParserA, splitParserC, f.Partial(b))
                select r
            );
            branches.Add(
                from c in splitParserC.Parser
                from r in Permute(splitParserA, splitParserB, f.Partial(c))
                select r
            );

            if (splitParserA.IsOptional && splitParserB.IsOptional && splitParserC.IsOptional)
            {
                branches.Add(Parsers.Return<TR, TParserInput>(f(splitParserA.DefaultValue, splitParserB.DefaultValue, splitParserC.DefaultValue)));
            }

            return Combinators.Choice(branches);
        }
    }
}
