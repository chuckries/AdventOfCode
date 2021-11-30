using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace AdventOfCode.Common
{
    [DebuggerDisplay("({X}, {Y})")]
    public struct IntVec2 : IEquatable<IntVec2>
    {
        public readonly int X;
        public readonly int Y;

        public int Distance => Math.Abs(X) + Math.Abs(Y);

        public IntVec2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public IntVec2(string x, string y)
        {
            X = int.Parse(x);
            Y = int.Parse(y);
        }

        public int DistanceFrom(in IntVec2 p)
        {
            return Math.Abs(X - p.X) + Math.Abs(Y - p.Y);
        }

        public IEnumerable<IntVec2> Adjacent()
        {
            yield return this + UnitX;
            yield return this + UnitY;
            yield return this - UnitX;
            yield return this - UnitY;
        }

        public IEnumerable<IntVec2> Adjacent(IntVec2 bounds)
        {
            if (X - 1 >= 0) yield return this - UnitX;
            if (X + 1 < bounds.X) yield return this + UnitX;
            if (Y - 1 >= 0) yield return this - UnitY;
            if (Y + 1 < bounds.Y) yield return this + UnitY;
        }

        public IEnumerable<IntVec2> Surrounding()
        {
            yield return (X - 1, Y - 1);
            yield return (X    , Y - 1);
            yield return (X + 1, Y - 1);
            yield return (X - 1, Y    );
            yield return (X + 1, Y    );
            yield return (X - 1, Y + 1);
            yield return (X    , Y + 1);
            yield return (X + 1, Y + 1);
        }

        public IntVec2 RotateRight() => new IntVec2(Y * 1, X * -1);
        public IntVec2 RotateLeft() => new IntVec2(Y * -1, X * 1);

        public IntVec2 RotateRight(int count)
        {
            int sign = Math.Sign(count);
            return (((count % 4) + 4) % 4) switch
            {
                0 => new IntVec2(X, Y),
                1 => new IntVec2(Y * sign, X * -sign),
                2 => new IntVec2(-X, -Y),
                3 => new IntVec2(Y * -sign, X * sign),
                _ => throw new InvalidOperationException()
            };
        }

        public IntVec2 RotateLeft(int count)
        {
            int sign = Math.Sign(count);
            return (((count % 4) + 4) % 4) switch
            {
                0 => new IntVec2(X, Y),
                1 => new IntVec2(Y * -sign, X * sign),
                2 => new IntVec2(-X, -Y),
                3 => new IntVec2(Y * sign, X * -sign),
                _ => throw new InvalidOperationException()
            };
        }

        public IntVec2 Transform(Func<int, int> transform)
        {
            return new IntVec2(transform(X), transform(Y));
        }

        public int ToIndex(IntVec2 max)
        {
            return Y * max.X + X;
        }

        public static IntVec2 FromIndex(int index, IntVec2 max)
        {
            return new IntVec2(index % max.X, index / max.X);
        }

        public int ToIndex(IntVec2 min, IntVec2 max)
        {
            int width = max.X - min.X + 1;
            return (Y - min.Y) * width + (X - min.X);
        }

        public static IntVec2 FromIndex(int index, IntVec2 min, IntVec2 max)
        {
            int width = max.X - min.X + 1;
            return new IntVec2(index % width, index / width);
        }

        public void Deconstruct(out int x, out int y) => (x, y) = (X, Y);

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        public override bool Equals(object obj)
        {
            return obj is IntVec2 point && Equals(point);
        }

        public bool Equals(IntVec2 other)
        {
            return X == other.X &&
                   Y == other.Y;
        }

        public static bool operator ==(IntVec2 a, IntVec2 b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(IntVec2 a, IntVec2 b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public static IntVec2 Min(IEnumerable<IntVec2> points)
        {
            int minX, minY;
            minX = minY = int.MaxValue;
            foreach (IntVec2 point in points)
            {
                if (point.X < minX) minX = point.X;
                if (point.Y < minY) minY = point.Y;
            }

            return new IntVec2(minX, minY);
        }

        public static IntVec2 Max(IEnumerable<IntVec2> points)
        {
            int maxX, maxY;
            maxX = maxY = int.MinValue;
            foreach (IntVec2 point in points)
            {
                if (point.X > maxX) maxX = point.X;
                if (point.Y > maxY) maxY = point.Y;
            }

            return new IntVec2(maxX, maxY);
        }

        public static (IntVec2 min, IntVec2 max) MinMax(IEnumerable<IntVec2> points)
        {
            int minX, minY, maxX, maxY;
            minX = minY = int.MaxValue;
            maxX = maxY = int.MinValue;
            foreach (IntVec2 point in points)
            {
                if (point.X < minX) minX = point.X;
                if (point.X > maxX) maxX = point.X;
                if (point.Y < minY) minY = point.Y;
                if (point.Y > maxY) maxY = point.Y;
            }

            return (new IntVec2(minX, minY), new IntVec2(maxX, maxY));
        }

        public static IntVec2 Zero => new IntVec2(0, 0);

        public static IntVec2 UnitX => new IntVec2(1, 0);
        public static IntVec2 UnitY => new IntVec2(0, 1);

        public static IntVec2 operator +(IntVec2 a) => a;
        public static IntVec2 operator -(IntVec2 a) => new IntVec2(-a.X, -a.Y);
        public static IntVec2 operator +(IntVec2 a, IntVec2 b) => new IntVec2(a.X + b.X, a.Y + b.Y);
        public static IntVec2 operator -(IntVec2 a, IntVec2 b) => new IntVec2(a.X - b.X, a.Y - b.Y);
        public static IntVec2 operator *(IntVec2 a, int b) => new IntVec2(a.X * b, a.Y * b);
        public static IntVec2 operator /(IntVec2 a, int b) => new IntVec2(a.X / b, a.Y / b);


        public static implicit operator (int, int)(IntVec2 pair) => (pair.X, pair.Y);
        public static implicit operator IntVec2(int i) => new IntVec2(i, i);
        public static implicit operator IntVec2((int x, int y) pair) => new IntVec2(pair.x, pair.y);
        public static implicit operator IntVec2((string x, string y) pair) => new IntVec2(pair.x, pair.y);
    }
}
