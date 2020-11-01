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
            int answer = EnumPositions().Last().Manhattan;
            Assert.Equal(226, answer);
        }

        [Fact]
        public void Part2()
        {
            HashSet<IntPoint2> visited = new HashSet<IntPoint2>();
            int answer = 0;
            foreach (IntPoint2 pos in EnumPositions())
            {
                if (!visited.Add(pos))
                {
                    answer = pos.Manhattan;
                    break;
                }
            }

            Assert.Equal(79, answer);
        }

        private IEnumerable<IntPoint2> EnumPositions()
        {
            IntPoint2 position = IntPoint2.Zero;
            IntPoint2 heading = IntPoint2.UnitY;

            foreach (string s in _input)
            {
                heading = s[0] switch
                {
                    'R' => heading.TurnRight(),
                    'L' => heading.TurnLeft(),
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
