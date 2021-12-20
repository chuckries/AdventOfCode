namespace AdventOfCode._2021
{
    public class Day20
    {
        readonly bool[] _lookup;
        readonly bool[,] _start;
        readonly IntVec2 _startBounds;

        public Day20()
        {
            string[] lines = File.ReadAllLines("Inputs/Day20.txt");
            _lookup = lines[0].Select(c => c is '#').ToArray();

            _startBounds = new IntVec2(lines[2].Length, lines.Length - 2);
            _start = new bool[_startBounds.X, _startBounds.Y];
            for (int j = 2; j < lines.Length; j++)
                for (int i = 0; i < lines[2].Length; i++)
                    if (lines[j][i] is '#')
                        _start[i, j - 2] = true;
        }

        [Fact]
        public void Part1()
        {
            int answer = Run(2);
            Assert.Equal(5663, answer);
        }

        [Fact]
        public void Part2()
        {
            int answer = Run(50);
            Assert.Equal(19638, answer);
        }

        private int Run(int iterations)
        {
            if (iterations % 2 is not 0)
                throw new InvalidOperationException();

            IntVec2 finalSize = _startBounds + (2 * iterations);
            bool[,] current = new bool[finalSize.X, finalSize.Y];
            bool[,] next = (bool[,])current.Clone();

            (IntVec2 low, IntVec2 hi) bounds = (new IntVec2(iterations, iterations), new IntVec2(finalSize.X - iterations - 1, finalSize.Y - iterations - 1));

            for (int i = bounds.low.X; i <= bounds.hi.X; i++)
                for (int j = bounds.low.Y; j <= bounds.hi.Y; j++)
                    current[i, j] = _start[i - bounds.low.X, j - bounds.low.Y];

            bounds = (bounds.low - 1, bounds.hi + 1);

            bool toggle = false;
            for (int iteration = 0; iteration < iterations; iteration++)
            {
                for (int i = bounds.low.X; i <= bounds.hi.X; i++)
                    for (int j = bounds.low.Y; j <= bounds.hi.Y; j++)
                    {
                        int val = 0;
                        for (int v = j - 1; v <= j + 1; v++)
                            for (int u = i - 1; u <= i + 1; u++)
                            {
                                bool sample;
                                if (u <= bounds.low.X || u >= bounds.hi.X || v <= bounds.low.Y || v >= bounds.hi.Y)
                                    sample = toggle;
                                else
                                    sample = current[u, v];

                                val <<= 1;
                                if (sample)
                                    val |= 1;
                            }

                        next[i, j] = _lookup[val];
                    }

                bounds = (bounds.low - 1, bounds.hi + 1);
                (current, next) = (next, current);
                toggle = !toggle;
            }

            int count = 0;
            for (int i = 0; i < finalSize.X; i++)
                for (int j = 0; j < finalSize.Y; j++)
                    if (current[i, j])
                        count++;

            return count;
        }
    }
}
