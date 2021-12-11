using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace AdventOfCode._2017
{
    public class Day22
    {
        private readonly Dictionary<IntVec2, char> _map;
        private readonly IntVec2 _bounds;
        private IntVec2 _pos;
        private IntVec2 _dir;


        public Day22()
        {
            string[] lines = File.ReadAllLines("Inputs/Day22.txt");
            _bounds = new IntVec2(lines[0].Length, lines.Length);
            _map = new(_bounds.X * _bounds.Y);
            for (int j = 0; j < _bounds.Y; j++)
                for (int i = 0; i < _bounds.X; i++)
                    _map[new IntVec2(i, j)] = lines[j][i];

            _pos = _bounds / 2;
            _dir = -IntVec2.UnitY;
        }

        [Fact]
        public void Part1()
        {
            static IntVec2 Turn(char c, IntVec2 dir) => c is '#' ? dir.RotateLeft() : dir.RotateRight();

            static char Visit(char c) => c is '.' ? '#' : '.';

            int answer = Move(10_000, Turn, Visit);
            Assert.Equal(5565, answer);
        }

        [Fact]
        public void Part2()
        {
            static IntVec2 Turn(char c, IntVec2 dir) => c switch
            {
                '.' => dir.RotateRight(),
                '#' => dir.RotateLeft(),
                'F' => -dir,
                _ => dir
            };

            static char Visit(char c) => c switch
            {
                '.' => 'W',
                'W' => '#',
                '#' => 'F',
                'F' => '.',
                _ => throw new InvalidOperationException()
            };

            int answer = Move(10_000_000, Turn, Visit);
            Assert.Equal(2511978, answer);
        }

        private int Move(int iterations, Func<char, IntVec2, IntVec2> turn, Func<char, char> visit)
        {
            int infected = 0;
            for (int i = 0; i < iterations; i++)
            {
                char c;
                if (!_map.TryGetValue(_pos, out c))
                    c = '.';

                _dir = turn(c, _dir);

                c = visit(c);
                if (c is '#')
                    infected++;

                _map[_pos] = c;
                _pos += _dir;
            }

            return infected;
        }
    }
}
