namespace AdventOfCode._2021
{
    public class Day17
    {
        private (IntVec2 low, IntVec2 hi) _bounds;

        public Day17()
        {
            string[] tok = File.ReadAllText("Inputs/Day17.txt").Split(new [] { " ", "=", "..", "," }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            _bounds = (
                new IntVec2(tok[3], tok[6]),
                new IntVec2(tok[4], tok[7]));
        }

        [Fact]
        public void Part1()
        {
            int maxY = 0;
            for (int i = 0; i < 200; i++)
                if (HitsY(i, out int y) && y > maxY)
                    maxY = y;

            Assert.Equal(7503, maxY);
        }

        [Fact]
        public void Part2()
        {
            List<int> xVels = new();
            List<int> yVels = new();

            for (int i = 1; i <= 200; i++)
                if (HitsX(i))
                    xVels.Add(i);

            for (int j = -200; j <= 200; j++)
                if (HitsY(j, out _))
                    yVels.Add(j);

            int count = 0;
            foreach (int i in xVels)
                foreach (int j in yVels)
                    if (HitsXY(i, j))
                        count++;

            Assert.Equal(3229, count);
        }

        bool HitsX(int vx0)
        {
            int px = 0;
            int vx = vx0;
            while (true)
            {
                if (InXRange(px))
                    return true;

                if (px > _bounds.hi.X || (vx == 0 && px < _bounds.low.X))
                    return false;

                px += vx;
                if (vx > 0)
                    vx--;
            }
        }

        bool HitsY(int vy0, out int maxY)
        {
            int py = 0;
            int vy = vy0;
            maxY = 0;
            while (true)
            {
                if (InYRange(py))
                {
                    return true;
                }

                if (vy < 0)
                {
                    // we are only decreasing, if we are lower than the range fail
                    if (py < _bounds.low.Y)
                    {
                        return false;
                    }
                }
                else if (vy == 0)
                {
                    maxY = py;
                }

                py += vy;
                vy -= 1;
            }
        }

        bool HitsXY(int vx0, int vy0)
        {
            int px = 0;
            int py = 0;
            int vx = vx0;
            int vy = vy0;

            while (true)
            {
                if (InRange(px, py))
                    return true;

                if (vy < 0 && py < _bounds.low.Y)
                    return false;

                if (px > _bounds.hi.X || (vx == 0 && px < _bounds.low.X))
                    return false;

                px += vx;
                py += vy;
                if (vx > 0) vx--;
                vy--;
            }
        }

        bool InXRange(int x) => _bounds.low.X <= x && x <= _bounds.hi.X;

        bool InYRange(int y) => _bounds.low.Y <= y && y <= _bounds.hi.Y;

        bool InRange(int x, int y) =>
            InXRange(x) && InYRange(y);
    }
}
