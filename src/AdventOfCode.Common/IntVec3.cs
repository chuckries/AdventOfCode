using System.Diagnostics;

namespace AdventOfCode.Common;

[DebuggerDisplay("({X}, {Y}, {Z})")]
public struct IntVec3 : IEquatable<IntVec3>
{
    public readonly int X;
    public readonly int Y;
    public readonly int Z;

    public int Manhattan => Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);

    public IntVec3(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public IntVec3(string x, string y, string z)
    {
        X = int.Parse(x);
        Y = int.Parse(y);
        Z = int.Parse(z);
    }

    public IEnumerable<IntVec3> Surrounding()
    {
        yield return new IntVec3(X - 1, Y - 1, Z - 1);
        yield return new IntVec3(X - 1, Y - 1, Z);
        yield return new IntVec3(X - 1, Y - 1, Z + 1);
        yield return new IntVec3(X - 1, Y, Z - 1);
        yield return new IntVec3(X - 1, Y, Z);
        yield return new IntVec3(X - 1, Y, Z + 1);
        yield return new IntVec3(X - 1, Y + 1, Z - 1);
        yield return new IntVec3(X - 1, Y + 1, Z);
        yield return new IntVec3(X - 1, Y + 1, Z + 1);

        yield return new IntVec3(X, Y - 1, Z - 1);
        yield return new IntVec3(X, Y - 1, Z);
        yield return new IntVec3(X, Y - 1, Z + 1);
        yield return new IntVec3(X, Y, Z - 1);

        yield return new IntVec3(X, Y, Z + 1);
        yield return new IntVec3(X, Y + 1, Z - 1);
        yield return new IntVec3(X, Y + 1, Z);
        yield return new IntVec3(X, Y + 1, Z + 1);

        yield return new IntVec3(X + 1, Y - 1, Z - 1);
        yield return new IntVec3(X + 1, Y - 1, Z);
        yield return new IntVec3(X + 1, Y - 1, Z + 1);
        yield return new IntVec3(X + 1, Y, Z - 1);
        yield return new IntVec3(X + 1, Y, Z);
        yield return new IntVec3(X + 1, Y, Z + 1);
        yield return new IntVec3(X + 1, Y + 1, Z - 1);
        yield return new IntVec3(X + 1, Y + 1, Z);
        yield return new IntVec3(X + 1, Y + 1, Z + 1);
    }

    public int Distance(in IntVec3 other) =>
        Math.Abs(X - other.X) + Math.Abs(Y - other.Y) + Math.Abs(Z - other.Z);

    public void Destructure(out int x, out int y, out int z) =>
        (x, y, z) = (X, Y, Z);

    public override string ToString()
    {
        return $"({X}, {Y}, {Z})";
    }

    public override bool Equals(object? obj)
    {
        return obj is IntVec3 point && Equals(point);
    }

    public bool Equals(IntVec3 other)
    {
        return X == other.X &&
               Y == other.Y &&
               Z == other.Z;
    }

    public static bool operator ==(IntVec3 a, IntVec3 b) =>
        a.X == b.X && a.Y == b.Y && a.Z == b.Z;

    public static bool operator !=(IntVec3 a, IntVec3 b) =>
        !(a == b);

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z);
    }

    public static (IntVec3 min, IntVec3 max) MinMax(IEnumerable<IntVec3> points)
    {
        int minX, minY, minZ, maxX, maxY, maxZ;
        minX = minY = minZ = int.MaxValue;
        maxX = maxY = maxZ = int.MinValue;

        foreach (IntVec3 point in points)
        {
            if (point.X < minX) minX = point.X;
            if (point.X > maxX) maxX = point.X;
            if (point.Y < minY) minY = point.Y;
            if (point.Y > maxY) maxY = point.Y;
            if (point.Z < minZ) minZ = point.Z;
            if (point.Z > maxZ) maxZ = point.Z;
        }

        return (new IntVec3(minX, minY, minZ), new IntVec3(maxX, maxY, maxZ));
    }

    public static IntVec3 Zero => new IntVec3(0, 0, 0);
    public static IntVec3 UnitX => new IntVec3(1, 0, 0);
    public static IntVec3 UnitY => new IntVec3(0, 1, 0);
    public static IntVec3 UnitZ => new IntVec3(0, 0, 1);

    public static IntVec3 operator +(in IntVec3 point) => point;
    public static IntVec3 operator -(in IntVec3 point) => new IntVec3(-point.X, -point.Y, -point.Z);
    public static IntVec3 operator +(in IntVec3 a, in IntVec3 b) =>
        new IntVec3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    public static IntVec3 operator -(in IntVec3 a, in IntVec3 b) =>
        new IntVec3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    public static IntVec3 operator +(in IntVec3 a, int b) =>
        new IntVec3(a.X + b, a.Y + b, a.Z + b);
    public static IntVec3 operator -(in IntVec3 a, int b) =>
        new IntVec3(a.X - b, a.Y - b, a.Z - b);
    public static IntVec3 operator *(in IntVec3 a, int b) =>
        new IntVec3(a.X * b, a.Y * b, a.Z * b);
    public static IntVec3 operator /(in IntVec3 a, int b) =>
        new IntVec3(a.X / b, a.Y / b, a.Z / b);


    public static implicit operator IntVec3(in (int x, int y, int z) point) =>
        new IntVec3(point.x, point.y, point.z);

    public static implicit operator IntVec3(in (string x, string y, string z) point) =>
        new IntVec3(point.x, point.y, point.z);

    public static implicit operator (int, int, int)(in IntVec3 point) =>
        (point.X, point.Y, point.Z);

}
