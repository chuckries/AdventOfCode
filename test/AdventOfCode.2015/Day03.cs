using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

using AdventOfCode.Common;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.ObjectModel;

namespace AdventOfCode._2015
{
    public class Day03
    {
        string _input;

        public Day03()
        {
            _input = File.ReadAllText("Inputs/Day03.txt");
        }

        [Fact]
        public void Part1()
        {
            IntVec2 current = default;
            HashSet<IntVec2> positions = new HashSet<IntVec2>();
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
            IntVec2 current1 = default;
            IntVec2 current2 = default;
            HashSet<IntVec2> positions = new HashSet<IntVec2>();
            positions.Add(current1);

            ref IntVec2 current = ref current1;
            ref IntVec2 next = ref current2;

            for (int i = 0; i < _input.Length; i++)
            {
                Move(_input[i], ref current);

                positions.Add(current);

                (current, next) = (next, current);
            }

            Assert.Equal(2360, positions.Count);
        }

        private void Move(char c, ref IntVec2 position)
        {
            position += c switch
            {
                '^' => IntVec2.UnitY,
                'v' => -IntVec2.UnitY,
                '>' => IntVec2.UnitX,
                '<' => -IntVec2.UnitX,
                _ => throw new InvalidOperationException()
            };
        }
    }
}
