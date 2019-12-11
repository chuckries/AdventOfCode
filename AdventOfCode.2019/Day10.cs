using AdventOfCode.Common;
using Newtonsoft.Json.Converters;
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
        HashSet<IntPoint2> _asteroids = new HashSet<IntPoint2>();

        public Day10()
        {
            string[] inputs = File.ReadAllLines("Inputs/Day10.txt").ToArray();

            for (int j = 0; j < inputs.Length; j++)
                for (int i = 0; i < inputs[j].Length; i++)
                    if (inputs[j][i] == '#')
                        _asteroids.Add(new IntPoint2(i, j));
        }

        [Fact]
        public void Part1()
        {
            int maxInSight = int.MinValue;
            IntPoint2 max = IntPoint2.Zero;
            foreach (IntPoint2 candidate in _asteroids)
            {
                HashSet<IntPoint2> slopes = new HashSet<IntPoint2>(_asteroids.Count);

                foreach (IntPoint2 asteroid in _asteroids)
                {
                    if (candidate.Equals(asteroid))
                        continue;

                    IntPoint2 slope = MinimizeSlope(asteroid - candidate);
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

        [Fact] 
        void Part2()
        {
            IntPoint2 origin = new IntPoint2(28, 29);

            Dictionary<IntPoint2, IntPoint2> slopeSearch = new Dictionary<IntPoint2, IntPoint2>();
            foreach (IntPoint2 asteroid in _asteroids)
            {
                if (origin.Equals(asteroid))
                    continue;

                IntPoint2 vector = asteroid - origin;
                IntPoint2 minSlope = MinimizeSlope(vector);

                if (!slopeSearch.TryGetValue(minSlope, out IntPoint2 minVector))
                {
                    slopeSearch.Add(minSlope, vector);
                }
                else if (vector.Manhattan < minVector.Manhattan)
                {
                    slopeSearch[minSlope] = vector;
                }
            }

            Assert.Equal(340, slopeSearch.Count);

            List<IntPoint2> uniqueVectors = slopeSearch.Values.ToList();
            uniqueVectors.Sort(Comparer<IntPoint2>.Create((left, right) =>
            {
                IntPoint2 signLeft = left.Transform(Sign);
                IntPoint2 signRight = right.Transform(Sign);

                int GetSlopeSortValue(IntPoint2 slope) => slope switch
                {
                    ( 0, -1) => 0,
                    ( 1, -1) => 1,
                    ( 1,  0) => 2,
                    ( 1,  1) => 3,
                    ( 0,  1) => 4,
                    (-1,  1) => 5,
                    (-1,  0) => 6,
                    (-1, -1) => 7,
                    _ => throw new InvalidOperationException()
                };

                int sortValue = GetSlopeSortValue(signLeft) - GetSlopeSortValue(signRight);
                if (sortValue == 0)
                {
                    int leftTop = Abs(left.Y * right.X);
                    int rightTop = Abs(right.Y * left.X);

                    if (signLeft.X == signLeft.Y)
                        sortValue = leftTop - rightTop;
                    else
                        sortValue = rightTop - leftTop;
                }

                return sortValue;
            }));

            IntPoint2 correctAsteroid = uniqueVectors[199] + origin;
            int answer = correctAsteroid.X * 100 + correctAsteroid.Y;
            Assert.Equal(2628, answer);
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
