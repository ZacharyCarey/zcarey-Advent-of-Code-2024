using AdventOfCode;
using AdventOfCode.Parsing;
using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Day_22
{
    internal class Program : ProgramStructure<IEnumerable<long>>
    {

        Program() : base(x => x.GetLines().Select(long.Parse))
        { }

        static void Main(string[] args)
        {
            new Program()
                .Run(args);
                //.Run(args, "Example.txt");
                //.Run(args, "Example2.txt");
        }

        protected override object SolvePart1(IEnumerable<long> input)
        {
            return input.Select(seed => GetNthSecret(seed, 2000)).Sum();
        }

        private static long NextSecret(long secret)
        {
            secret = ((secret * 64) ^ secret) % 16777216;
            secret = ((secret / 32) ^ secret) % 16777216;
            secret = ((secret * 2048) ^ secret) % 16777216;
            return secret;
        }

        private static long GetNthSecret(long seed, int n)
        {
            long secret = seed;
            for (int i = 0; i < n; i++)
            {
                secret = NextSecret(secret);
            }
            return secret;
        }

        protected override object SolvePart2(IEnumerable<long> input)
        {
            Dictionary<PriceChanges, long> bananaPriceTotals = new();
            Queue<int> history = new Queue<int>(4);
            foreach(var seed in input)
            {
                Dictionary<PriceChanges, long> firstOccurance = new();
                history.Clear();
                history.Enqueue(0); // dummy value
                IEnumerable<(int Price, int Change)> priceChanges = GetPriceChanges(GetPrices(CalculateSecrets(seed)));
                foreach (var priceChange in priceChanges.Take(3))
                {
                    history.Enqueue(priceChange.Change);
                }
                foreach((int price, int change) in priceChanges.Take(2000).Skip(3))
                {
                    // Update the last 4 price changes
                    history.Dequeue();
                    history.Enqueue(change);
                    PriceChanges changes = new PriceChanges(history);

                    // Update banana values, but only if it's the first occurance of this pattern
                    if (!firstOccurance.ContainsKey(changes))
                    {
                        firstOccurance[changes] = price;
                    }
                }

                // Add together the first occurance values to the totals
                foreach((PriceChanges changes, long price) in firstOccurance)
                {
                    long total = bananaPriceTotals.GetValueOrDefault(changes, 0);
                    total += price;
                    bananaPriceTotals[changes] = total;
                }
            }

            // Now simply return the largest
            return bananaPriceTotals.Values.Max();
        }

        private static IEnumerable<long> CalculateSecrets(long seed)
        {
            long secret = seed;
            yield return secret;
            while (true)
            {
                secret = NextSecret(secret);
                yield return secret;
            }
        }

        private static IEnumerable<int> GetPrices(IEnumerable<long> secrets)
        {
            return secrets.Select(secret => (int)(secret % 10));
        }

        private static IEnumerable<(int Price, int Change)> GetPriceChanges(IEnumerable<int> prices)
        {
            int last = prices.First();
            foreach (int price in prices.Skip(1))
            {
                int delta = price - last;
                yield return (price, delta);
                last = price;
            }
        }
    }

    internal struct PriceChanges
    {
        public byte Change1;
        public byte Change2;
        public byte Change3;
        public byte Change4;

        public PriceChanges(IEnumerable<int> values)
        {
            var itr = values.GetEnumerator();
            itr.MoveNext();
            Change1 = (byte)itr.Current;
            itr.MoveNext();
            Change2 = (byte)itr.Current;
            itr.MoveNext();
            Change3 = (byte)itr.Current;
            itr.MoveNext();
            Change4 = (byte)itr.Current;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Change1, Change2, Change3, Change4);
        }

        public static bool operator ==(PriceChanges lhs, PriceChanges rhs)
        {
            return lhs.Change1 == rhs.Change1
                && lhs.Change2 == rhs.Change2
                && lhs.Change3 == rhs.Change3
                && lhs.Change4 == rhs.Change4;
        }

        public static bool operator != (PriceChanges lhs, PriceChanges rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals([NotNullWhen(true)] object obj)
        {
            if (obj != null && obj is PriceChanges other)
            {
                return this == other;
            }

            return false;
        }
    }
}
