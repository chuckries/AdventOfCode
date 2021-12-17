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
            // vax vy0 is -(bounds.lo.y + 1): assuming a positive vy0 and the target in the stricly negative y,
            // we will go up and come back down to y = 0 with v = -vy0. Fastest vel would be going from y = 0 to y = bounds.lo.y 
            // in a single step. 
            // max py caused by vy0 is 0 + v0 + (v0 - 1) + (v0 - 2) + ... + (v0 - (v0 -1)) + (v0 - v0) = 
            //      0 + 1 + 2 + 3 + 4 + .. + v0 =
            //      v0 * (v0 + 1) / 2

            int maxVy0 = -(_bounds.low.Y + 1);
            int maxPy = maxVy0 * (maxVy0 + 1) / 2;

            Assert.Equal(7503, maxPy);
        }

        [Fact]
        public void Part2()
        {
            int count = 0;

            // every initial velocity that hits in 1 step is trivial, so add each of these to the count
            // it ends up being the area of the range
            // we will then only search for velocities that take > 1 step
            count += (_bounds.hi.X - _bounds.low.X + 1) * (_bounds.hi.Y - _bounds.low.Y + 1);

            List<int> xVels = new();
            List<int> yVels = new();

            // result of calculating v0 * (v0 + 1) / 2 >= _bouds.low.X
            int minX = (int)Ceiling((Sqrt(1 + 8 * _bounds.low.X) - 1) / 2);

            // largest initial velocity that will hit in 2 steps
            int maxX = (_bounds.hi.X + 1) / 2;

            for (int i = minX; i <= maxX; i++)
                if (HitsX(i))
                    xVels.Add(i);

            // similar math to x, most negative vel we can have and hit the target in 2 steps
            int minY = (_bounds.low.Y + 1) / 2;

            // highest velocity where we can still hit the range
            // on the way back down, when y == 0, v == -v0. if v0 > the lower bound of y, we will always pass over it
            int maxY = -(_bounds.low.Y + 1);

            for (int j = minY; j <= maxY; j++)
                if (HitsY(j))
                    yVels.Add(j);

            foreach (int i in xVels)
                foreach (int j in yVels)
                    if (HitsXY(i, j))
                        count++;

            Assert.Equal(3229, count);
        }

        [Fact]
        public void Part2Alt()
        {
            int total = 0;

            // every initial velocity that is a point within the range will immediately hit, so just count them
            total += (_bounds.hi.X - _bounds.low.X + 1) * (_bounds.hi.Y - _bounds.low.Y + 1);

            // calculate vx0's that will stall within the range and have valid t's at any time > stall t
            List<int> xStalls = new();

            // result of calculating v0 * (v0 + 1) / 2 >= _bouds.low.X
            int minStallX = (int)Ceiling((Sqrt(1 + 8 * _bounds.low.X) - 1) / 2);
            int maxStallX = (int)((Sqrt(1 + 8 * _bounds.hi.X) - 1) / 2);
            for (int xStall = minStallX; xStall <= maxStallX; xStall++)
            {
                // for this stall we want to add the min time it hits the range, not the time it stalls
                xStalls.Add((int)Ceiling((2 * xStall + 1 - Sqrt(Pow(2 * xStall + 1, 2) - 8 * _bounds.low.X)) / 2));
            }

            // collect hits in our unknown range
            // our lower bound is maxStallX + 1
            // our upper bound is (_bounds.hi.X + 1) / 2 because math (this is the highest velocity that hits the range in 2 steps, we've already account for all 1 steps)
            int minSearchX = maxStallX + 1;
            int maxSearchX = (_bounds.hi.X + 1) / 2;
            List<(int vx0, int t)> xHits = new();
            for (int x = minSearchX; x <= maxSearchX; x++)
                CollectXHits(x, xHits);

            // search y's 
            int minSearchY = (_bounds.low.Y + 1) / 2; // similar math to x, most negative vel we can have and hit the target in 2 steps
            int maxSearchY = -(_bounds.low.Y + 1); // explained in part 1
            List<(int vy0, int t)> yHits = new();
            for (int y = minSearchY; y <= maxSearchY; y++)
                CollectYHits(y, yHits);

            HashSet<IntVec2> uniques = new(xHits.Count * yHits.Count);
            foreach (var y in yHits)
            {
                foreach (int xStall in xStalls)
                    if (xStall <= y.t)
                        uniques.Add(new IntVec2(xStall, y.vy0));

                foreach (var x in xHits)
                    if (x.t == y.t)
                        uniques.Add(new IntVec2(x.vx0, y.vy0));
            }

            total += uniques.Count;
            Assert.Equal(3229, total);
        }

        void CollectXHits(int vx0, List<(int vx0, int t)> hits)
        {
            int px = vx0;
            int vx = vx0 - 1;
            int t = 1;
            while (true)
            {
                if (InXRange(px))
                    hits.Add((vx0, t));

                if (px >= _bounds.hi.X)
                    break;

                IncX(ref px, ref vx);
                t++;
            }
        }

        void CollectYHits(int vy0, List<(int vy0, int t)> hits)
        {
            int py = vy0;
            int vy = vy0 - 1;
            int t = 1;
            while (true)
            {
                if (InYRange(py))
                    hits.Add((vy0, t));

                if (py <= _bounds.low.Y)
                    break;

                IncY(ref py, ref vy);
                t++;
            }
        }

        bool HitsX(int vx0)
        {
            int px = 0;
            int vx = vx0;
            while (true)
            {
                if (InXRange(px))
                    return true;

                if (ShouldStopX(px, vx))
                    return false;

                IncX(ref px, ref vx);
            }
        }

        bool HitsY(int vy0)
        {
            int py = 0;
            int vy = vy0;
            while (true)
            {
                if (InYRange(py))
                    return true;

                if (ShouldStopY(py, vy))
                    return false;

                IncY(ref py, ref vy);
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
                if (InXYRange(px, py))
                    return true;

                if (ShouldStopX(px, vx) || ShouldStopY(py, vy))
                    return false;

                IncX(ref px, ref vx);
                IncY(ref py, ref vy);
            }
        }

        bool InXRange(int x) => _bounds.low.X <= x && x <= _bounds.hi.X;

        bool InYRange(int y) => _bounds.low.Y <= y && y <= _bounds.hi.Y;

        bool InXYRange(int x, int y) =>
            InXRange(x) && InYRange(y);

        bool ShouldStopX(int px, int vx) => px >= _bounds.hi.X || (vx == 0 && px <= _bounds.low.X);

        bool ShouldStopY(int py, int vy) => vy <= 0 && py <= _bounds.low.Y;

        void IncX(ref int px, ref int vx)
        {
            px += vx;
            if (vx > 0)
                vx--;
        }

        void IncY(ref int py, ref int vy)
        {
            py += vy;
            vy--;
        }
    }
}
