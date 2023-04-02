using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONtoXML.XMLValues
{
    internal interface IXMLVisitor<T, A>
    {
        T ElementVisit(XMLElementNode element, A arg);
        T TextVisit(XMLTextNode text, A arg);
    }
}
