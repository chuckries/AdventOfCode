namespace AdventOfCode._2022;

public class Day03
{
    List<string> _input;

    public Day03()
    {
        _input = File.ReadAllLines("Inputs/Day03.txt").ToList();
    }

    [Fact]
    public void Part1()
    {
        int answer = _input
            .Select(s => (a: s[..(s.Length / 2)], b: s[(s.Length / 2)..]))
            .Select(pair => pair.a.Intersect(pair.b).Single()).Sum(Score);
        Assert.Equal(7793, answer);
    }

    [Fact]
    public void Part2()
    {
        int answer = _input.Chunk(3)
            .Select(strs => strs[1..]
                .Aggregate(new HashSet<char>(strs[0]), (acc, s) =>
                {
                    acc.IntersectWith(s);
                    return acc;
                })
                .Single())
            .Sum(Score);

        Assert.Equal(2499, answer);
    }

    private static int Score(char c) => c switch
    {
        >= 'a' and <= 'z' => c - 'a' + 1,
        >= 'A' and <= 'Z' => c - 'A' + 27,
        _ => throw new InvalidOperationException()
    };
}
