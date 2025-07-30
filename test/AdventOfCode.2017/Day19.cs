namespace AdventOfCode._2017;

public class Day19
{
    private readonly char[,] _map;
    private readonly IntVec2 _start;

    public Day19()
    {
        string[] lines = File.ReadAllLines("Inputs/Day19.txt");
        IntVec2 bounds = new IntVec2(lines[0].Length, lines.Length);
        _map = new char[bounds.X, bounds.Y];

        for (int i = 0; i < bounds.X; i++)
        {
            char c = lines[0][i];
            _map[i, 0] = c;
            if (c == '|')
                _start = new IntVec2(i, 0);
        }

        for (int j = 1; j < bounds.Y; j++)
            for (int i = 0; i < bounds.X; i++)
                _map[i, j] = lines[j][i];
    }

    [Fact]
    public void Part1()
    {
        (_, string letters) = Traverse();
        Assert.Equal("VEBTPXCHLI", letters);
    }

    [Fact]
    public void Part2()
    {
        (int steps, _) = Traverse();
        Assert.Equal(18702, steps);
    }

    (int steps, string letters) Traverse()
    {
        int steps = 0;
        List<char> letters = new();

        IntVec2 pos = _start;
        IntVec2 dir = IntVec2.UnitY;

        while (true)
        {
            pos = pos + dir;
            steps++;

            char c = _map[pos.X, pos.Y];

            if (char.IsLetter(c))
                letters.Add(c);
            else if (c == '+')
            {
                IntVec2 left = dir.RotateLeft();
                IntVec2 leftPos = pos + left;
                if (_map[leftPos.X, leftPos.Y] != ' ')
                    dir = left;
                else
                    dir = dir.RotateRight();
            }
            else if (c == ' ')
                break;
        }

        return (steps, new string(letters.ToArray()));
    }
}
