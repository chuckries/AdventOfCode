namespace AdventOfCode._2020;

public class Day15
{
    int[] _input;

    public Day15()
    {
        _input = File.ReadAllText("Inputs/Day15.txt")
            .Split(',')
            .Select(int.Parse)
            .ToArray();
    }

    [Fact]
    public void Part1()
    {
        int answer = GetSequence(2020).Last();
        Assert.Equal(403, answer);
    }

    [Fact]
    public void Part2()
    {
        int answer = GetSequence(30_000_000).Last();
        Assert.Equal(6823, answer);
    }

    private IEnumerable<int> GetSequence(int n)
    {
        int previous = _input[0];
        yield return previous;

        int turn = 2;
        int[] named = new int[n];

        int next;
        for (int i = 1; i < _input.Length; i++)
        {
            next = _input[i];
            yield return next;
            named[previous] = turn - 1;
            previous = next;
            turn++;
        }

        while (turn <= n)
        {
            next = named[previous];
            if (next != 0)
                next = turn - next - 1;
            yield return next;
            named[previous] = turn - 1;
            previous = next;
            turn++;
        }
    }
}
