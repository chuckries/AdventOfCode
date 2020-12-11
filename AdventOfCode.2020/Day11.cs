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
            int answer = Settle(4, FindSeatsSurrouding);

            Assert.Equal(2222, answer);
        }

        [Fact]
        public void Part2()
        {
            int answer = Settle(5, FindSeatsInSight);

            Assert.Equal(2032, answer);
        }

        private int Settle(int occupiedThreshold, Func<IntPoint2, IEnumerable<IntPoint2>> findAdjacents)
        {
            List<IntPoint2>[,] adjacents = new List<IntPoint2>[_bounds.X, _bounds.Y];
            for (int x = 0; x < _bounds.X; x++)
                for (int y = 0; y < _bounds.Y; y++)
                {
                    if (!IsSeat((x, y)))
                        continue;

                    List<IntPoint2> adjacent = new(8);
                    adjacent.AddRange(findAdjacents((x, y)));
                    adjacents[x, y] = adjacent;
                }

            while (Tick(occupiedThreshold, adjacents)) { }

            int total = 0;
            for (int x = 0; x < _bounds.X; x++)
                for (int y = 0; y < _bounds.Y; y++)
                    if (IsOccupied((x, y)))
                        total++;

            return total;
        }

        private bool Tick(int occupiedThreshold, List<IntPoint2>[,] adjacents)
        {
            bool changed = false;

            for (int x = 0; x < _bounds.X; x++)
                for (int y = 0; y < _bounds.Y; y++)
                {
                    if (!IsSeat((x, y)))
                        continue;

                    int occupied = adjacents[x, y].Count(IsOccupied);

                    char seat = _current[x, y];
                    switch (seat)
                    {
                        case 'L' when occupied is 0:
                            seat = '#';
                            changed = true;
                            break;
                        case '#' when occupied >= occupiedThreshold:
                            seat = 'L';
                            changed = true;
                            break;
                    }

                    _next[x, y] = seat;
                }

            var tmp = _current;
            _current = _next;
            _next = tmp;

            return changed;
        }

        private IEnumerable<IntPoint2> FindSeatsSurrouding(IntPoint2 seat) =>
            seat.Surrounding().Where(InBounds).Where(IsSeat);

        private IEnumerable<IntPoint2> FindSeatsInSight(IntPoint2 seat)
        {
            foreach (IntPoint2 dir in IntPoint2.Zero.Surrounding())
            {
                IntPoint2 cand = seat + dir;

                while (InBounds(cand))
                {
                    if (IsSeat(cand))
                    {
                        yield return cand;
                        break;
                    }

                    cand += dir;
                }
            }
        }

        private bool IsSeat(IntPoint2 p) =>
            _current[p.X, p.Y] != '.';

        private bool IsOccupied(IntPoint2 p) =>
            _current[p.X, p.Y] == '#';

        private bool InBounds(IntPoint2 p)
        {
            return p.X >= 0 && p.X < _bounds.X && p.Y >= 0 && p.Y < _bounds.Y;
        }
    }
}
