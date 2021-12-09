namespace AdventOfCode._2021
{
    public class Day09
    {
        private readonly IntVec2 _bounds;
        private readonly int[,] _map;

        public Day09()
        {
            string[] input = File.ReadAllLines("Inputs/Day09.txt");
            _bounds = new IntVec2(input[0].Length, input.Length);
            _map = new int[_bounds.X, _bounds.Y];

            for (int i = 0; i < _bounds.X; i++)
                for (int j = 0; j < _bounds.Y; j++)
                    _map[i, j] = input[j][i] - '0';
        }

        [Fact]
        public void Part1()
        {
            int risk = GetLowPoints().Sum(p => _map[p.X, p.Y] + 1);
            Assert.Equal(486, risk);
        }

        [Fact]
        public void Part2()
        {
            int answer = GetLowPoints().Select(GetBasinSize).OrderByDescending(c => c).Take(3).Aggregate(1, (total, count) => total * count);
            Assert.Equal(1059300, answer);
        }

        private IEnumerable<IntVec2> GetLowPoints()
        {
            for (int i = 0; i < _bounds.X; i++)
                for (int j = 0; j < _bounds.Y; j++)
                    if (new IntVec2(i, j).Adjacent(_bounds).All(adj => _map[i, j] < _map[adj.X, adj.Y]))
                        yield return new IntVec2(i, j);
        }

        private int GetBasinSize(IntVec2 start)
        {
            Queue<IntVec2> toSearch = new();
            HashSet<IntVec2> visited = new();
            int count = 0;

            toSearch.Enqueue(start);
            while (toSearch.Count > 0)
            {
                IntVec2 current = toSearch.Dequeue();

                if (visited.Contains(current))
                    continue;

                visited.Add(current);
                count++;

                foreach (IntVec2 next in current.Adjacent(_bounds).Where(adj => _map[adj.X, adj.Y] != 9))
                    if (!visited.Contains(next))
                        toSearch.Enqueue(next);
            }

            return count;
        }
    }
}
