using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONParser
{
    struct ObjectKeyValuePair
    {
        public StringValue Key { get; init; }
        public JsonValue Value { get; init; }
    }
}
