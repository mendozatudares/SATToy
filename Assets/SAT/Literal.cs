using System.Diagnostics;

namespace Assets.SAT
{
    /// <summary>
    /// Represents a literal in a clause, i.e. either a
    /// proposition or its negation
    /// </summary>
    [DebuggerDisplay("{" + nameof(PrintedForm) + "}")]
    public class Literal
    {
        /// <summary>
        /// The proposition referred to in the literal
        /// For both the literals p and !p, this will be the proposition p.
        /// </summary>
        public readonly Proposition Proposition;

        /// <summary>
        /// True if the literal is just the proposition
        /// So this will be true for p, and false for !p.
        /// </summary>
        public readonly bool IsPositive;

        /// <summary>
        /// A literal within a clause
        /// </summary>
        /// <param name="proposition">The proposition involved in the literal</param>
        /// <param name="isPositive">True unless the proposition is negated</param>
        public Literal(Proposition proposition, bool isPositive)
        {
            Proposition = proposition;
            IsPositive = isPositive;
        }

        /// <summary>
        /// A literal within a clause
        /// </summary>
        /// <param name="expression">Printed representation of the literal, e.g. "p" or "!p"</param>
        /// <param name="problem">Problem whose clause this literal will be stored in</param>
        public Literal(string expression, Problem problem)
        {
            IsPositive = !expression.StartsWith("!");
            Proposition = problem[expression.Replace("!", "").Trim()];
        }

        /// <summary>
        /// How to print a literal
        /// </summary>
        public string PrintedForm => IsPositive ? Proposition.Name : "!" + Proposition.Name;

        public override string ToString() => PrintedForm;
    }
}
