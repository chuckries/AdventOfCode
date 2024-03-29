﻿using AdventOfCode.Common;
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
        HashSet<IntVec2> _asteroids = new HashSet<IntVec2>();

        public Day10()
        {
            string[] inputs = File.ReadAllLines("Inputs/Day10.txt").ToArray();

            for (int j = 0; j < inputs.Length; j++)
                for (int i = 0; i < inputs[j].Length; i++)
                    if (inputs[j][i] == '#')
                        _asteroids.Add(new IntVec2(i, j));
        }

        [Fact]
        public void Part1()
        {
            int maxInSight = int.MinValue;
            IntVec2 max = IntVec2.Zero;
            foreach (IntVec2 candidate in _asteroids)
            {
                HashSet<IntVec2> slopes = new HashSet<IntVec2>(_asteroids.Count);

                foreach (IntVec2 asteroid in _asteroids)
                {
                    if (candidate.Equals(asteroid))
                        continue;

                    IntVec2 slope = ReduceSlope(asteroid - candidate);
                    slopes.Add(slope);
                }

                if (slopes.Count > maxInSight)
                {
                    maxInSight = slopes.Count;
                    max = candidate;
                }
            }

            Assert.Equal(new IntVec2(28, 29), max);
            Assert.Equal(340, maxInSight);
        }

        [Fact] 
        public void Part2()
        {
            IntVec2 origin = new IntVec2(28, 29);

            Dictionary<IntVec2, IntVec2> slopeSearch = new Dictionary<IntVec2, IntVec2>();
            foreach (IntVec2 asteroid in _asteroids)
            {
                if (origin.Equals(asteroid))
                    continue;

                IntVec2 vector = asteroid - origin;
                IntVec2 minSlope = ReduceSlope(vector);

                if (!slopeSearch.TryGetValue(minSlope, out IntVec2 minVector))
                {
                    slopeSearch.Add(minSlope, vector);
                }
                else if (vector.Manhattan < minVector.Manhattan)
                {
                    slopeSearch[minSlope] = vector;
                }
            }

            Assert.Equal(340, slopeSearch.Count);

            List<IntVec2> uniqueVectors = slopeSearch.Values.ToList();
            uniqueVectors.Sort(Comparer<IntVec2>.Create((left, right) =>
            {
                IntVec2 signLeft = left.Transform(Sign);
                IntVec2 signRight = right.Transform(Sign);

                int GetSlopeSortValue(IntVec2 slope) => slope switch
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

            IntVec2 correctAsteroid = uniqueVectors[199] + origin;
            int answer = correctAsteroid.X * 100 + correctAsteroid.Y;
            Assert.Equal(2628, answer);
        }

        private IntVec2 ReduceSlope(IntVec2 slope)
        {
            if (slope.X == 0 || slope.Y == 0)
                return new IntVec2(Sign(slope.X), Sign(slope.Y));
            else
            {
                return slope / (int)MathUtils.GreatestCommonFactor(slope.X, slope.Y);
            }
        }
    }
}
