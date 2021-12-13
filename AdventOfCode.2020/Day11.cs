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
        IntVec2 _bounds;
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

        private int Settle(int occupiedThreshold, Func<IntVec2, IEnumerable<IntVec2>> findAdjacents)
        {
            List<IntVec2>[,] adjacents = new List<IntVec2>[_bounds.X, _bounds.Y];
            for (int x = 0; x < _bounds.X; x++)
                for (int y = 0; y < _bounds.Y; y++)
                {
                    if (!IsSeat((x, y)))
                        continue;

                    List<IntVec2> adjacent = new(8);
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

        private bool Tick(int occupiedThreshold, List<IntVec2>[,] adjacents)
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

            (_current, _next) = (_next, _current);

            return changed;
        }

        private IEnumerable<IntVec2> FindSeatsSurrouding(IntVec2 seat) =>
            seat.Surrounding().Where(InBounds).Where(IsSeat);

        private IEnumerable<IntVec2> FindSeatsInSight(IntVec2 seat)
        {
            foreach (IntVec2 dir in IntVec2.Zero.Surrounding())
            {
                IntVec2 cand = seat + dir;

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

        private bool IsSeat(IntVec2 p) =>
            _current[p.X, p.Y] != '.';

        private bool IsOccupied(IntVec2 p) =>
            _current[p.X, p.Y] == '#';

        private bool InBounds(IntVec2 p)
        {
            return p.X >= 0 && p.X < _bounds.X && p.Y >= 0 && p.Y < _bounds.Y;
        }
    }
}
