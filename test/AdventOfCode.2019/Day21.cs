using System.Text;

namespace AdventOfCode._2019;

public class Day21
{
    long[] _program;

    public Day21()
    {
        _program = File.ReadAllText("Inputs/Day21.txt").Split(',').Select(long.Parse).ToArray();
    }

    [Fact]
    public void Part1()
    {
        string answer = Run(MakeInput(
            "NOT T T",
            "AND A T",
            "AND B T",
            "AND C T",
            "NOT T J",
            "AND D J",
            "WALK"
            ));

        Assert.Equal("Input instructions:\n\nWalking...\n\n19358870", answer);
    }

    [Fact]
    public void Part2()
    {
        string answer = Run(MakeInput(
            "NOT T T",
            "AND A T",
            "AND B T",
            "AND C T",
            "NOT T J",
            "AND D J",
            "NOT E T",
            "NOT T T",
            "OR H T",
            "AND T J",
            "RUN"
            ));

        Assert.Equal("Input instructions:\n\nRunning...\n\n1143356492", answer);
    }

    private string Run(string input)
    {
        int index = 0;
        StringBuilder sb = new StringBuilder();
        IntCode intCode = new IntCode(
            _program,
            () => input[index++],
            val =>
            {
                if (val >= 0 && val <= 127)
                    sb.Append((char)val);
                else
                    sb.Append(val.ToString());
            });
        intCode.Run();
        return sb.ToString();
    }

    private string MakeInput(params string[] inputs)
    {
        return string.Join('\n', inputs) + '\n';
    }
}
