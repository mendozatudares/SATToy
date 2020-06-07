using System.Collections.Generic;

namespace Assets.SAT
{
    /// <summary>
    /// Random number generator
    /// To make debugging easier, the generator uses a fixed seed,
    /// so you'll get the same sequence of results on each run.
    /// </summary>
    public static class Random
    {
        // Use the .NET / Mono random generator.
        private static readonly System.Random Rand = new System.Random(1234567899);

        /// <summary>
        /// Returns a random Boolean
        /// </summary>
        public static bool CoinToss => Rand.Next(2) != 0;

        /// <summary>
        /// Return true the specified percentage of the time
        /// </summary>
        public static bool Percent(int percentTrueChance) => Rand.Next(100) < percentTrueChance;

        /// <summary>
        /// Returns a random element of a list
        /// </summary>
        public static T RandomElement<T>(this List<T> list) => list[Rand.Next(list.Count)];

        /// <summary>
        /// Returns a random element of an array
        /// </summary>
        public static T RandomElement<T>(this T[] array) => array[Rand.Next(array.Length)];
    }
}
