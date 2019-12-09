using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AdventOfCode.Common
{
    [DebuggerDisplay("({X}, {Y}, {Z})")]
    public struct IntPoint3
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public int Manhattan => Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);

        public IntPoint3(int x , int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public int Distance(in IntPoint3 other) =>
            Math.Abs(X - other.X) + Math.Abs(Y - other.Y) + Math.Abs(Z - other.Z);

        public static IntPoint3 Zero => new IntPoint3(0, 0, 0);
        public static IntPoint3 UnitX => new IntPoint3(1, 0, 0);
        public static IntPoint3 UnitY => new IntPoint3(0, 1, 0);
        public static IntPoint3 UnitZ => new IntPoint3(0, 0, 1);

        public static IntPoint3 operator +(in IntPoint3 point) => point;
        public static IntPoint3 operator -(in IntPoint3 point) => new IntPoint3(-point.X, -point.Y, -point.Z);
        public static IntPoint3 operator +(in IntPoint3 a, in IntPoint3 b) =>
            new IntPoint3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static IntPoint3 operator -(in IntPoint3 a, in IntPoint3 b) =>
            new IntPoint3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static IntPoint3 operator +(in IntPoint3 a, int b) =>
            new IntPoint3(a.X + b, a.Y + b, a.Z + b);
        public static IntPoint3 operator -(in IntPoint3 a, int b) =>
            new IntPoint3(a.X - b, a.Y - b, a.Z - b);
        public static IntPoint3 operator *(in IntPoint3 a, int b) =>
            new IntPoint3(a.X * b, a.Y * b, a.Z * b);
        public static IntPoint3 operator /(in IntPoint3 a, int b) =>
            new IntPoint3(a.X / b, a.Y / b, a.Z / b);


        public static implicit operator IntPoint3(in (int x, int y, int z) point) =>
            new IntPoint3(point.x, point.y, point.z);

        public static implicit operator (int, int, int)(in IntPoint3 point) =>
            (point.X, point.Y, point.Z);

    }
}
