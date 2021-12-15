namespace AdventOfCode._2021
{
    public class Day15
    {
        private readonly int[,] _map;
        private readonly IntVec2 _bounds;

        public Day15()
        {
            string[] lines = File.ReadAllLines("Inputs/Day15.txt");
            _bounds = new(lines[0].Length, lines.Length);

            _map = new int[_bounds.X, _bounds.Y];
            for (int i = 0; i < _bounds.X; i++)
                for (int j = 0; j < _bounds.Y; j++)
                    _map[i, j] = lines[j][i] - '0';
        }

        [Fact]
        public void Part1()
        {
            int answer = MinDistance(IntVec2.Zero, _bounds - 1, _map, _bounds);
            Assert.Equal(595, answer);
        }

        [Fact]
        public void Part2()
        {
            const int multi = 5;
            IntVec2 bounds = _bounds * multi;
            int[,] map = new int[bounds.X, bounds.Y];

            for (int i = 0; i < bounds.X; i++)
                for (int j = 0; j < bounds.Y; j++)
                {
                    int add = i / _bounds.X + j / _bounds.Y;
                    int val = _map[i % _bounds.X, j % _bounds.Y];
                    for (int adds = 0; adds < add; adds++)
                    {
                        val++;
                        if (val == 10)
                            val = 1;
                    }

                    map[i, j] = val;
                }

            int answer = MinDistance(IntVec2.Zero, bounds - 1, map, bounds);
            Assert.Equal(2914, answer);
        }

        private static int MinDistance(IntVec2 start, IntVec2 end, int[,] map, IntVec2 bounds)
        {
            PriorityQueue<IntVec2, int> toSearch = new(bounds.X * bounds.Y);
            int[,] dists = new int[bounds.X, bounds.Y];
            for (int i = 0; i < bounds.X; i++)
                for (int j = 0; j < bounds.Y; j++)
                    dists[i, j] = int.MaxValue;

            toSearch.Enqueue(start, 0);
            dists[start.X, start.Y] = 0;
            while (toSearch.TryDequeue(out IntVec2 pCurrent, out int dCurrent))
            {
                if (pCurrent == end)
                    return dCurrent;

                if (dCurrent == dists[pCurrent.X, pCurrent.Y])
                {
                    foreach (IntVec2 adj in pCurrent.Adjacent(bounds))
                    {
                        int candDist = dCurrent + map[adj.X, adj.Y];
                        ref int adjDist = ref dists[adj.X, adj.Y];
                        if (candDist < adjDist)
                        {
                            adjDist = candDist;
                            toSearch.Enqueue(adj, adjDist);
                        }
                    }
                }
            }

            throw new InvalidOperationException();
        }
    }
}
