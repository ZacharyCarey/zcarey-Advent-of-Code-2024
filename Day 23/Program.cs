using AdventOfCode;
using AdventOfCode.Parsing;
using AdventOfCode.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Day_23
{
    internal class Program : ProgramStructure<IEnumerable<string[]>>
    {

        Program() : base(x => x.GetLines().Select(line => line.Split('-')))
        { }

        static void Main(string[] args)
        {
            new Program()
                .Run(args);
                //.Run(args, "Example.txt");
        }

        protected override object SolvePart1(IEnumerable<string[]> input)
        {
            var connections = GetConnections(input);
            var groups = GetGroupsOf3(connections);

            return groups.Where(
                group => group.Where(computer => computer.StartsWith('t')).Any()
            ).Count();
        }

        internal Dictionary<string, string[]> GetConnections(IEnumerable<string[]> input)
        {
            Dictionary<string, HashSet<string>> lookup = new();
            foreach (var pair in input)
            {
                HashSet<string> connections;
                if (!lookup.TryGetValue(pair[0], out connections))
                {
                    connections = new HashSet<string>();
                    lookup[pair[0]] = connections;
                }
                connections.Add(pair[1]);

                if (!lookup.TryGetValue(pair[1], out connections))
                {
                    connections = new HashSet<string>();
                    lookup[pair[1]] = connections;
                }
                connections.Add(pair[0]);
            }

            Dictionary<string, string[]> result = new();
            foreach(var pair in lookup)
            {
                result[pair.Key] = pair.Value.ToArray();
            }
            return result;
        }

        internal static HashSet<LAN> GetGroupsOf3(Dictionary<string, string[]> allConnections)
        {
            HashSet<LAN> groups = new();
            foreach((string key, string[] connections) in allConnections)
            {
                // Find pairs in connections that are also connected together
                foreach(var pair in GetPairs(connections))
                {
                    if (allConnections.TryGetValue(pair.Item1, out string[] pairConnections) && pairConnections.Contains(pair.Item2))
                    {
                        // HashSet combined with LAN struct should reduce to only unique groups
                        groups.Add(new LAN(key, pair.Item1, pair.Item2));
                    }
                }
            }
            return groups;
        }

        internal static IEnumerable<(T Item1, T Item2)> GetPairs<T>(T[] input)
        {
            for (int i = 0; i < input.Length - 1; i++)
            {
                for (int j = i + 1; j < input.Length; j++)
                {
                    yield return (input[i], input[j]);
                }
            }
        }

        public struct LAN : IEnumerable<string>
        {
            public string[] Computers;

            public LAN(params string[] computers)
            {
                this.Computers = computers.Order().ToArray();
            }

            public static bool operator ==(LAN lhs, LAN rhs)
            {
                return lhs.Computers.SequenceEqual(rhs.Computers);
            }

            public static bool operator !=(LAN lhs, LAN rhs)
            {
                return !(lhs == rhs);
            }

            public override bool Equals([NotNullWhen(true)] object obj)
            {
                if ((obj != null) && (obj is LAN other))
                {
                    return this == other;
                }
                return false;
            }

            public override int GetHashCode()
            {
                if (this.Computers.Length == 0)
                {
                    return 0;
                } else
                {
                    return HashCode.Combine(Computers.Length.GetHashCode(), Computers[0].GetHashCode());
                }
            }

            public IEnumerator<string> GetEnumerator()
            {
                return ((IEnumerable<string>)this.Computers).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.Computers.GetEnumerator();
            }

            public override string ToString()
            {
                return string.Join(", ", this.Computers);
            }
        }

        protected override object SolvePart2(IEnumerable<string[]> input)
        {
            var connections = GetConnections(input);
            HashSet<LAN> groups = GetGroups(connections);
            LAN largest = groups.MaxBy(group => group.Computers.Length);
            return string.Join(',', largest); // the LAN struct already orders elements alphebetically
        }

        internal static HashSet<LAN> GetGroups(Dictionary<string, string[]> allConnections)
        {
            HashSet<LAN> groups = new();
            foreach ((string key, string[] connections) in allConnections)
            {
                HashSet<string> remaining = new HashSet<string>(connections);
                while (remaining.Count > 0)
                {
                    List<string> currentGroup = new List<string>(remaining.Count);
                    currentGroup.Add(key);
                    foreach (string connection in remaining)
                    {
                        string[] otherConnections = allConnections[connection];
                        // Only add if it's connected to each computer in this group
                        bool connected = true;
                        foreach(string currentConnection in currentGroup)
                        {
                            if (!otherConnections.Contains(currentConnection))
                            {
                                connected = false;
                                break;
                            }
                        }
                        if (connected)
                        {
                            currentGroup.Add(connection);
                            remaining.Remove(connection);
                        }
                    }
                    groups.Add(new LAN(currentGroup.ToArray()));
                }
            }
            return groups;
        }
    }
}
