using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode.Common
{
    public struct IntPair
    {
        public int X { get; set; }
        public int Y { get; set; }

        /// <summary>
        /// Returns the Manhattan Distance represented by this pair as a coordinate pair
        /// </summary>
        public int Manhattan => Math.Abs(X) + Math.Abs(Y);

        public IntPair(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static IntPair Up => new IntPair(0, 1);
        public static IntPair Down => new IntPair(0, -1);
        public static IntPair Left => new IntPair(-1, 0);
        public static IntPair Right => new IntPair(1, 0);

        public static IntPair operator +(IntPair a) => a;
        public static IntPair operator -(IntPair a) => new IntPair(-a.X, -a.Y);
        public static IntPair operator +(IntPair a, IntPair b) => new IntPair(a.X + b.X, a.Y + b.Y);
        public static IntPair operator -(IntPair a, IntPair b) => new IntPair(a.X - b.X, a.Y - b.Y);
        public static IntPair operator *(IntPair a, int b) => new IntPair(a.X * b, a.Y * b);
        public static IntPair operator /(IntPair a, int b) => new IntPair(a.X / b, a.Y / b);


        public static implicit operator (int, int)(IntPair pair) => (pair.X, pair.Y);
        public static implicit operator IntPair((int x, int y) pair) => new IntPair(pair.x, pair.y);
    }
}
