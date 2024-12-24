using AdventOfCode;
using AdventOfCode.Parsing;
using AdventOfCode.Utils;
using Day_04;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using static Day_12.Program;

namespace Day_12
{
    internal class Program : ProgramStructure<CharMap>
    {

        Program() : base(CharMap.Parse)
        { }

        static void Main(string[] args)
        {
            new Program()
                .Run(args);
                //.Run(args, "Example.txt");
                //.Run(args, "Example2.txt");
                //.Run(args, "Example3.txt");
                //.Run(args, "Example4.txt");
                //.Run(args, "Example5.txt");
        }

        protected override object SolvePart1(CharMap input)
        {
            Region?[,] regions = new Region?[input.Width, input.Height];
            
            for (int y = 0; y < input.Height; y++)
            {
                for (int x = 0; x < input.Width; x++)
                {
                    if (regions[x, y] == null)
                    {
                        FloodFill(input, regions, x, y);
                    }
                }
            }

            // Now calculate perimiters
            for (int y = 0; y < input.Height; y++)
            {
                for (int x = 0; x < input.Width; x++)
                {
                    Region region = regions[x, y];

                    // Left border
                    if (x == 0)
                    {
                        region.Perimeter++;
                    } else
                    {
                        if (regions[x - 1, y] != region)
                        {
                            region.Perimeter++;
                            regions[x - 1, y].Perimeter++;
                        }
                    }
                    
                    // Special case right border
                    if (x == input.Width - 1)
                    {
                        region.Perimeter++;
                    }

                    // Upper border
                    if (y == 0)
                    {
                        region.Perimeter++;
                    } else
                    {
                        if (regions[x, y - 1] != region)
                        {
                            region.Perimeter++;
                            regions[x, y - 1].Perimeter++;
                        }
                    }

                    // Special case bottom border
                    if (y == input.Height - 1)
                    {
                        region.Perimeter++;
                    }
                }
            }

            return regions.AsEnumerable().Distinct().Select(region => (long)region.Area * region.Perimeter).Sum();
        }

        internal class Region
        {
            public char Plant;
            public int Area = 0;
            public int Perimeter = 0;
            public List<Fence> HFences = new();
            public List<Fence> VFences = new();

            public Region(char plant)
            {
                Plant = plant;
            }
        }

        // https://en.wikipedia.org/wiki/Flood_fill#Moving_the_recursion_into_a_data_structure
        private static void FloodFill(CharMap input, Region?[,] regions, int nodeX, int nodeY)
        {
            Region region = new Region(input[nodeX, nodeY]);

            // 1.Set Q to the empty queue or stack.
            Queue<Point> Q = new();

            // 2.Add node to the end of Q.
            Q.Enqueue(new Point(nodeX, nodeY));

            // 3.While Q is not empty:
            while (Q.Count > 0)
            {
                // 4.Set n equal to the first element of Q.
                // 5.Remove first element from Q.
                Point n = Q.Dequeue();

                // 6.If n is Inside:
                if (n.X >= 0 && n.Y >= 0 && n.X < input.Width && n.Y < input.Height && input[n] == region.Plant && regions[n.X, n.Y] == null)
                {
                    // Set the n
                    region.Area++;
                    regions[n.X, n.Y] = region;

                    // Add the node to the west of n to the end of Q.
                    Q.Enqueue(new Point(n.X - 1, n.Y));

                    // Add the node to the east of n to the end of Q.
                    Q.Enqueue(new Point(n.X + 1, n.Y));

                    // Add the node to the north of n to the end of Q.
                    Q.Enqueue(new Point(n.X, n.Y - 1));

                    // Add the node to the south of n to the end of Q.
                    Q.Enqueue(new Point(n.X, n.Y + 1));
                }
                // 7.Continue looping until Q is exhausted.
            }

            // 8.Return.
        }

        protected override object SolvePart2(CharMap input)
        {
            Region?[,] regions = new Region?[input.Width, input.Height];

            for (int y = 0; y < input.Height; y++)
            {
                for (int x = 0; x < input.Width; x++)
                {
                    if (regions[x, y] == null)
                    {
                        FloodFill(input, regions, x, y);
                    }
                }
            }

            // Now calculate perimiters
            for (int y = 0; y < input.Height; y++)
            {
                for (int x = 0; x < input.Width; x++)
                {
                    Region region = regions[x, y];

                    // Left border
                    CheckForFence(
                        region,
                        x == 0,
                        region.VFences,
                        y, x,
                        false,
                        () => regions[x - 1, y]
                    );

                    // right border
                    CheckForFence(
                        region,
                        x == input.Width - 1,
                        region.VFences,
                        y, x,
                        true,
                        () => regions[x + 1, y]
                    );

                    // Upper border
                    CheckForFence(
                        region,
                        y == 0,
                        region.HFences,
                        x, y,
                        false,
                        () => regions[x, y - 1]
                    );

                    // Bottom border
                    CheckForFence(
                        region, 
                        y == input.Height - 1,
                        region.HFences,
                        x, y,
                        true,
                        () => regions[x, y + 1]
                    );
                }
            }

            return regions.AsEnumerable().Distinct().Select(region => 
                (long)region.Area * (region.VFences.Count + region.HFences.Count)
            ).Sum();
        }

        private static bool CheckForFence(Region region, bool isEdge, List<Fence> fences, int pos, int alignment, bool side, Func<Region> getCheckRegion)
        {
            if (isEdge || (getCheckRegion() != region))
            {
                region.Perimeter++;
                Fence? fence = fences.Where(f => f.Position == alignment && f.Side == side && ((pos + 1) == f.Start || (pos - 1) == f.End)).FirstOrDefault();
                if (fence == null)
                {
                    fence = new Fence(alignment, pos, side);
                    fences.Add(fence);
                }
                else
                {
                    if ((pos + 1) == fence.Start)
                    {
                        fence.Start = pos;
                    }
                    else
                    {
                        fence.End = pos;
                    }
                }

                return true;
            }

            return false;
        }

        internal class Fence
        {
            // Either X or Y, depending on orientation
            public int Position;
            public int Start;
            public int End;
            public bool Side;

            public int Length => End - Start + 1;

            public Fence(int position, int start, bool side)
            {
                this.Position = position;
                this.Start = this.End = start;
                this.Side = side;
            }
        }
    }
}
