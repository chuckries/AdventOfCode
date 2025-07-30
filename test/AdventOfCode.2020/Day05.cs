namespace AdventOfCode._2020;

public class Day05
{
    string[] _input;

    public Day05()
    {
        _input = File.ReadAllLines("Inputs/Day05.txt");
    }

    [Fact]
    public void Part1()
    {
        int answer = _input.Select(Calculate).Max();

        Assert.Equal(980, answer);
    }

    [Fact]
    public void Part2()
    {
        List<int> seats = _input.Select(Calculate).ToList();
        seats.Sort();

        int answer = 0;
        for (int i = 0; i < seats.Count - 1; i++)
            if (seats[i] == seats[i + 1] - 2)
            {
                answer = seats[i] + 1;
                break;
            }

        Assert.Equal(607, answer);
    }

    private int Calculate(string input) =>
        input.Aggregate(0, (total, c) => (total << 1) | (c is 'B' or 'R' ? 1 : 0));
}
