using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParsecCore.Input
{
    public class ParserInput
    {
        public static IParserInput Create(string inputString)
        {
            return new StringParserInput(inputString);
        }
    }
}
