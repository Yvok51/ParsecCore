
using System;

namespace ParsecCore.Indentation
{
    public enum Relation
    {
        ANY,
        EQ,
        GE,
        GT
    }

    public static class RelationHelp
    {
        public static string ToPrettyString(this Relation relation)
        {
            return relation switch
            {
                Relation.EQ => "equal to",
                Relation.GT => "greater than",
                Relation.GE => "greater or equal to",
                Relation.ANY => "with no relation to",
                _ => throw new ArgumentException("Unknown relation", nameof(relation))
            };
        }

        public static bool Satisfies(this Relation relation, IndentLevel reference, IndentLevel actual)
        {
            return relation switch
            {
                Relation.EQ => actual == reference,
                Relation.GT => actual > reference,
                Relation.GE => actual >= reference,
                Relation.ANY => true,
                _ => throw new ArgumentException("Unknown relation", nameof(relation))
            };
        }
    }
}
