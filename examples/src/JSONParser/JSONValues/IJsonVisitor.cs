using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONtoXML.JSONValues
{
    internal interface IJsonVisitor<T, A>
    {
        public T VisitArray(ArrayValue value, A arg);
        public T VisitBool(BoolValue value, A arg);
        public T VisitNull(NullValue value, A arg);
        public T VisitNumber(NumberValue value, A arg);
        public T VisitObject(ObjectValue value, A arg);
        public T VisitString(StringValue value, A arg);
    }
}
