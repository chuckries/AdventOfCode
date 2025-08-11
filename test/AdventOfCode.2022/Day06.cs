using System.Runtime.ExceptionServices;

namespace AdventOfCode._2022;

public class Day06
{
    string _input;

    public Day06()
    {
        _input = File.ReadAllText("Inputs/Day06.txt");
    }

    private int Search(int count)
    {
        Dictionary<char, int> counts = new();

        for (int i = 0; i < count; i++)
        {
            if (!counts.ContainsKey(_input[i]))
            {
                counts[_input[i]] = 0;
            }
            counts[_input[i]]++;
        }

        int front = 0;
        int back = count;

        while (true)
        {
            if (counts.Count == count)
                return back;

            counts[_input[front]]--;
            if (counts[_input[front]] == 0)
                counts.Remove(_input[front]);

            if (!counts.ContainsKey(_input[back]))
                counts[_input[back]] = 0;
            counts[_input[back]]++;

            front++;
            back++;
        }
    }

    [Fact]
    public void Part1()
    {
        int answer = Search(4);
        Assert.Equal(1542, answer);
    }

    [Fact]
    public void Part2()
    {
        int answer = Search(14);
        Assert.Equal(3153, answer);
    }
}
