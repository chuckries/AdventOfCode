using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

using static System.Math;

namespace AdventOfCode._2019
{
    public class Day10
    {
        HashSet<IntPoint2> _stars = new HashSet<IntPoint2>();

        public Day10()
        {
            string[] inputs = File.ReadAllLines("Inputs/Day10.txt").ToArray();

            for (int j = 0; j < inputs.Length; j++)
                for (int i = 0; i < inputs[j].Length; i++)
                    if (inputs[j][i] == '#')
                        _stars.Add(new IntPoint2(i, j));
        }

        [Fact]
        public void Part1()
        {
            int maxInSight = int.MinValue;
            IntPoint2 max = IntPoint2.Zero;
            foreach (IntPoint2 candidate in _stars)
            {
                HashSet<IntPoint2> slopes = new HashSet<IntPoint2>(_stars.Count);

                foreach (IntPoint2 star in _stars)
                {
                    if (candidate.Equals(star))
                        continue;

                    IntPoint2 slope = MinimizeSlope(star - candidate);
                    slopes.Add(slope);
                }

                if (slopes.Count > maxInSight)
                {
                    maxInSight = slopes.Count;
                    max = candidate;
                }
            }

            Assert.Equal(new IntPoint2(28, 29), max);
            Assert.Equal(340, maxInSight);
        }

        private IntPoint2 MinimizeSlope(IntPoint2 slope)
        {
            if (slope.X == 0)
                return new IntPoint2(0, Sign(slope.Y));
            else if (slope.Y == 0)
                return new IntPoint2(Sign(slope.X), 0);
            else
            {
                IntPoint2 absSlope = new IntPoint2(Abs(slope.X), Abs(slope.Y));

                if (absSlope.Y % absSlope.X == 0)
                    slope /= absSlope.X;
                else if (absSlope.X % absSlope.Y == 0)
                    slope /= absSlope.Y;
                else
                {
                    int max = Min(absSlope.X, absSlope.Y) / 2;
                    for (int i = max; i >= 2; i--)
                    {
                        if (absSlope.X % i == 0 && absSlope.Y % i == 0)
                        {
                            slope /= i;
                            break;
                        }
                    }
                }

                return slope;
            }
        }
    }
}
