using System.Diagnostics;

namespace AdventOfCode.Common;

[DebuggerDisplay("({X}, {Y}, {Z})")]
public struct IntVec4 : IEquatable<IntVec4>
{
    public readonly int X;
    public readonly int Y;
    public readonly int Z;
    public readonly int W;

    public int Manhattan => Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z) + Math.Abs(W);

    public IntVec4(int x, int y, int z, int w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    public IntVec4(string x, string y, string z, string w)
    {
        X = int.Parse(x);
        Y = int.Parse(y);
        Z = int.Parse(z);
        W = int.Parse(w);
    }

    public IEnumerable<IntVec4> Surrounding()
    {
        yield return new IntVec4(X - 1, Y - 1, Z - 1, W - 1);
        yield return new IntVec4(X - 1, Y - 1, Z - 1, W);
        yield return new IntVec4(X - 1, Y - 1, Z - 1, W + 1);
        yield return new IntVec4(X - 1, Y - 1, Z, W - 1);
        yield return new IntVec4(X - 1, Y - 1, Z, W);
        yield return new IntVec4(X - 1, Y - 1, Z, W + 1);
        yield return new IntVec4(X - 1, Y - 1, Z + 1, W - 1);
        yield return new IntVec4(X - 1, Y - 1, Z + 1, W);
        yield return new IntVec4(X - 1, Y - 1, Z + 1, W + 1);

        yield return new IntVec4(X - 1, Y, Z - 1, W - 1);
        yield return new IntVec4(X - 1, Y, Z - 1, W);
        yield return new IntVec4(X - 1, Y, Z - 1, W + 1);
        yield return new IntVec4(X - 1, Y, Z, W - 1);
        yield return new IntVec4(X - 1, Y, Z, W);
        yield return new IntVec4(X - 1, Y, Z, W + 1);
        yield return new IntVec4(X - 1, Y, Z + 1, W - 1);
        yield return new IntVec4(X - 1, Y, Z + 1, W);
        yield return new IntVec4(X - 1, Y, Z + 1, W + 1);

        yield return new IntVec4(X - 1, Y + 1, Z - 1, W - 1);
        yield return new IntVec4(X - 1, Y + 1, Z - 1, W);
        yield return new IntVec4(X - 1, Y + 1, Z - 1, W + 1);
        yield return new IntVec4(X - 1, Y + 1, Z, W - 1);
        yield return new IntVec4(X - 1, Y + 1, Z, W);
        yield return new IntVec4(X - 1, Y + 1, Z, W + 1);
        yield return new IntVec4(X - 1, Y + 1, Z + 1, W - 1);
        yield return new IntVec4(X - 1, Y + 1, Z + 1, W);
        yield return new IntVec4(X - 1, Y + 1, Z + 1, W + 1);

        yield return new IntVec4(X, Y - 1, Z - 1, W - 1);
        yield return new IntVec4(X, Y - 1, Z - 1, W);
        yield return new IntVec4(X, Y - 1, Z - 1, W + 1);
        yield return new IntVec4(X, Y - 1, Z, W - 1);
        yield return new IntVec4(X, Y - 1, Z, W);
        yield return new IntVec4(X, Y - 1, Z, W + 1);
        yield return new IntVec4(X, Y - 1, Z + 1, W - 1);
        yield return new IntVec4(X, Y - 1, Z + 1, W);
        yield return new IntVec4(X, Y - 1, Z + 1, W + 1);

        yield return new IntVec4(X, Y, Z - 1, W - 1);
        yield return new IntVec4(X, Y, Z - 1, W);
        yield return new IntVec4(X, Y, Z - 1, W + 1);
        yield return new IntVec4(X, Y, Z, W - 1);

        yield return new IntVec4(X, Y, Z, W + 1);
        yield return new IntVec4(X, Y, Z + 1, W - 1);
        yield return new IntVec4(X, Y, Z + 1, W);
        yield return new IntVec4(X, Y, Z + 1, W + 1);

        yield return new IntVec4(X, Y + 1, Z - 1, W - 1);
        yield return new IntVec4(X, Y + 1, Z - 1, W);
        yield return new IntVec4(X, Y + 1, Z - 1, W + 1);
        yield return new IntVec4(X, Y + 1, Z, W - 1);
        yield return new IntVec4(X, Y + 1, Z, W);
        yield return new IntVec4(X, Y + 1, Z, W + 1);
        yield return new IntVec4(X, Y + 1, Z + 1, W - 1);
        yield return new IntVec4(X, Y + 1, Z + 1, W);
        yield return new IntVec4(X, Y + 1, Z + 1, W + 1);

        yield return new IntVec4(X + 1, Y - 1, Z - 1, W - 1);
        yield return new IntVec4(X + 1, Y - 1, Z - 1, W);
        yield return new IntVec4(X + 1, Y - 1, Z - 1, W + 1);
        yield return new IntVec4(X + 1, Y - 1, Z, W - 1);
        yield return new IntVec4(X + 1, Y - 1, Z, W);
        yield return new IntVec4(X + 1, Y - 1, Z, W + 1);
        yield return new IntVec4(X + 1, Y - 1, Z + 1, W - 1);
        yield return new IntVec4(X + 1, Y - 1, Z + 1, W);
        yield return new IntVec4(X + 1, Y - 1, Z + 1, W + 1);

        yield return new IntVec4(X + 1, Y, Z - 1, W - 1);
        yield return new IntVec4(X + 1, Y, Z - 1, W);
        yield return new IntVec4(X + 1, Y, Z - 1, W + 1);
        yield return new IntVec4(X + 1, Y, Z, W - 1);
        yield return new IntVec4(X + 1, Y, Z, W);
        yield return new IntVec4(X + 1, Y, Z, W + 1);
        yield return new IntVec4(X + 1, Y, Z + 1, W - 1);
        yield return new IntVec4(X + 1, Y, Z + 1, W);
        yield return new IntVec4(X + 1, Y, Z + 1, W + 1);

        yield return new IntVec4(X + 1, Y + 1, Z - 1, W - 1);
        yield return new IntVec4(X + 1, Y + 1, Z - 1, W);
        yield return new IntVec4(X + 1, Y + 1, Z - 1, W + 1);
        yield return new IntVec4(X + 1, Y + 1, Z, W - 1);
        yield return new IntVec4(X + 1, Y + 1, Z, W);
        yield return new IntVec4(X + 1, Y + 1, Z, W + 1);
        yield return new IntVec4(X + 1, Y + 1, Z + 1, W - 1);
        yield return new IntVec4(X + 1, Y + 1, Z + 1, W);
        yield return new IntVec4(X + 1, Y + 1, Z + 1, W + 1);
    }

    public void Destructure(out int x, out int y, out int z, out int w) =>
        (x, y, z, w) = (X, Y, Z, W);

    public override string ToString()
    {
        return $"({X}, {Y}, {Z}, {W})";
    }

    public override bool Equals(object obj)
    {
        return obj is IntVec4 point && Equals(point);
    }

    public bool Equals(IntVec4 other)
    {
        return X == other.X &&
               Y == other.Y &&
               Z == other.Z &&
               W == other.W;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z, W);
    }

    public static IntVec4 Zero => new IntVec4(0, 0, 0, 0);

    public static IntVec4 operator +(in IntVec4 point) => point;
    public static IntVec4 operator -(in IntVec4 point) => new IntVec4(-point.X, -point.Y, -point.Z, -point.W);
    public static IntVec4 operator +(in IntVec4 a, in IntVec4 b) =>
        new IntVec4(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
    public static IntVec4 operator -(in IntVec4 a, in IntVec4 b) =>
        new IntVec4(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
    public static IntVec4 operator +(in IntVec4 a, int b) =>
        new IntVec4(a.X + b, a.Y + b, a.Z + b, a.W + b);
    public static IntVec4 operator -(in IntVec4 a, int b) =>
        new IntVec4(a.X - b, a.Y - b, a.Z - b, a.W - b);
    public static IntVec4 operator *(in IntVec4 a, int b) =>
        new IntVec4(a.X * b, a.Y * b, a.Z * b, a.W * b);
    public static IntVec4 operator /(in IntVec4 a, int b) =>
        new IntVec4(a.X / b, a.Y / b, a.Z / b, a.W / b);


    public static implicit operator IntVec4(in (int x, int y, int z, int w) point) =>
        new IntVec4(point.x, point.y, point.z, point.w);

    public static implicit operator IntVec4(in (string x, string y, string z, string w) point) =>
        new IntVec4(point.x, point.y, point.z, point.w);

    public static implicit operator (int x, int y, int z, int w)(in IntVec4 point) =>
        (point.X, point.Y, point.Z, point.W);

}
