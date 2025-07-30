using System.Text;

namespace AdventOfCode._2019;

public class Day17
{
    long[] _program = File.ReadAllText("Inputs/Day17.txt")
        .Split(',')
        .Select(long.Parse)
        .ToArray();

    string[] _map;

    public Day17()
    {
        StringBuilder sb = new StringBuilder();
        IntCode intCode = new IntCode(
            _program,
            null,
            value =>
            {
                if (value == 10)
                    sb.AppendLine();
                else
                    sb.Append((char)value);
            });

        intCode.Run();

        _map = sb.ToString().Split().Where(s => s != string.Empty).ToArray();
    }

    [Fact]
    public void Part1()
    {
        int total = 0;
        for (int j = 0; j < _map.Length; j++)
        {
            for (int i = 0; i < _map[j].Length; i++)
            {
                if (_map[j][i] == '#')
                {
                    if (j - 1 >= 0 && _map[j - 1][i] == '#' &&
                        j + 1 < _map.Length && _map[j + 1][i] == '#' &&
                        i - 1 >= 0 && _map[j][i - 1] == '#' &&
                        i + 1 < _map[j].Length && _map[j][i + 1] == '#')
                        total += i * j;
                }
            }
        }

        Assert.Equal(3888, total);
    }

    [Fact]
    public void Part2()
    {
        string str = GetString();

        // do magic

        string[] functions = new string[]
        {
            "A,B,A,C,B,C,B,C,A,C",
            "L,10,R,12,R,12",
            "R,6,R,10,L,10",
            "R,10,L,10,L,12,R,6"
        };

        string functionString = string.Join('\n', functions) + '\n' + 'n' + '\n';
        int current = 0;
        long answer = 0;
        IntCode intCode = new IntCode(
            _program,
            () => functionString[current++],
            val => answer = val
            );
        intCode[0] = 2;
        intCode.Run();

        Assert.Equal(927809, answer);
    }

    private string GetString()
    {
        List<string> parts = new List<string>();

        IntVec2 current = GetStart();
        IntVec2 dir = -IntVec2.UnitY;

        while (true)
        {
            char? letter = GetTurn(current, ref dir);
            if (letter == null)
                break;

            parts.Add(new string(new[] { letter.Value }));

            parts.Add(MoveForward(ref current, dir).ToString());
        }

        return string.Join(',', parts);
    }

    private IntVec2 GetStart()
    {
        for (int j = 0; j < _map.Length; j++)
        {
            for (int i = 0; i < _map[0].Length; i++)
            {
                if (_map[j][i] == '^')
                    return (i, j);
            }
        }

        throw new InvalidOperationException();
    }

    private int MoveForward(ref IntVec2 current, IntVec2 direction)
    {
        int count = 0;

        while (InBounds(current + direction) &&
            _map[current.Y + direction.Y][current.X + direction.X] == '#')
        {
            current += direction;
            count++;
        }

        return count;
    }

    private char? GetTurn(IntVec2 current, ref IntVec2 direction)
    {
        IntVec2 leftPoint = current + -direction.RotateLeft();
        IntVec2 rightPoint = current + -direction.RotateRight();

        if (InBounds(leftPoint) && _map[leftPoint.Y][leftPoint.X] == '#')
        {
            direction = -direction.RotateLeft();
            return 'L';
        }
        else if (InBounds(rightPoint) && _map[rightPoint.Y][rightPoint.X] == '#')
        {
            direction = -direction.RotateRight();
            return 'R';
        }

        return null;
    }

    private bool InBounds(IntVec2 point)
    {
        return point.X >= 0 && point.X < _map[0].Length &&
               point.Y >= 0 && point.Y < _map.Length;
    }
}
