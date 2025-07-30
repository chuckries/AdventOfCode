using System.Text;

namespace AdventOfCode._2019;

public class Day11
{
    long[] _program = File.ReadAllText("Inputs/Day11.txt")
        .Split(',')
        .Select(long.Parse)
        .ToArray();

    [Fact]
    public void Part1()
    {
        Dictionary<IntVec2, bool> canvas = new Dictionary<IntVec2, bool>();
        Run(_program, canvas);
        int answer = canvas.Count;
        Assert.Equal(2018, answer);
    }

    [Fact]
    public void Part2()
    {
        Dictionary<IntVec2, bool> canvas = new Dictionary<IntVec2, bool>
        { { IntVec2.Zero, true } };
        Run(_program, canvas);

        (IntVec2 min, IntVec2 max) = IntVec2.MinMax(canvas.Keys);

        StringBuilder sb = new StringBuilder();
        sb.AppendLine();

        for (int y = max.Y; y >= min.Y; y--)
        {
            for (int x = min.X; x <= max.X; x++)
            {
                if (canvas.TryGetValue((x, y), out bool val))
                    sb.Append(val ? '█' : ' ');
                else
                    sb.Append(' ');
            }
            sb.AppendLine();
        }

        string answer = sb.ToString();

        const string expected = @"
  ██  ███  ████ █  █ ███  █  █ ███  ███    
 █  █ █  █ █    █ █  █  █ █ █  █  █ █  █   
 █  █ █  █ ███  ██   █  █ ██   ███  █  █   
 ████ ███  █    █ █  ███  █ █  █  █ ███    
 █  █ █    █    █ █  █ █  █ █  █  █ █ █    
 █  █ █    █    █  █ █  █ █  █ ███  █  █   
";

        Assert.Equal(expected, answer);
    }

    private void Run(long[] program, Dictionary<IntVec2, bool> canvas)
    {
        IntVec2 position = IntVec2.Zero;
        IntVec2 heading = IntVec2.UnitY;
        bool writeMode = false;

        IntCode.InputReader reader = () =>
        {
            if (!canvas.TryGetValue(position, out bool value))
                value = false;

            return value ? 1 : 0;
        };

        IntCodeBase.OutputWriter writer = value =>
        {
            if (!writeMode)
            {
                canvas[position] = value != 0;
            }
            else
            {
                if (value == 0)
                    heading = heading.RotateLeft();
                else if (value == 1)
                    heading = heading.RotateRight();
                else
                    throw new InvalidOperationException();

                position += heading;
            }
            writeMode = !writeMode;
        };

        IntCode intCode = new IntCode(program, reader, writer);
        intCode.Run();
    }
}
