using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

using AdventOfCode.Common;

namespace AdventOfCode._2020
{
    public class Day03
    {
        string[] _map;

        public Day03()
        {
            _map = File.ReadAllLines("Inputs/Day03.txt");
        }

        [Fact]
        public void Part1()
        {
            int answer = CountTrees((3, 1));

            Assert.Equal(209, answer);
        }

        [Fact]
        public void Part2()
        {
            int answer = new IntPoint2[]
            {
                (1, 1),
                (3, 1),
                (5, 1),
                (7, 1),
                (1, 2)
            }
            .Select(CountTrees)
            .Aggregate(1, (x, y) => x * y);

            Assert.Equal(1574890240, answer);
        }

        private int CountTrees(IntPoint2 slope)
        {
            IntPoint2 pos = IntPoint2.Zero;

            int trees = 0;
            while (pos.Y < _map.Length)
            {
                if (_map[pos.Y][pos.X] == '#')
                    ++trees;

                pos = ((pos.X + slope.X) % _map[0].Length, pos.Y + slope.Y);
            }

            return trees;
        }
    }
}
