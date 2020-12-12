using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using AdventOfCode.Common;

using Xunit;

namespace AdventOfCode._2015
{
    public class Day18
    {
        IntVec2 _bounds;
        char[,] _current;
        char[,] _next;

        public Day18()
        {
            string[] lines = File.ReadAllLines("Inputs/Day18.txt");
            _bounds = (lines[0].Length, lines.Length);
            _current = new char[_bounds.X, _bounds.Y];
            _next = (char[,])_current.Clone();

            for (int j = 0; j < _bounds.Y; j++)
                for (int i = 0; i < _bounds.X; i++)
                    _current[i, j] = lines[j][i];
        }

        [Fact]
        public void Part1()
        {
            for (int i = 0; i < 100; i++)
                Tick();

            int answer = CountOns();

            Assert.Equal(821, answer);
        }

        [Fact]
        public void Part2()
        {
            for (int i = 0; i < 100; i++)
            {
                TurnOnCorners();
                Tick();
            }
            TurnOnCorners();

            int answer = CountOns();

            Assert.Equal(886, answer);
        }

        private void TurnOnCorners()
        {
            _current[0, 0] = '#';
            _current[0, _bounds.Y - 1] = '#';
            _current[_bounds.X - 1, 0] = '#';
            _current[_bounds.X - 1, _bounds.Y - 1] = '#';
        }

        private int CountOns()
        {
            int total = 0;
            for (int i = 0; i < _bounds.X; i++)
                for (int j = 0; j < _bounds.Y; j++)
                    if (_current[i, j] == '#')
                        total++;

            return total;
        }

        private void Tick()
        {
            for (int i = 0; i < _bounds.X; i++)
            {
                for (int j = 0; j < _bounds.Y; j++)
                {
                    int Ons = 0;
                    foreach ((int u, int v) in (new IntVec2(i, j)).Surrounding())
                    {
                        if (u < 0 || u >= _bounds.X || v < 0 || v >= _bounds.Y)
                            continue;

                        if (_current[u, v] == '#') Ons++;
                    }

                    _next[i, j] = _current[i, j] switch
                    {
                        '#' when Ons == 2 || Ons == 3 => '#',
                        '.' when Ons == 3 => '#',
                        _ => '.'
                    };
                }
            }

            char[,] tmp = _current;
            _current = _next;
            _next = tmp;
        }
    }
}
