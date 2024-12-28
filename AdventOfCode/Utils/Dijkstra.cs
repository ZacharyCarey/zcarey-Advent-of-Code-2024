using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TVGL;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AdventOfCode.Utils
{
    public static class Dijkstra
    {

        public static IEnumerable<(long Cost, List<T> Path)> Search<T>(T start, T end, Func<T, IEnumerable<(T Node, long Cost)>> getNeighbors) where T : IEquatable<T>
        {
            return Search(start, node => node.Equals(end), getNeighbors);
        }

        // Returns all shortest paths
        public static IEnumerable<(long Cost, List<T> Path)> Search<T>(T start, Func<T, bool> isEnd, Func<T, IEnumerable<(T Node, long Cost)>> getNeighbors) where T : IEquatable<T>
        {
            // https://en.wikipedia.org/wiki/Dijkstra's_algorithm#Using_a_priority_queue
            // Uses some alternate options since updating the priority is not available, and not
            // all nodes are known during initialization
            Dictionary<T, List<T>> prev = new();
            HashSet<T> endNodes = new();
            long lowestCost = long.MaxValue;

            // create vertex priority queue Q
            PriorityQueue<T, long> Q = new();

            // dist[source] ← 0                          // Initialization
            Dictionary<T, long> dist = new();
            dist[start] = 0;

            // Q.add_with_priority(source, 0)            // associated priority equals dist[·]
            Q.Enqueue(start, 0);

            // while Q is not empty:                     // The main loop
            while (Q.Count > 0)
            {
                // u ← Q.extract_min()                   // Remove and return best vertex
                var u = Q.Dequeue();

                if (isEnd(u))
                {
                    endNodes.Add(u);
                    lowestCost = Math.Min(lowestCost, dist[u]);
                    continue;
                }

                // for each neighbor v of u:             // Go through all v neighbors of u
                foreach ((T v, long cost) in getNeighbors(u)) { 
                    // alt ← dist[u] + Graph.Edges(u, v)
                    long alt = dist[u] + cost;

                    // if alt < dist[v]:
                    long neighborDist = dist.GetValueOrDefault(v, long.MaxValue);
                    if (alt < neighborDist)
                    {
                        // prev[v] ← u
                        prev[v] = [u];

                        // dist[v] ← alt
                        dist[v] = alt;

                        // Q.add_with_priority(v, alt)
                        Q.Enqueue(v, alt);
                    } else if (alt == neighborDist)
                    {
                        // Used to find ALL lowest paths
                        prev[v].Add(u);
                    }
                }
            }

            
            foreach (var endNode in endNodes)
            {
                long cost = dist[endNode];
                if (cost == lowestCost)
                {
                    foreach (var path in GetPath(prev, start, endNode, new List<T>()))
                    {
                        yield return (cost, path);
                    }
                }
            }
        }

        private static IEnumerable<List<T>> GetPath<T>(Dictionary<T, List<T>> prev, T start, T node, List<T> path) where T : IEquatable<T>
        {
            List<T> prevNodes;
            while (!node.Equals(start))
            {
                path.Add(node);

                prevNodes = prev[node];
                foreach(var n in prevNodes.Skip(1))
                {
                    IEnumerable<List<T>> otherPaths = GetPath(prev, start, n, new List<T>(path));
                    foreach(var p in otherPaths)
                    {
                        yield return p;
                    }
                }

                node = prevNodes[0];
            }

            // The start has been reached
            path.Add(start);
            ReverseList(path);
            yield return path;
        }

        private static void ReverseList<T>(List<T> list)
        {
            T temp;
            for (int i = 0; i < list.Count / 2; i++) {
                int j = list.Count - i - 1;
                temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
    }
}
