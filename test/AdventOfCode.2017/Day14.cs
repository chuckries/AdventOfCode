namespace AdventOfCode._2017;

public class Day14
{
    private const string Seed = "vbqugkhl";
    private bool[,] _map;

    public Day14()
    {
        _map = new bool[128, 128];

        for (int i = 0; i < 128; i++)
        {
            string hash = KnotHash.HashString($"{Seed}-{i}");

            int col = 0;
            foreach (char c in hash)
            {
                int num = c <= '9' ? c - '0' : c - 'a' + 10;
                for (int bit = 3; bit >= 0; bit--)
                    _map[i, col++] = (num & (1 << bit)) != 0;
            }
        }
    }

    [Fact]
    public void Part1()
    {
        int answer = 0;
        for (int i = 0; i < 128; i++)
            for (int j = 0; j < 128; j++)
                if (_map[i, j])
                    answer++;

        Assert.Equal(8148, answer);
    }

    [Fact]
    public void Part2()
    {
        int answer = 0;
        for (int i = 0; i < 128; i++)
            for (int j = 0; j < 128; j++)
            {
                if (_map[i, j])
                {
                    answer++;

                    Queue<IntVec2> toSearch = new();
                    toSearch.Enqueue(new IntVec2(i, j));

                    while (toSearch.Count > 0)
                    {
                        IntVec2 current = toSearch.Dequeue();

                        if (!_map[current.X, current.Y])
                            continue;

                        _map[current.X, current.Y] = false;

                        foreach (IntVec2 next in current.Adjacent(new IntVec2(128, 128)).Where(adj => _map[adj.X, adj.Y]))
                            toSearch.Enqueue(next);
                    }
                }
            }

        Assert.Equal(1180, answer);
    }
}
