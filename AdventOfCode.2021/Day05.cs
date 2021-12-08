namespace AdventOfCode._2021
{
    public class Day05
    {
        private readonly IEnumerable<(IntVec2 p0, IntVec2 p1)> _input;
        private Dictionary<IntVec2, int> _map = new();

        public Day05()
        {
            _input = File.ReadAllLines("Inputs/Day05.txt")
                .Select(l => l.Split(new[] { ",", " ", "->" }, StringSplitOptions.RemoveEmptyEntries))
                .Select(tok => (new IntVec2(int.Parse(tok[0]), int.Parse(tok[1])), new IntVec2(int.Parse(tok[2]), int.Parse(tok[3]))));
        }

        [Fact]
        public void Part1()
        {
            foreach (var line in _input)
                AddNonDiagonalLine(line.p0, line.p1);

            int answer = _map.Values.Count(v => v > 1);

            Assert.Equal(5576, answer);
        }

        [Fact]
        public void Part2()
        {
            foreach (var line in _input)
                AddLine(line.p0, line.p1);

            int answer = _map.Values.Count(v => v > 1);

            Assert.Equal(18144, answer);
        }

        private void AddLine(IntVec2 p0, IntVec2 p1)
        {
            if (p0.X != p1.X && p0.Y != p1.Y)
            {
                int x0 = p0.X;
                int x1 = p1.X;
                int y0 = p0.Y;
                int y1 = p1.Y;

                if (x1 < x0)
                {
                    x0 = p1.X;
                    x1 = p0.X;
                    y0 = p1.Y;
                    y1 = p0.Y;
                }

                int j = y0;

                bool dec = y1 < y0;

                for (int i = x0; i <= x1; i++)
                {
                    AddPoint(new IntVec2(i, j));
                    if (dec)
                        j--;
                    else
                        j++;
                }
            }
            else
                AddNonDiagonalLine(p0, p1);
        }

        private void AddNonDiagonalLine(IntVec2 p0, IntVec2 p1)
        {
            if (p0.X != p1.X && p0.Y == p1.Y)
            {
                int x0 = p0.X;
                int x1 = p1.X;
                if (x1 < x0)
                {
                    x0 = p1.X;
                    x1 = p0.X;
                }
                for (int i = x0; i <= x1; i++)
                    AddPoint(new IntVec2(i, p0.Y));
            }
            
            if (p0.Y != p1.Y && p0.X == p1.X)
            {
                int y0 = p0.Y;
                int y1 = p1.Y;
                if (y1 < y0)
                {
                    y0 = p1.Y;
                    y1 = p0.Y;
                }
                for (int j = y0; j <= y1; j++)
                    AddPoint(new IntVec2(p0.X, j));
            }
        }

        private void AddPoint(IntVec2 p)
        {
            if (!_map.ContainsKey(p))
                _map[p] = 1;
            else
                _map[p]++;
        }
    }
}
