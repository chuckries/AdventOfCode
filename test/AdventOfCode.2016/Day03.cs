using System.Text.RegularExpressions;

namespace AdventOfCode._2016;

public class Day03
{
    IntVec3[] _input;

    public Day03()
    {
        _input = s_Regex
            .Matches(File.ReadAllText("Inputs/Day03.txt"))
            .Select(m => new IntVec3(
                m.Groups["x"].Value,
                m.Groups["y"].Value,
                m.Groups["z"].Value))
            .ToArray();
    }

    [Fact]
    public void Part1()
    {
        int answer = _input.Where(IsValidTriangle).Count();
        Assert.Equal(982, answer);
    }

    [Fact]
    public void Part2()
    {
        int count = 0;
        for (int i = 0; i < _input.Length; i += 3)
        {
            IntVec3 r0 = _input[i];
            IntVec3 r1 = _input[i + 1];
            IntVec3 r2 = _input[i + 2];

            if (IsValidTriangle((r0.X, r1.X, r2.X))) count++;
            if (IsValidTriangle((r0.Y, r1.Y, r2.Y))) count++;
            if (IsValidTriangle((r0.Z, r1.Z, r2.Z))) count++;
        }

        Assert.Equal(1826, count);
    }

    private static bool IsValidTriangle(IntVec3 tri) =>
        tri.X + tri.Y > tri.Z &&
        tri.X + tri.Z > tri.Y &&
        tri.Y + tri.Z > tri.X;

    private static Regex s_Regex = new Regex(
        @"^\s*(?'x'\d+)\s+(?'y'\d+)\s+(?'z'\d+)\s*$",
        RegexOptions.Compiled | RegexOptions.Multiline);
}
