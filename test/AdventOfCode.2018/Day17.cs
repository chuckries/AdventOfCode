using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AdventOfCode._2018
{
    public class Day17
    {
        const char Wet = '|';
        const char Water = '~';
        const char Wall = '#';
        const char Space = ' ';

        private readonly Dictionary<IntVec2, char> _map;
        private readonly IntVec2 _start;
        private readonly int _maxY;

        private char Map(IntVec2 p) => _map.GetValueOrDefault(p, ' ');

        public Day17()
        {
            _map = new();
            int minY = int.MaxValue;
            int maxY = int.MinValue;
            foreach (string line in File.ReadLines("Inputs/Day17.txt"))
            {
                string[] tok = line.Split(new[] { "=", ",", ".." }, StringSplitOptions.TrimEntries);
                int a = int.Parse(tok[1]);
                int b = int.Parse(tok[3]);
                int c = int.Parse(tok[4]);
                if (tok[0] == "x")
                {
                    if (b < minY)
                        minY = b;
                    if (c > maxY)
                        maxY = c;

                    for (int i = b; i <= c; i++)
                    {
                        _map[new IntVec2(a, i)] = Wall;
                    }
                }
                else
                    for (int i = b; i <= c; i++)
                        _map[new IntVec2(i, a)] = Wall;
            }

            _start = new IntVec2(500, minY);
            _maxY = maxY;

            Flow();
        }

        [Fact]
        public void Part1()
        {
            int answer = _map.Values.Count(c => c is Wet or Water);
            Assert.Equal(32552, answer);
        }

        [Fact]
        public void Part2()
        {
            int answer = _map.Values.Count(c => c is Water);
            Assert.Equal(26405, answer);
        }

        private void Flow()
        {
            Queue<IntVec2> heads = new();
            heads.Enqueue(_start);

            while (heads.Count > 0)
            {
                IntVec2 pCurrent = heads.Dequeue();
                while (true)
                {
                    if (pCurrent.Y > _maxY)
                        break; // past bounds

                    // what are we?
                    char cCurrent = Map(pCurrent);
                    if (cCurrent is Space)
                    {
                        // we are empty, fill us in and continue downward flow
                        Fill(Wet, pCurrent);
                        pCurrent += IntVec2.UnitY;
                    }
                    else if (cCurrent is Wall or Water)
                    {
                        // we found wall or water, climb back up filling until we are unbounded
                        while (true)
                        {
                            pCurrent -= IntVec2.UnitY;

                            if (Map(pCurrent) is Water)
                                continue;

                            var left = Search(pCurrent, -IntVec2.UnitX);
                            var right = Search(pCurrent, IntVec2.UnitX);

                            if (left.foundWall && right.foundWall)
                            {
                                // we are bounded, all this is water
                                Fill(Water, left.p.X, right.p.X, pCurrent.Y);
                            }
                            else
                            {
                                // we are unbounded, all this is wet and we have new flow heads
                                Fill(Wet, left.p.X, right.p.X, pCurrent.Y);

                                if (!left.foundWall)
                                    heads.Enqueue(new IntVec2(left.p.X - 1, pCurrent.Y));

                                if (!right.foundWall)
                                    heads.Enqueue(new IntVec2(right.p.X + 1, pCurrent.Y));

                                break;
                            }
                        }
                    }
                    else if (cCurrent is Wet)
                    {
                        // we have hit an existing flow, stop this flow now
                        break;
                    }
                    else
                        throw new InvalidOperationException();
                }
            }
        }

        private (IntVec2 p, bool foundWall) Search(IntVec2 p, IntVec2 dir)
        {
            while (true)
            {
                IntVec2 pNext = p + dir;
                char cNext = Map(pNext);

                if (cNext is Wall)
                    return (p, true);
                else
                {
                    IntVec2 pNextBelow = pNext + IntVec2.UnitY;
                    char cNextBelow = Map(pNextBelow);

                    if (cNextBelow is Water or Wall)
                        p = pNext;
                    else
                        return (p, false);
                }
            }
        }

        private void Fill(char type, int x0, int x1, int y)
        {
            for (int i = x0; i <= x1; i++)
                Fill(type, new IntVec2(i, y));
        }

        private void Fill(char type, IntVec2 p)
        {
            _map[p] = type;
        }
    }
}
