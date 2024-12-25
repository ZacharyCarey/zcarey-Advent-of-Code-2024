using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Utils
{
    public static class PointUtils
    {
        public static int ManhattanDistance(this Point left, Point right)
        {
            return Math.Abs(left.X - right.X) + Math.Abs(left.Y - right.Y);
        }

        public static long ManhattanDistance(this LargePoint left, LargePoint right)
        {
            return Math.Abs(left.X - right.X) + Math.Abs(left.Y - right.Y);
        }
    }
}
