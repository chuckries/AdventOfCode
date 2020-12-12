using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using AdventOfCode.Common;

using Xunit;

namespace AdventOfCode._2016
{
    public class Day01
    {
        string[] _input;

        public Day01()
        {
            _input = File.ReadAllText("Inputs/Day01.txt").Split(',', StringSplitOptions.TrimEntries);
        }

        [Fact]
        public void Part1()
        {
            int answer = EnumPositions().Last().Distance;
            Assert.Equal(226, answer);
        }

        [Fact]
        public void Part2()
        {
            HashSet<IntVec2> visited = new HashSet<IntVec2>();
            int answer = 0;
            foreach (IntVec2 pos in EnumPositions())
            {
                if (!visited.Add(pos))
                {
                    answer = pos.Distance;
                    break;
                }
            }

            Assert.Equal(79, answer);
        }

        private IEnumerable<IntVec2> EnumPositions()
        {
            IntVec2 position = IntVec2.Zero;
            IntVec2 heading = IntVec2.UnitY;

            foreach (string s in _input)
            {
                heading = s[0] switch
                {
                    'R' => heading.RotateRight(),
                    'L' => heading.RotateLeft(),
                    _ => throw new InvalidOperationException()
                };

                int distance = int.Parse(s.AsSpan(1));
                for (int i = 0; i < distance; i++)
                {
                    position += heading;
                    yield return position;
                }
            }
        }
    }
}
