using System.Text;

namespace AdventOfCode._2015;

public class Day10
{
    const string Input = "1321131112";

    [Fact]
    public void Part1()
    {
        string answer = Transform(Input, 40);

        Assert.Equal(492982, answer.Length);
    }

    [Fact]
    public void Part2()
    {
        string answer = Transform(Input, 50);

        Assert.Equal(6989950, answer.Length);
    }

    private static string Transform(string input, int iterations)
    {
        string current = input;
        for (int i = 0; i < iterations; i++)
        {
            current = Transform(current);
        }
        return current;
    }

    private static string Transform(string input)
    {
        int index = 0;
        StringBuilder s = new StringBuilder();

        while (index < input.Length)
        {
            char c = input[index++];
            int count = 1;
            while (index < input.Length && input[index] == c)
            {
                count++;
                index++;
            }

            s.Append(count.ToString());
            s.Append(c);
        }

        return s.ToString();
    }
}
