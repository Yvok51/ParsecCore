using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParsecCore.Permutations
{
    class Perms<TA, TParserInput, TR>
    {
        public Perms(SplitParser<TA, TParserInput> splitParser, Func<TA, TR> f)
        {
            _splitParser = splitParser;
            _f = f;
        }

        public Parser<TR, TParserInput> Permute()
        {
            int branchCount = _splitParser.IsOptional ? 2 : 1;
            List<Parser<TR, TParserInput>> branches = new(branchCount);

            branches.Add(
                from a in _splitParser.Parser
                select _f(a)
            );

            if (_splitParser.IsOptional)
            {
                branches.Add(Parsers.Return<TR, TParserInput>(_f(_splitParser.DefaultValue)));
            }

            return Combinators.Choice(branches);
        }

        private readonly SplitParser<TA, TParserInput> _splitParser;
        private readonly Func<TA, TR> _f;
    }

    class Perms<TA, TB, TParserInput, TR>
    {
        public Perms(
            SplitParser<TA, TParserInput> splitParserA,
            SplitParser<TB, TParserInput> splitParserB,
            Func<TA, TB, TR> f)
        {
            _splitParserA = splitParserA;
            _splitParserB = splitParserB;
            _f = f;
        }

        public Parser<TR, TParserInput> Permute()
        {
            int branchCount = _splitParserA.IsOptional && _splitParserB.IsOptional ? 3 : 2;
            List<Parser<TR, TParserInput>> branches = new(branchCount);

            branches.Add(
                from a in _splitParserA.Parser
                from r in (new Perms<TB, TParserInput, TR>(_splitParserB, _f.Partial(a))).Permute()
                select r
            );
            branches.Add(
                from b in _splitParserB.Parser
                from r in (new Perms<TA, TParserInput, TR>(_splitParserA, _f.Partial(b))).Permute()
                select r
            );

            if (_splitParserA.IsOptional && _splitParserB.IsOptional)
            {
                branches.Add(Parsers.Return<TR, TParserInput>(_f(_splitParserA.DefaultValue, _splitParserB.DefaultValue)));
            }

            return Combinators.Choice(branches);
        }

        private readonly SplitParser<TA, TParserInput> _splitParserA;
        private readonly SplitParser<TB, TParserInput> _splitParserB;
        private readonly Func<TA, TB, TR> _f;
    }

    class Perms<TA, TB, TC, TParserInput, TR>
    {
        public Perms(
            SplitParser<TA, TParserInput> splitParserA,
            SplitParser<TB, TParserInput> splitParserB,
            SplitParser<TC, TParserInput> splitParserC,
            Func<TA, TB, TC, TR> f)
        {
            _splitParserA = splitParserA;
            _splitParserB = splitParserB;
            _splitParserC = splitParserC;
            _f = f;
        }

        public Parser<TR, TParserInput> Permute()
        {
            int branchCount = _splitParserA.IsOptional && _splitParserB.IsOptional && _splitParserC.IsOptional ? 3 : 2;
            List<Parser<TR, TParserInput>> branches = new(branchCount);

            branches.Add(
                from a in _splitParserA.Parser
                from r in (new Perms<TB, TC, TParserInput, TR>(_splitParserB, _splitParserC, _f.Partial(a))).Permute()
                select r
            );
            branches.Add(
                from b in _splitParserB.Parser
                from r in (new Perms<TA, TC, TParserInput, TR>(_splitParserA, _splitParserC, _f.Partial(b))).Permute()
                select r
            );
            branches.Add(
                from c in _splitParserC.Parser
                from r in (new Perms<TA, TB, TParserInput, TR>(_splitParserA, _splitParserB, _f.Partial(c))).Permute()
                select r
            );

            if (_splitParserA.IsOptional && _splitParserB.IsOptional && _splitParserC.IsOptional)
            {
                branches.Add(Parsers.Return<TR, TParserInput>(_f(_splitParserA.DefaultValue, _splitParserB.DefaultValue, _splitParserC.DefaultValue)));
            }

            return Combinators.Choice(branches);
        }

        private readonly SplitParser<TA, TParserInput> _splitParserA;
        private readonly SplitParser<TB, TParserInput> _splitParserB;
        private readonly SplitParser<TC, TParserInput> _splitParserC;
        private readonly Func<TA, TB, TC, TR> _f;
    }
}
