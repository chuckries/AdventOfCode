using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace AdventOfCode.Common
{
    [DebuggerDisplay("({X}, {Y})")]
    public struct IntPoint2 : IEquatable<IntPoint2>
    {
        public readonly int X;
        public readonly int Y;

        /// <summary>
        /// Returns the Manhattan Distance represented by this pair as a coordinate pair
        /// </summary>
        public int Manhattan => Math.Abs(X) + Math.Abs(Y);

        public IntPoint2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int Distance(in IntPoint2 p)
        {
            return Math.Abs(X - p.X) + Math.Abs(Y - p.Y);
        }

        public IEnumerable<IntPoint2> Adjacent()
        {
            yield return this + UnitX;
            yield return this + UnitY;
            yield return this - UnitX;
            yield return this - UnitY;
        }

        public IEnumerable<IntPoint2> Adjacent(IntPoint2 bounds)
        {
            if (X - 1 >= 0) yield return this - UnitX;
            if (X + 1 < bounds.X) yield return this + UnitX;
            if (Y - 1 >= 0) yield return this - UnitY;
            if (Y + 1 < bounds.Y) yield return this + UnitY;
        }

        public IntPoint2 TurnRight() => new IntPoint2(Y * 1, X * -1);
        public IntPoint2 TurnLeft() => new IntPoint2(Y * -1, X * 1);

        public IntPoint2 Transform(Func<int, int> transform)
        {
            return new IntPoint2(transform(X), transform(Y));
        }

        public int ToIndex(IntPoint2 max)
        {
            return Y * max.X + X;
        }

        public static IntPoint2 FromIndex(int index, IntPoint2 max)
        {
            return new IntPoint2(index % max.X, index / max.X);
        }

        public int ToIndex(IntPoint2 min, IntPoint2 max)
        {
            int width = max.X - min.X + 1;
            return (Y - min.Y) * width + (X - min.X);
        }

        public static IntPoint2 FromIndex(int index, IntPoint2 min, IntPoint2 max)
        {
            int width = max.X - min.X + 1;
            return new IntPoint2(index % width, index / width);
        }

        public void Deconstruct(out int x, out int y) => (x, y) = (X, Y);

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        public override bool Equals(object obj)
        {
            return obj is IntPoint2 point && Equals(point);
        }

        public bool Equals(IntPoint2 other)
        {
            return X == other.X &&
                   Y == other.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public static IntPoint2 Min(IEnumerable<IntPoint2> points)
        {
            int minX, minY;
            minX = minY = int.MaxValue;
            foreach (IntPoint2 point in points)
            {
                if (point.X < minX) minX = point.X;
                if (point.Y < minY) minY = point.Y;
            }

            return new IntPoint2(minX, minY);
        }

        public static IntPoint2 Max(IEnumerable<IntPoint2> points)
        {
            int maxX, maxY;
            maxX = maxY = int.MinValue;
            foreach (IntPoint2 point in points)
            {
                if (point.X > maxX) maxX = point.X;
                if (point.Y > maxY) maxY = point.Y;
            }

            return new IntPoint2(maxX, maxY);
        }

        public static (IntPoint2 min, IntPoint2 max) MinMax(IEnumerable<IntPoint2> points)
        {
            int minX, minY, maxX, maxY;
            minX = minY = int.MaxValue;
            maxX = maxY = int.MinValue;
            foreach (IntPoint2 point in points)
            {
                if (point.X < minX) minX = point.X;
                if (point.X > maxX) maxX = point.X;
                if (point.Y < minY) minY = point.Y;
                if (point.Y > maxY) maxY = point.Y;
            }

            return (new IntPoint2(minX, minY), new IntPoint2(maxX, maxY));
        }

        public static IntPoint2 Zero => new IntPoint2(0, 0);

        public static IntPoint2 UnitX => new IntPoint2(1, 0);
        public static IntPoint2 UnitY => new IntPoint2(0, 1);

        public static IntPoint2 operator +(IntPoint2 a) => a;
        public static IntPoint2 operator -(IntPoint2 a) => new IntPoint2(-a.X, -a.Y);
        public static IntPoint2 operator +(IntPoint2 a, IntPoint2 b) => new IntPoint2(a.X + b.X, a.Y + b.Y);
        public static IntPoint2 operator -(IntPoint2 a, IntPoint2 b) => new IntPoint2(a.X - b.X, a.Y - b.Y);
        public static IntPoint2 operator *(IntPoint2 a, int b) => new IntPoint2(a.X * b, a.Y * b);
        public static IntPoint2 operator /(IntPoint2 a, int b) => new IntPoint2(a.X / b, a.Y / b);


        public static implicit operator (int, int)(IntPoint2 pair) => (pair.X, pair.Y);
        public static implicit operator IntPoint2((int x, int y) pair) => new IntPoint2(pair.x, pair.y);
    }
}
