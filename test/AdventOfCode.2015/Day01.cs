namespace AdventOfCode._2015;

public class Day01
{

    string _str;

    public Day01()
    {
        _str = File.ReadAllText("Inputs/Day01.txt");
    }

    [Fact]
    public void Part1()
    {
        int answer = _str.
            Select(c => c switch
            {
                '(' => 1,
                ')' => -1,
                _ => throw new InvalidOperationException()
            })
            .Sum();

        Assert.Equal(138, answer);
    }

    [Fact]
    public void Part2()
    {
        int index = 0;
        int total = 0;
        while (true)
        {
            char c = _str[index++];
            total += c switch
            {
                '(' => 1,
                ')' => -1,
                _ => throw new OperationCanceledException()
            };

            if (total == -1)
                break;
        }

        Assert.Equal(1771, index);
    }
}
