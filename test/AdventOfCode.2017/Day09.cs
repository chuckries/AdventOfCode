namespace AdventOfCode._2017;

public class Day09
{
    string _input;

    public Day09()
    {
        _input = File.ReadAllText("Inputs/Day09.txt");
    }

    [Fact]
    public void Part1()
    {
        char[] tokens = new[] { '{', '}', '<', '>' };

        int currentScore = 0;
        int total = 0;
        int index = 0;

        for (; index < _input.Length; index++)
        {
            char c = _input[index];

            if (c is '{')
            {
                currentScore++;
                total += currentScore;
            }
            else if (c is '}')
                currentScore--;
            else if (c is '<')
            {
                index++;
                for (; index < _input.Length; index++)
                {
                    c = _input[index];
                    if (c is '!')
                        index++;
                    else if (c is '>')
                        break;
                }
            }
        }

        Assert.Equal(8337, total);
    }

    [Fact]
    public void Part2()
    {
        int count = 0;
        int index = 0;
        for (; index < _input.Length; index++)
        {
            char c = _input[index];
            if (c is '<')
            {
                index++;
                for (; index < _input.Length; index++)
                {
                    c = _input[index];
                    if (c is '!')
                        index++;
                    else if (c is '>')
                        break;
                    else
                        count++;
                }
            }
        }

        Assert.Equal(4330, count);
    }
}
