namespace Assets.SAT
{
    /// <summary>
    /// Represents a truth assignment for a problem,
    /// i.e. a mapping from propositions to truth values.
    /// </summary>
    public class TruthAssignment
    {
        /// <summary>
        /// Makes a random truth assignment for the propositions in the specified Problem.
        /// </summary>
        public TruthAssignment(Problem problem)
        {
            truthValues = new bool[problem.PropositionCount];

            foreach (var p in problem.Propositions)
                this[p] = Random.CoinToss;
        }

        /// <summary>
        /// Array of truth values for the different propositions,
        /// indexed by the Index field of the proposition.  So the
        /// truth value of p is in truthValues[p.Index].
        /// </summary>
        private readonly bool[] truthValues;

        /// <summary>
        /// Truth value of proposition in this assignment
        /// </summary>
        public bool this[Proposition p]
        {
            get => truthValues[p.Index];
            set => truthValues[p.Index] = value;
        }

        /// <summary>
        /// Truth value of literal in this TruthAssignment.
        /// If this[p] is false, then this[!p] will be true.
        /// </summary>
        public bool this[Literal l] => this[l.Proposition] == l.IsPositive;

        /// <summary>
        /// The number of literals in the clause that are true given this truth assignment.
        /// WARNING: this computes it from scratch.  It should not be used anywhere except
        /// the constructor for Problem, which it's initializing the the TrueLiteralCounts array.
        /// </summary>
        public int TrueLiteralCount(Clause c)
        {
            var count = 0;
            foreach (var d in c.Disjuncts)
                if (this[d])
                    count++;
            return count;
        }

        /// <summary>
        /// Flips the truth value of the proposition.
        /// This just updates the truth assignment.  It does not update any of the tables
        /// in the problem (TrueLiteralCounts or UnsatisfiedClauses).
        /// </summary>
        public void Flip(Proposition p) => truthValues[p.Index] = !truthValues[p.Index];
    }
}
