using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Assets.SAT
{
    /// <summary>
    /// Represents a clause in a problem, i.e. a disjunction of literals
    /// </summary>
    [DebuggerDisplay("{" + nameof(Text) + "}")]
    public class Clause
    {
        /// <summary>
        /// The literals in the clause
        /// </summary>
        public readonly Literal[] Disjuncts;

        /// <summary>
        /// Unique integer within the Problem assigned to this clause.
        /// Used for indexing into TrueLiteralCounts array.  So the
        /// count of literals for clause c is TrueLiteralCounts[c.Index].
        /// </summary>
        public readonly int Index;

        /// <summary>
        /// Returns a randomly chosen literal from the clause.
        /// </summary>
        public Literal RandomDisjunct => Disjuncts.RandomElement();

        /// <summary>
        /// Make a new clause containing the specified literals
        /// </summary>
        /// <param name="disjuncts">Literals to include in the clause</param>
        /// <param name="index">Position within the problem's clause table of this clause</param>
        public Clause(Literal[] disjuncts, int index)
        {
            Disjuncts = disjuncts;
            Index = index;

            foreach (var d in Disjuncts)
                if (d.IsPositive)
                    d.Proposition.PositiveClauses.Add(this);
                else
                    d.Proposition.NegativeClauses.Add(this);

            // Reconstruct the source text of the clause
            // We do this rather than keeping the original text
            // to help debug the parsing process.  If the parser
            // gets it wrong, we'll see what it produced.
            var b = new StringBuilder();
            var firstOne = true;
            foreach (var d in Disjuncts)
            {
                if (firstOne)
                    firstOne = false;
                else
                    b.Append(" | ");
                b.Append(d);
            }

            Text = b.ToString();
        }

        /// <summary>
        /// A clause containing the disjuncts specified in the expression
        /// </summary>
        /// <param name="expression">A string representing the clause, e.g. "a | !b | c"</param>
        /// <param name="problem"></param>
        public Clause(string expression, Problem problem) : this(ParseDisjuncts(expression, problem), problem.Clauses.Count)
        { }

        /// <summary>
        /// Returns the Literal objects corresponding to the text representation of a clause
        /// </summary>
        /// <param name="expression">The text for the clause, e.g. "a | !b | c"</param>
        /// <param name="problem">The problem this clause is a part of</param>
        /// <returns></returns>
        private static Literal[] ParseDisjuncts(string expression, Problem problem) 
            => expression.Split('|')
                .Select(subexpression => new Literal(subexpression.Trim(), problem))
                .ToArray();

        /// <summary>
        /// Textual representation of this clause, e.g. "a | !b | c".
        /// </summary>
        public readonly string Text;

        public override string ToString() => Text;
    }
}
