using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AdventOfCode.Common
{
    [DebuggerDisplay("({X}, {Y})")]
    public struct IntPoint2
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

        public IntPoint2 Transform(Func<int, int> transform)
        {
            return new IntPoint2(transform(X), transform(Y));
        }

        public void Deconstruct(out int x, out int y) => (x, y) = (X, Y);

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        public IntPoint2 TurnRight() => new IntPoint2(Y * 1, X * -1);
        public IntPoint2 TurnLeft() => new IntPoint2(Y * -1, X * 1);

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
