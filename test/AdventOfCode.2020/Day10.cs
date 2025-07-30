using System.Text;

namespace AdventOfCode._2020;

public class Day10
{
    int[] _input;

    public Day10()
    {
        _input = File.ReadAllLines("Inputs/Day10.txt").Select(int.Parse).Append(0).ToArray();
        Array.Sort(_input);
    }

    [Fact]
    public void Part1()
    {
        int ones = 0;
        int threes = 1;
        for (int i = 0; i < _input.Length - 1; i++)
        {
            int diff = _input[i + 1] - _input[i];
            if (diff == 1) ones++;
            else if (diff == 3) threes++;
        }

        int answer = ones * threes;
        Assert.Equal(1998, answer);
    }

    [Fact]
    public void Part2()
    {
        long[] paths = new long[_input.Length];
        paths[0] = 1;

        for (int i = 0; i < paths.Length - 1; i++)
        {
            paths[i + 1] += paths[i];
            for (int j = i + 2; j < paths.Length && _input[j] - _input[i] <= 3; j++)
                paths[j] += paths[i];
        }

        long answer = paths[^1];

        Assert.Equal(347250213298688, answer);
    }
}
