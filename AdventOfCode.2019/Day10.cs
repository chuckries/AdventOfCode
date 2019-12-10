using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace AdventOfCode._2019
{
    public class Day10
    {
        HashSet<IntPoint2> _stars = new HashSet<IntPoint2>();
        IntPoint2 _bounds;

        public Day10()
        {
            string[] inputs = File.ReadAllLines("Inputs/Day10.txt").ToArray();

            _bounds = new IntPoint2(inputs[0].Length, inputs.Length);

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
                int starsInSight = 0;
                foreach (IntPoint2 star in _stars)
                {
                    if (candidate.Equals(star))
                        continue;

                    IntPoint2 slope = MinimizeSlope(star - candidate);

                    IntPoint2 current = candidate;
                    do
                    {
                        current += slope;
                        if (_stars.Contains(current))
                        {
                            if (current.Equals(star))
                                starsInSight++;

                            break;
                        }

                    } while (current.X <= _bounds.X && current.Y <= _bounds.Y);
                }

                if (starsInSight > maxInSight)
                {
                    maxInSight = starsInSight;
                    max = candidate;
                }
            }

            Assert.Equal(340, maxInSight);
        }

        private IntPoint2 MinimizeSlope(IntPoint2 slope)
        {
            if (slope.X == 0)
                return new IntPoint2(0, Math.Sign(slope.Y));
            else if (slope.Y == 0)
                return new IntPoint2(Math.Sign(slope.X), 0);
            else
            {
                IntPoint2 absSlope = new IntPoint2(Math.Abs(slope.X), Math.Abs(slope.Y));

                while (absSlope.Y > 1 && absSlope.X > 1)
                {
                    if (absSlope.Y % absSlope.X == 0)
                        absSlope /= absSlope.X;
                    else if (absSlope.X % absSlope.Y == 0)
                        absSlope /= absSlope.Y;
                    else
                    {
                        int max = Math.Min(absSlope.X, absSlope.Y) / 2;
                        bool foundFactor = false;
                        for (int i = max; i > 1; i--)
                        {
                            if (absSlope.X % i == 0 && absSlope.Y % i == 0)
                            {
                                absSlope /= i;
                                foundFactor = true;
                                break;
                            }
                        }

                        if (!foundFactor)
                            break;
                    }
                }

                return new IntPoint2(absSlope.X * Math.Sign(slope.X), absSlope.Y * Math.Sign(slope.Y));
            }
        }
    }
}
