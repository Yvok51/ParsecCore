
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

        /// <summary>
        /// Returns whether the <paramref name="actual"/> indentation level satisfies a given relation compared to
        /// <paramref name="reference"/> indentation level.
        /// For example, for <see cref="Relation.GT"/> we are looking whether the <paramref name="actual"/>
        /// indentation is greater than <paramref name="reference"/> indentation.
        /// In general the format is 
        /// <c><paramref name="actual"/> <paramref name="relation"/> <paramref name="reference"/></c>.
        /// </summary>
        /// <param name="relation"> The relation we want the two indentations to fullfil </param>
        /// <param name="reference"> The reference indentation we are comparing against </param>
        /// <param name="actual"> The actual indentation that we are testing </param>
        /// <returns> Whether <paramref name="actual"/> satisfies the relation </returns>
        /// <exception cref="ArgumentException"> If an unknown <see cref="Relation"/> value is provided </exception>
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
