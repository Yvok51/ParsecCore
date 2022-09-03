using ParsecCore.Help;
using System.Collections.Generic;
using System.Linq;

namespace ParsecCore
{
    internal class SepBy1Parser
    {
        public static Parser<IReadOnlyList<TValue>, TInputToken> Parser<TValue, TSeperator, TInputToken>(
            Parser<TValue, TInputToken> valueParser,
            Parser<TSeperator, TInputToken> seperatorParser
        )
        {
            var sepValueParser = from sep in seperatorParser
                                 from val in valueParser
                                 select val;
            return from firstParse in valueParser
                   from subsequentParses in sepValueParser.Many()
                   select subsequentParses.Prepend(firstParse);
        }
    }
}
