using ParsecCore.Help;
using System.Collections.Generic;
using System.Linq;

namespace ParsecCore
{
    class SepBy1Parser
    {
        public static Parser<IReadOnlyList<TValue>> Parser<TValue, TSeperator>(
            Parser<TValue> valueParser,
            Parser<TSeperator> seperatorParser
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
