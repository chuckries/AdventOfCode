using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

using AdventOfCode.Common;
using System.IO;
using System.Linq;

namespace AdventOfCode._2015
{
    public class Day3
    {
        string _input;

        public Day3()
        {
            _input = File.ReadAllText("Inputs/Day3.txt");
        }

        [Fact]
        public void Part1()
        {
            IntPoint2 current = default;
            HashSet<IntPoint2> positions = new HashSet<IntPoint2>();
            positions.Add(current);

            for (int i = 0; i < _input.Length; i++)
            {
                Move(_input[i], ref current);

                positions.Add(current);
            }

            Assert.Equal(2592, positions.Count);
        }

        [Fact]
        public void Part2()
        {
            IntPoint2 current1 = default;
            IntPoint2 current2 = default;
            HashSet<IntPoint2> positions = new HashSet<IntPoint2>();
            positions.Add(current1);

            ref IntPoint2 current = ref current1;
            ref IntPoint2 next = ref current2;

            for (int i = 0; i < _input.Length; i++)
            {
                Move(_input[i], ref current);

                positions.Add(current);

                ref IntPoint2 tmp = ref current;
                current = ref next;
                next = ref tmp;
            }

            Assert.Equal(2360, positions.Count);
        }

        private void Move(char c, ref IntPoint2 position)
        {
            position += c switch
            {
                '^' => IntPoint2.UnitY,
                'v' => -IntPoint2.UnitY,
                '>' => IntPoint2.UnitX,
                '<' => -IntPoint2.UnitX,
                _ => throw new InvalidOperationException()
            };
        }
    }
}
