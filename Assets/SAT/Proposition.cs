using System.Collections.Generic;
using System.Diagnostics;

namespace Assets.SAT
{
    /// <summary>
    /// Represents a proposition within a Problem
    /// </summary>
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    public class Proposition
    {
        /// <summary>
        /// Name of the proposition provided by the user
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// A unique number for this Proposition within its Problem.
        /// Used as an index into a truth assignment.
        /// </summary>
        public readonly int Index;

        public Proposition(string name, int index)
        {
            Name = name;
            Index = index;
        }

        /// <summary>
        /// List of clauses in which this proposition appears in unnegated form.
        /// </summary>
        public readonly List<Clause> PositiveClauses = new List<Clause>();

        /// <summary>
        /// List of clauses in which this proposition appears negated.
        /// </summary>
        public readonly List<Clause> NegativeClauses = new List<Clause>();
    }
}