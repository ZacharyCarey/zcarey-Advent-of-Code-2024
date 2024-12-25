using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.WebSockets;
using System.Numerics;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Utils
{
    public struct LargeSizeF : IEquatable<LargeSizeF>
    {
        public static readonly LargeSizeF Empty = new LargeSizeF(0, 0);

        public LargeSizeF(LargePointF pt)
        {
            this.Width = pt.X;
            this.Height = pt.Y;
        }

        public LargeSizeF(LargeSizeF size)
        {
            this.Width = size.Width;
            this.Height = size.Height;
        }
       
        public LargeSizeF(double width, double height)
        {
            this.Width = width;
            this.Height = height;
        }

        public double Height { readonly get; set; }

        public readonly bool IsEmpty { get => this.Width == 0.0 && this.Height == 0.0; }

        public double Width { readonly get; set; }

        public static LargeSizeF Add(LargeSizeF sz1, LargeSizeF sz2) => new LargeSizeF(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
       
        public static LargeSizeF Subtract(LargeSizeF sz1, LargeSizeF sz2) => new LargeSizeF(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
       
        public readonly bool Equals(LargeSizeF other) => this == other;

        public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is LargeSizeF && Equals((LargeSizeF)obj);

        public readonly override int GetHashCode() => HashCode.Combine(this.Width.GetHashCode() + this.Height.GetHashCode());

        public readonly LargePointF ToPointF() => new LargePointF(this.Width, this.Height);

        public readonly LargeSize ToSize() => new LargeSize((long)this.Width, (long)this.Height);
       
        public readonly override string ToString() => $"{{W={this.Width}, H={this.Height}}}";
       
        public static LargeSizeF operator +(LargeSizeF sz1, LargeSizeF sz2) => Add(sz1, sz2);
      
        public static LargeSizeF operator -(LargeSizeF sz1, LargeSizeF sz2) => Subtract(sz1, sz2);

        public static LargeSizeF operator *(double left, LargeSizeF right) => new LargeSizeF(right.Width * left, right.Height * left);

        public static LargeSizeF operator *(LargeSizeF left, double right) => new LargeSizeF(left.Width * right, left.Height * right);

        public static LargeSizeF operator /(LargeSizeF left, double right) => new LargeSizeF(left.Width / right, left.Height / right);

        public static bool operator ==(LargeSizeF sz1, LargeSizeF sz2) => sz1.Width == sz2.Width && sz1.Height == sz2.Height;

        public static bool operator !=(LargeSizeF sz1, LargeSizeF sz2) => !(sz1 == sz2);

        public static explicit operator LargePointF(LargeSizeF size) => new LargePointF(size.Width, size.Height);
    }
}
