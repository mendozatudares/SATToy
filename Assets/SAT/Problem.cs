using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.SAT
{
    /// <summary>
    /// Represents a SAT problem
    /// </summary>
    public class Problem
    {
        /// <summary>
        /// Make a problem from the clauses specified in the file
        /// The format of the file is:
        /// - 1 line per clause
        /// - ! means not, | means or
        /// - proposition names can be anything not including those characters
        /// </summary>
        /// <param name="path">Path to the file to load</param>
        public Problem(string path)
        {
            Clauses.AddRange(File.ReadAllLines(path).Select(clauseExp => new Clause(clauseExp, this)));
            TrueLiteralCounts = new int[Clauses.Count];

            Solution = new TruthAssignment(this);

            for (var i = 0; i < Clauses.Count; i++)
                TrueLiteralCounts[i] = Solution.TrueLiteralCount(Clauses[i]);

            UnsatisfiedClauses.AddRange(Clauses.Where(Unsatisfied));
        }
        
        /// <summary>
        /// The truth assignment we are trying to make into a solution.
        /// This starts completely random is then is gradually changed into
        /// a solution by "flipping" the values of specific propositions
        /// </summary>
        public TruthAssignment Solution;

        /// <summary>
        /// Percent of the time we do a random walk step rather than a greedy one.
        /// 0   = pure greedy
        /// 100 = pure random walk
        /// </summary>
        public int NoiseLevel = 10;

        #region Proposition information
        /// <summary>
        /// The Proposition object within this problem with the specified name.
        /// Creates a new proposition object if necessary.
        /// </summary>
        public Proposition this[string name]
        {
            get
            {
                if (propositionTable.TryGetValue(name, out var result))
                    return result;
                return propositionTable[name ] = new Proposition(name, propositionTable.Count);
            }
        }

        /// <summary>
        /// Hash table mapping names (string) to the Proposition objects with that name
        /// </summary>
        private readonly Dictionary<string, Proposition> propositionTable = new Dictionary<string, Proposition>();

        /// <summary>
        /// Enumeration of all the propositions in the problem
        /// </summary>
        public IEnumerable<Proposition> Propositions => propositionTable.Select(pair => pair.Value);

        /// <summary>
        /// Total number of propositions in the problem.
        /// Note this is the number of propositions, not the number of disjuncts in clauses.
        /// If a Proposition appears in 3 clauses, it's only counted once here.
        /// </summary>
        public int PropositionCount => propositionTable.Count;

        /// <summary>
        /// True if the current value of Solution is in fact a solution.
        /// If it's false, then we need to work on it some more.
        /// </summary>
        public bool IsSolved => UnsatisfiedClauses.Count == 0;
        #endregion

        #region Clause information
        /// <summary>
        /// Clauses in the SAT problem.
        /// </summary>
        public readonly List<Clause> Clauses = new List<Clause>();

        /// <summary>
        /// List of clauses whose disjuncts are presently all false.
        /// The solver needs to make these true.
        /// </summary>
        public readonly List<Clause> UnsatisfiedClauses = new List<Clause>();

        /// <summary>
        /// Number of literals in each clause that are true, indexed by the Index field of the clause.
        /// So to find out how many literals are true in c, look at TrueLiteralCounts[c.Index].
        /// </summary>
        public int[] TrueLiteralCounts;

        /// <summary>
        /// True if the specified clause is currently satisfied
        /// (i.e. if it's true in the current truth assignment)
        /// </summary>
        public bool Satisfied(Clause c) => TrueLiteralCounts[c.Index] > 0;

        /// <summary>
        /// True if the specified clause is currently unsatisfied
        /// (i.e. false in the current truth assignment).
        /// </summary>
        public bool Unsatisfied(Clause c) => !Satisfied(c);
        
        /// <summary>
        /// Checks that the TrueLiteralCounts array and UnsatisfiedClauses list are correct.
        /// Use this to look for bugs in your implementation of Flip.
        /// </summary>
        public void CheckConsistency()
        {
            for (var i = 0; i < Clauses.Count; i++)
                if (TrueLiteralCounts[i] != Solution.TrueLiteralCount(Clauses[i]))
                    throw new Exception($"True literal count incorrect for clause {i}");
            foreach (var c in Clauses)
            {
                var count = TrueLiteralCounts[c.Index];
                var present = UnsatisfiedClauses.IndexOf(c) >= 0;
                if (count > 0)
                {
                    if (present)
                        throw new Exception($"Clause {c.Index} appears in UnsatisfiedClauses but is satisfied");
                }
                else if (!present)
                    throw new Exception($"Clause {c.Index} is unsatisfied but does not appear in the UnsatisfiedClauses list");
            }
        }
        #endregion

        #region Solver
        /// <summary>
        /// Pick one variable to flip and flip it by calling Flip, below.
        /// </summary>
        /// <returns>True if all clauses are satisfied.</returns>
        public bool StepOne()
        {
            // Replace with your code
            var c = UnsatisfiedClauses.RandomElement();
            Literal literal;

            if (Random.Percent(NoiseLevel))
            {
                literal = c.Disjuncts.RandomElement();
            }
            else
            {
                var deltas = c.Disjuncts.Select(l => SatisfiedClauseDelta(l.Proposition)).ToList();
                literal = c.Disjuncts[deltas.IndexOf(deltas.Max())];
            }

            Flip(literal);

            return IsSolved;
        }

        /// <summary>
        /// Flip the value of the specified literal.
        /// Call Solution.Flip to do the actual flipping.  But make sure to update
        /// TrueLiteralCounts and UnsatisfiedClauses, accordingly
        /// </summary>
        void Flip(Literal l)
        {
            Solution.Flip(l.Proposition);

            if (Solution[l.Proposition])
            {
                foreach (var c in l.Proposition.PositiveClauses)
                {
                    TrueLiteralCounts[c.Index] += 1;
                    if (TrueLiteralCounts[c.Index] == 1)
                    {
                        UnsatisfiedClauses.Remove(c);
                    }
                }
                foreach (var c in l.Proposition.NegativeClauses)
                {
                    TrueLiteralCounts[c.Index] -= 1;
                    if (TrueLiteralCounts[c.Index] == 0)
                    {
                        UnsatisfiedClauses.Add(c);
                    }
                }
            }
            else
            {
                foreach (var c in l.Proposition.PositiveClauses)
                {
                    TrueLiteralCounts[c.Index] -= 1;
                    if (TrueLiteralCounts[c.Index] == 0)
                    {
                        UnsatisfiedClauses.Add(c);
                    }
                }
                foreach (var c in l.Proposition.NegativeClauses)
                {
                    TrueLiteralCounts[c.Index] += 1;
                    if (TrueLiteralCounts[c.Index] == 1)
                    {
                        UnsatisfiedClauses.Remove(c);
                    }
                }
            }

        }

        /// <summary>
        /// Return the net increase or decrease in satisfied clauses if we were to flip this proposition
        /// </summary>
        int SatisfiedClauseDelta(Proposition p)
        {
            // Replace with your code
            var newlySatisfied = 0;
            var newlyUnsatisfied = 0;

            if (Solution[p])
            {
                foreach (var c in p.PositiveClauses)
                {
                    if (TrueLiteralCounts[c.Index] == 1)
                    {
                        newlyUnsatisfied += 1;
                    }
                }
                foreach (var c in p.NegativeClauses)
                {
                    if (TrueLiteralCounts[c.Index] == 0)
                    {
                        newlySatisfied += 1;
                    }
                }
            }
            else
            {
                foreach (var c in p.PositiveClauses)
                {
                    if (TrueLiteralCounts[c.Index] == 0)
                    {
                        newlySatisfied += 1;
                    }
                }
                foreach (var c in p.NegativeClauses)
                {
                    if (TrueLiteralCounts[c.Index] == 1)
                    {
                        newlyUnsatisfied += 1;
                    }
                }
            }

            return newlySatisfied - newlyUnsatisfied;
        }
        #endregion
    }
}
