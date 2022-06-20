using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParsecCore.Input
{
    struct Position
    {
        public int Line { get; init; }
        public int Column { get; init; }

        public override string ToString()
        {
            return $"{Line}:{Column}";
        }
    }
}
