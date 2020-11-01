using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AdventOfCode.Common
{
    [DebuggerDisplay("({X}, {Y}, {Z})")]
    public struct IntPoint3 : IEquatable<IntPoint3>
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Z;

        public int Manhattan => Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);

        public IntPoint3(int x , int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public IntPoint3(string x, string y, string z)
        {
            X = int.Parse(x);
            Y = int.Parse(y);
            Z = int.Parse(z);
        }

        public int Distance(in IntPoint3 other) =>
            Math.Abs(X - other.X) + Math.Abs(Y - other.Y) + Math.Abs(Z - other.Z);

        public void Destructure(out int x, out int y, out int z) =>
            (x, y, z) = (X, Y, Z);

        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }

        public override bool Equals(object obj)
        {
            return obj is IntPoint3 point && Equals(point);
        }

        public bool Equals(IntPoint3 other)
        {
            return X == other.X &&
                   Y == other.Y &&
                   Z == other.Z;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        public static (IntPoint3 min, IntPoint3 max) MinMax(IEnumerable<IntPoint3> points)
        {
            int minX, minY, minZ, maxX, maxY, maxZ;
            minX = minY = minZ = int.MaxValue;
            maxX = maxY = maxZ = int.MinValue;

            foreach(IntPoint3 point in points)
            {
                if (point.X < minX) minX = point.X;
                if (point.X > maxX) maxX = point.X;
                if (point.Y < minY) minY = point.Y;
                if (point.Y > maxY) maxY = point.Y;
                if (point.Z < minZ) minZ = point.Z;
                if (point.Z > maxZ) maxZ = point.Z;
            }

            return (new IntPoint3(minX, minY, minZ), new IntPoint3(maxX, maxY, maxZ));
        }

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

        public static implicit operator IntPoint3(in (string x, string y, string z) point) =>
            new IntPoint3(point.x, point.y, point.z);

        public static implicit operator (int, int, int)(in IntPoint3 point) =>
            (point.X, point.Y, point.Z);

    }
}
