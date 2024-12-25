using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Utils
{

    public struct LargePoint : IEquatable<LargePoint>
    {
        public static readonly LargePoint Empty = new LargePoint(0, 0);

        public LargePoint(LargeSize sz)
        {
            this.X = sz.Width;
            this.Y = sz.Height;
        }

        public LargePoint(long x, long y)
        {
            this.X = x;
            this.Y = y;
        }

        public readonly bool IsEmpty => X == 0 && Y == 0;

        public long X { readonly get; set; }

        public long Y { readonly get; set; }

        public static LargePoint Add(LargePoint pt, LargeSize sz) => new LargePoint(pt.X + sz.Width, pt.Y + sz.Height);

        public static LargePoint Ceiling(LargePointF value) => new LargePoint((long)Math.Ceiling(value.X), (long)Math.Ceiling(value.Y));

        public static LargePoint Round(LargePointF value) => new LargePoint((long)Math.Round(value.X), (long)Math.Round(value.Y));
        
        public static LargePoint Subtract(LargePoint pt, LargeSize sz) => new LargePoint(pt.X - sz.Width, pt.Y - sz.Height);
       
        public static LargePoint Truncate(LargePointF value) => new LargePoint((long)Math.Truncate(value.X), (long)Math.Truncate(value.Y));
       
        public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is LargePoint && Equals((LargePoint)obj);
        
        public readonly bool Equals(LargePoint other) => this == other;
        
        public readonly override int GetHashCode() => HashCode.Combine(this.X.GetHashCode(), this.Y.GetHashCode());

        public void Offset(LargePoint p) {
            this.X += p.X;
            this.Y += p.Y;
        }
        
        public void Offset(long dx, long dy)
        {
            this.X += dx;
            this.Y += dy;
        }
        
        public readonly override string ToString() => $"{{X={X}, Y={Y}}}";

        public static LargePoint operator +(LargePoint pt, LargeSize sz) => Add(pt, sz);
       
        public static LargePoint operator -(LargePoint pt, LargeSize sz) => Subtract(pt, sz);
        
        public static bool operator ==(LargePoint left, LargePoint right) => Equals(left, right);
      
        public static bool operator !=(LargePoint left, LargePoint right) => !Equals(left, right);

        public static implicit operator LargePointF(LargePoint p) => new LargePointF(p.X, p.Y);

        public static explicit operator LargeSize(LargePoint p) => new LargeSize(p.X, p.Y);
    }
}
