using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Utils
{
    public struct LargeSize : IEquatable<LargeSize>
    {
        public static readonly LargeSize Empty = new LargeSize(0, 0);

        public LargeSize(LargePoint pt)
        {
            this.Width = pt.X;
            this.Height = pt.Y;
        }

        public LargeSize(long width, long height)
        {
            this.Width = width;
            this.Height = height;
        }

        public long Height { readonly get; set; }

        public readonly bool IsEmpty => this.Width == 0 && this.Height == 0;

        public long Width { readonly get; set; }

        public static LargeSize Add(LargeSize sz1, LargeSize sz2) => new LargeSize(sz1.Width + sz2.Width, sz1.Height + sz2.Height);

        public static LargeSize Ceiling(LargeSizeF value) => new LargeSize((long)Math.Ceiling(value.Width), (long)Math.Ceiling(value.Height));

        public static LargeSize Round(LargeSizeF value) => new LargeSize((long)Math.Round(value.Width), (long)Math.Round(value.Height));

        public static LargeSize Subtract(LargeSize sz1, LargeSize sz2) => new LargeSize(sz1.Width - sz2.Width, sz1.Height - sz2.Height);

        public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is LargeSize && Equals((LargeSize)obj);

        public readonly bool Equals(LargeSize other) => this == other;

        public readonly override int GetHashCode() => HashCode.Combine(this.Width.GetHashCode(), this.Height.GetHashCode());

        public readonly override string ToString() => $"{{W={Width}, H={Height}}}";

        public static LargeSize operator +(LargeSize sz1, LargeSize sz2) => new LargeSize(sz1.Width + sz2.Width, sz1.Height + sz2.Height);

        public static LargeSize operator -(LargeSize sz1, LargeSize sz2) => new LargeSize(sz1.Width - sz2.Width, sz1.Height - sz2.Height);

        public static LargeSizeF operator *(double left, LargeSize right) => new LargeSizeF(right.Width * left, right.Height * left);
        
        public static LargeSizeF operator *(LargeSize left, double right) => new LargeSizeF(left.Width * right, left.Height * right);
       
        public static LargeSize operator *(LargeSize left, long right) => new LargeSize(left.Width * right, left.Height * right);
        
        public static LargeSize operator *(long left, LargeSize right) => new LargeSize(right.Width * left, right.Height * left);
        
        public static LargeSizeF operator /(LargeSize left, double right) => new LargeSizeF(left.Width / right, left.Height / right);
       
        public static LargeSize operator /(LargeSize left, long right) => new LargeSize(left.Width / right, left.Height / right);
        
        public static bool operator ==(LargeSize sz1, LargeSize sz2) => sz1.Width == sz2.Width && sz1.Height == sz2.Height;
       
        public static bool operator !=(LargeSize sz1, LargeSize sz2) => !(sz1 == sz2);

        public static implicit operator LargeSizeF(LargeSize p) => new LargeSizeF(p.Width, p.Height);

        public static explicit operator LargePoint(LargeSize size) => new LargePoint(size.Width, size.Height);
    }
}
