using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

using AdventOfCode.Common;
using System.IO;

namespace AdventOfCode._2020
{
    public class Day12
    {
        (char dir, int count)[] _input;

        public Day12()
        {
            _input = File.ReadAllLines("Inputs/Day12.txt")
                .Select(s => (s[0], int.Parse(s.AsSpan(1))))
                .ToArray();
        }

        [Fact]
        public void Part1()
        {
            IntVec2 pos = 0;
            IntVec2 heading = (1, 0);

            foreach ((char c, int count) in _input)
            {
                if (c is 'L' or 'R')
                {
                    int times = count / 90;
                    heading = c switch
                    {
                        'L' => heading.RotateLeft(times),
                        'R' => heading.RotateRight(times),
                        _ => throw new InvalidOperationException()
                    };
                }
                else
                {
                    pos += c switch
                    {
                        'F' => heading,
                        'N' => IntVec2.UnitY,
                        'S' => -IntVec2.UnitY,
                        'W' => -IntVec2.UnitX,
                        'E' => IntVec2.UnitX,
                        _ => throw new InvalidOperationException()
                    } * count;
                }
            }

            int answer = pos.Manhattan;
            Assert.Equal(1441, answer);
        }

        [Fact]
        public void Part2()
        {
            IntVec2 pos = IntVec2.Zero;
            IntVec2 wayPoint = (10, 1);

            foreach ((char c, int count) in _input)
            {
                if (c is 'F')
                {
                    IntVec2 delta = (wayPoint - pos) * count;
                    pos += delta;
                    wayPoint += delta;
                }
                else if (c is 'L' or 'R')
                {
                    int times = count / 90;
                    IntVec2 rel = wayPoint - pos;
                    rel = c switch
                    {
                        'L' => rel.RotateLeft(times),
                        'R' => rel.RotateRight(times),
                        _ => throw new InvalidOperationException()
                    };
                    wayPoint = pos + rel;
                }
                else
                {
                    wayPoint += c switch
                    {
                        'N' => IntVec2.UnitY,
                        'S' => -IntVec2.UnitY,
                        'W' => -IntVec2.UnitX,
                        'E' => IntVec2.UnitX,
                        _ => throw new InvalidOperationException()
                    } * count;
                }
            }

            int answer = pos.Manhattan;
            Assert.Equal(61616, answer);
        }
    }
}
