namespace JSONtoXML.XMLValues
{
    internal interface IXMLVisitor<T, A>
    {
        T ElementVisit(XMLElementNode element, A arg);
        T TextVisit(XMLTextNode text, A arg);
    }
}
