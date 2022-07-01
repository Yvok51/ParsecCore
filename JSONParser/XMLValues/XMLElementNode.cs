﻿using System.Collections.Generic;

namespace JSONtoXML
{
    class XMLElementNode : XMLNode
    {
        public string Tag { get; init; }
        public Dictionary<string, string> Attributes { get; init; }
        public IReadOnlyList<XMLNode> ChildNodes { get; init; }
    }
}