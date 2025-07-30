namespace AdventOfCode._2017;

public class Day10
{
    string _input;

    public Day10()
    {
        _input = File.ReadAllText("Inputs/Day10.txt");
    }

    [Fact]
    public void Part1()
    {
        int[] lengths = _input.Split(',').Select(int.Parse).ToArray();

        KnotHash hash = new KnotHash(256);
        foreach (int length in lengths)
            hash.Apply(length);

        int answer = hash.Enumerate().Take(2).Aggregate((x, y) => x * y);
        Assert.Equal(23715, answer);
    }

    [Fact]
    public void Part2()
    {
        string answer = KnotHash.HashString(_input);
        Assert.Equal("541dc3180fd4b72881e39cf925a50253", answer);
    }
}
