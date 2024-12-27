using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TVGL;

namespace AdventOfCode.Utils
{
    public class Dijkstra<T>
    {

        public struct Node
        {
            public T Element;
            public int Distance;
            public LargePoint? Previous;
        }

        private readonly Node[][] Map;
        public readonly int Width;
        public readonly int Height;
        public readonly LargePoint Source;

        private Dijkstra(T[][] map, LargePoint source, int initialDistance)
        {
            this.Source = source;
            Map = new Node[map.Length][];
            Width = map[0].Length;
            Height = map.Length;
            for (int y = 0; y < Height; y++)
            {
                Map[y] = new Node[Width];
                for (int x = 0; x < Width; x++)
                {
                    Map[y][x] = new Node();
                    Map[y][x].Element = map[y][x];
                    Map[y][x].Distance = initialDistance;
                    Map[y][x].Previous = null;
                }
            }
        }

        public Node this[int x, int y]
        {
            get => Map[y][x];
        }

        public Node this[LargePoint p]
        {
            get => Map[p.Y][p.X];
        }

        public IEnumerable<(Node Node, LargePoint Location)> GetPathToSource(LargePoint start)
        {
            LargePoint p = start;
            while (p != Source)
            {
                yield return (Map[p.Y][p.X], p);
                p = (LargePoint)Map[p.Y][p.X].Previous;
            }
            yield return (Map[p.Y][p.X], p);
        }

        // Distance formula args: (From, To)
        public static Dijkstra<T> Generate(T[][] map, LargePoint source, Func<(LargePoint, T), (LargePoint, T), int> Distance = null)
        {
            if (Distance == null)
            {
                Distance = ((LargePoint p1, T t1) a, (LargePoint p2, T t2) b) => 1;
            }
            Dijkstra<T> result = new(map, source, int.MaxValue);
            result.Map[source.Y][source.X].Distance = 0;

            UpdatablePriorityQueue<LargePoint, int> queue = new();
            for (int y = 0; y < result.Height; y++)
            {
                for (int x = 0; x < result.Width; x++)
                {
                    queue.Enqueue(new LargePoint(x, y), result.Map[y][x].Distance);
                }
            }

            while (queue.Count > 0)
            {
                LargePoint u = queue.Dequeue();
                foreach (LargePoint v in result.GetNeighbors(u))
                {
                    int travelDist = Distance((u, map[u.Y][u.X]), (v, map[v.Y][v.X]));
                    if (travelDist == int.MaxValue)
                    {
                        // Assume this neighbor can't be traveled to
                        continue;
                    }

                    int alt = (int)Math.Min((long)result.Map[u.Y][u.X].Distance + (long)travelDist, int.MaxValue);
                    if (alt < result.Map[v.Y][v.X].Distance)
                    {
                        result.Map[v.Y][v.X].Distance = alt;
                        result.Map[v.Y][v.X].Previous = u;
                        queue.UpdatePriority(v, alt);
                    }
                }
            }

            return result;
        }

        private IEnumerable<LargePoint> GetNeighbors(LargePoint p)
        {
            if (p.X < Width - 1)
            {
                yield return new LargePoint(p.X + 1, p.Y);
            }
            if (p.Y < Height - 1)
            {
                yield return new LargePoint(p.X, p.Y + 1);
            }
            if (p.X > 0)
            {
                yield return new LargePoint(p.X - 1, p.Y);
            }
            if (p.Y > 0)
            {
                yield return new LargePoint(p.X, p.Y - 1);
            }
        }

    }
}
