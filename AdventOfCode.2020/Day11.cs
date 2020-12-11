using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xunit;

using AdventOfCode.Common;

namespace AdventOfCode._2020
{
    public class Day11
    {
        IntPoint2 _bounds;
        char[,] _current;
        char[,] _next;

        public Day11()
        {
            string[] input = File.ReadAllLines("Inputs/Day11.txt");
            _bounds = (input[0].Length, input.Length);
            _current = new char[_bounds.X, _bounds.Y];
            for (int j = 0; j < input.Length; j++)
                for (int i = 0; i < input[j].Length; i++)
                    _current[i, j] = input[j][i];
            _next = (char[,])_current.Clone();
        }

        [Fact]
        public void Part1()
        {
            int answer = Settle(4, (in IntPoint2 current) =>
            {
                int occupied = 0;
                foreach ((int u, int v) in current.Surrounding())
                {
                    if (!InBounds((u, v)))
                        continue;

                    if (_current[u, v] == '#') 
                        occupied++;
                }
                return occupied;
            });

            Assert.Equal(2222, answer);
        }

        [Fact]
        public void Part2()
        {
            List<IntPoint2>[,] inSights = new List<IntPoint2>[_bounds.X, _bounds.Y];
            for (int x = 0; x < _bounds.X; x++)
                for (int y = 0; y < _bounds.Y; y++)
                {
                    if (_current[x, y] == '.')
                        continue;

                    List<IntPoint2> inSight = new List<IntPoint2>(8);
                    inSight.AddRange(FindSeatsInSight((x, y)));
                    inSights[x, y] = inSight;
                }

            int answer = Settle(5, (in IntPoint2 current) =>
            {
                return inSights[current.X, current.Y].Count(adj => _current[adj.X, adj.Y] == '#');
            });

            Assert.Equal(2032, answer);
        }

        private delegate int Kernel(in IntPoint2 current);

        private int Settle(int occupiedThreshold, Kernel kernel)
        {
            while (Tick(occupiedThreshold, kernel)) { }

            int total = 0;
            for (int x = 0; x < _bounds.X; x++)
                for (int y = 0; y < _bounds.Y; y++)
                    if (_current[x, y] == '#')
                        total++;

            return total;
        }

        private bool Tick(int occupiedThreshold, Kernel kernel)
        {
            bool changed = false;

            for (int x = 0; x < _bounds.X; x++)
            {
                for (int y = 0; y < _bounds.Y; y++)
                {
                    char c = _current[x, y];
                    if (c == '.')
                        continue;

                    int occupied = kernel((x, y));

                    char next = c;
                    switch (_current[x, y])
                    {
                        case 'L' when occupied is 0:
                            next = '#';
                            changed = true;
                            break;
                        case '#' when occupied >= occupiedThreshold:
                            next = 'L';
                            changed = true;
                            break;
                    }

                    _next[x, y] = next;
                }
            }

            var tmp = _current;
            _current = _next;
            _next = tmp;

            return changed;
        }

        private IEnumerable<IntPoint2> FindSeatsInSight(IntPoint2 seat)
        {
            foreach (IntPoint2 dir in IntPoint2.Zero.Surrounding())
            {
                IntPoint2 cand = seat + dir;

                while (true)
                {
                    if (!InBounds(cand))
                        break;

                    if (_current[cand.X, cand.Y] != '.')
                    {
                        yield return cand;
                        break;
                    }

                    cand += dir;
                }
            }
        }

        private bool InBounds(in IntPoint2 p)
        {
            return p.X >= 0 && p.X < _bounds.X && p.Y >= 0 && p.Y < _bounds.Y;
        }
    }
}
