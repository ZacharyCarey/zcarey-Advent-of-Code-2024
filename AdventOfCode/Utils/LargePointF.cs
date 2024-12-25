using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Utils
{
    public struct LargePointF : IEquatable<LargePointF>
    {
        public static readonly LargePointF Empty = new LargePointF(0.0, 0.0);
        private double x; 
        private double y;

        public LargePointF(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public readonly bool IsEmpty => x == 0.0 && y == 0.0;

        public double X
        {
            readonly get => x;
            set => x = value;
        }

        public double Y
        {
            readonly get => y;
            set => y = value;
        }

        public static LargePointF operator +(LargePointF pt, LargeSize sz) => Add(pt, sz);

        public static LargePointF operator -(LargePointF pt, LargeSize sz) => Subtract(pt, sz);

        public static LargePointF operator +(LargePointF pt, LargeSizeF sz) => Add(pt, sz);

        public static LargePointF operator -(LargePointF pt, LargeSizeF sz) => Subtract(pt, sz);

        public static bool operator ==(LargePointF left, LargePointF right) => left.X == right.X && left.Y == right.Y;

        public static bool operator !=(LargePointF left, LargePointF right) => !(left == right);

        public static LargePointF Add(LargePointF pt, LargeSize sz) => new LargePointF(pt.X + sz.Width, pt.Y + sz.Height);

        public static LargePointF Subtract(LargePointF pt, LargeSize sz) => new LargePointF(pt.X - sz.Width, pt.Y - sz.Height);

        public static LargePointF Add(LargePointF pt, LargeSizeF sz) => new LargePointF(pt.X + sz.Width, pt.Y + sz.Height);

        public static LargePointF Subtract(LargePointF pt, LargeSizeF sz) => new LargePointF(pt.X - sz.Width, pt.Y - sz.Height);

        public override readonly bool Equals([NotNullWhen(true)] object? obj) => obj is LargePointF && Equals((LargePointF)obj);

        public readonly bool Equals(LargePointF other) => this == other;

        public override readonly int GetHashCode() => HashCode.Combine(X.GetHashCode(), Y.GetHashCode());

        public override readonly string ToString() => $"{{X={x}, Y={y}}}";
    }
}
