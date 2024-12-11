using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Utils
{
    public class Dijkstra<T>
    {

        public struct Node
        {
            public T Element;
            public int Distance;
            public Point? Previous;
        }

        private readonly Node[][] Map;
        public readonly int Width;
        public readonly int Height;
        public readonly Point Source;

        private Dijkstra(T[][] map, Point source, int initialDistance)
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

        public Node this[Point p]
        {
            get => Map[p.Y][p.X];
        }

        public IEnumerable<(Node Node, Point Location)> GetPathToSource(Point start)
        {
            Point p = start;
            while (p != Source)
            {
                yield return (Map[p.Y][p.X], p);
                p = (Point)Map[p.Y][p.X].Previous;
            }
            yield return (Map[p.Y][p.X], p);
        }

        // Distance formula args: (From, To)
        public static Dijkstra<T> Generate(T[][] map, Point source, Func<(Point, T), (Point, T), int> Distance = null)
        {
            if (Distance == null)
            {
                Distance = ((Point p1, T t1) a, (Point p2, T t2) b) => 1;
            }
            Dijkstra<T> result = new(map, source, int.MaxValue);
            result.Map[source.Y][source.X].Distance = 0;

            CustomPriorityQueue<Point> queue = new(true);
            for (int y = 0; y < result.Height; y++)
            {
                for (int x = 0; x < result.Width; x++)
                {
                    queue.Enqueue(new Point(x, y), result.Map[y][x].Distance);
                }
            }

            while (queue.Count > 0)
            {
                Point u = queue.Dequeue();
                foreach (Point v in result.GetNeighbors(u))
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
                        queue.UpdatePriority(v, alt, (x, y) => x == y);
                    }
                }
            }

            return result;
        }

        private IEnumerable<Point> GetNeighbors(Point p)
        {
            if (p.X < Width - 1)
            {
                yield return new Point(p.X + 1, p.Y);
            }
            if (p.Y < Height - 1)
            {
                yield return new Point(p.X, p.Y + 1);
            }
            if (p.X > 0)
            {
                yield return new Point(p.X - 1, p.Y);
            }
            if (p.Y > 0)
            {
                yield return new Point(p.X, p.Y - 1);
            }
        }

    }
}
