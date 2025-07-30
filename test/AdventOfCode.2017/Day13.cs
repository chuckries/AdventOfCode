namespace AdventOfCode._2017;

public class Day13
{
    private class Scanner
    {
        public readonly int Depth;
        public readonly int Range;

        private readonly int _modulo;

        public Scanner(int depth, int range)
        {
            Depth = depth;
            Range = range;
            _modulo = (range - 1) * 2;
        }

        public int PositionAtInitialTime(int t0) => (t0 + Depth) % _modulo;
    }

    Scanner[] _scanners;

    public Day13()
    {
        _scanners = File.ReadAllLines("Inputs/Day13.txt")
            .Select(s => s.Split(':', StringSplitOptions.TrimEntries).Select(int.Parse).ToArray())
            .Select(tok => new Scanner(tok[0], tok[1]))
            .ToArray();
    }

    [Fact]
    public void Part1()
    {
        int answer = _scanners.Where(s => s.PositionAtInitialTime(0) == 0).Sum(s => s.Depth * s.Range);

        Assert.Equal(1704, answer);
    }

    [Fact]
    public void Part2()
    {
        int time = 0;
        while (true)
        {
            for (int i = 0; i < _scanners.Length; i++)
                if (_scanners[i].PositionAtInitialTime(time) == 0)
                    goto increment;

            break;

        increment:
            time++;
        }

        Assert.Equal(3970918, time);
    }
}
