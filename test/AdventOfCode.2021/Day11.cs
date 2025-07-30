namespace AdventOfCode._2021;

public class Day11
{
    int[,] _map;
    IntVec2 _bounds;

    public Day11()
    {
        string[] lines = File.ReadAllLines("Inputs/Day11.txt");
        _bounds = new IntVec2(lines[0].Length, lines.Length);
        _map = new int[_bounds.X, _bounds.Y];

        for (int i = 0; i < _bounds.X; i++)
            for (int j = 0; j < _bounds.Y; j++)
                _map[i, j] = lines[j][i] - '0';
    }

    [Fact]
    public void Part1()
    {
        int answer = Run().Take(100).Sum();
        Assert.Equal(1591, answer);
    }

    [Fact]
    public void Part2()
    {
        int target = _bounds.X * _bounds.Y;
        int iteration = 0;
        foreach (int flashers in Run())
        {
            iteration++;
            if (flashers == target)
                break;
        }

        Assert.Equal(314, iteration);
    }

    private IEnumerable<int> Run()
    {
        List<IntVec2> flashers = new(_bounds.X * _bounds.Y);
        while (true)
        {
            for (int i = 0; i < _bounds.X; i++)
                for (int j = 0; j < _bounds.Y; j++)
                    if (++_map[i, j] == 10)
                        flashers.Add(new IntVec2(i, j));

            for (int i = 0; i < flashers.Count; i++)
                foreach (IntVec2 adj in flashers[i].Surrounding(_bounds))
                    if (++_map[adj.X, adj.Y] == 10)
                        flashers.Add(adj);

            yield return flashers.Count;

            foreach (IntVec2 toZero in flashers)
                _map[toZero.X, toZero.Y] = 0;

            flashers.Clear();
        }
    }
}
