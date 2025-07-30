namespace AdventOfCode._2021;

public class Day07
{
    readonly int[] _input;
    readonly int _min;
    readonly int _max;

    public Day07()
    {
        _input = File.ReadAllText("Inputs/Day07.txt").Split(',').Select(int.Parse).ToArray();

        int min = int.MaxValue;
        int max = int.MinValue;

        foreach (int i in _input)
        {
            if (i < min)
                min = i;
            if (i > max)
                max = i;
        }

        _min = min;
        _max = max;
    }

    [Fact]
    public void Part1()
    {
        int minFuel = int.MaxValue;

        for (int i = _min; i <= _max; i++)
        {
            int fuel = 0;
            for (int j = 0; j < _input.Length; j++)
                fuel += Abs(i - _input[j]);
            if (fuel < minFuel)
                minFuel = fuel;
        }

        Assert.Equal(329389, minFuel);
    }

    [Fact]
    public void Part2()
    {
        int minFuel = int.MaxValue;

        for (int i = _min; i <= _max; i++)
        {
            int fuel = 0;
            for (int j = 0; j < _input.Length; j++)
            {
                int dist = Abs(i - _input[j]);
                fuel += dist * (dist + 1) / 2;
            }
            if (fuel < minFuel)
                minFuel = fuel;
        }

        Assert.Equal(86397080, minFuel);
    }
}
