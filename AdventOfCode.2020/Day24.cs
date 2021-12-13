using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AdventOfCode.Common;

using Xunit;

namespace AdventOfCode._2020
{
    public class Day24
    {
        Dictionary<IntVec2, bool> _tiles;

        public Day24()
        {
            _tiles = new();

            foreach (string s in File.ReadAllLines("Inputs/Day24.txt"))
            {
                IntVec2 current = (0, 0);
                int index = 0;
                while (index < s.Length)
                {
                    if (s[index] == 'e')
                    {
                        ++index;
                        current += (2, 0);
                    }
                    else if (s[index] == 's')
                    {
                        if (s[index + 1] == 'e')
                        {
                            current += (1, -1);
                        }
                        else if (s[index + 1] == 'w')
                        {
                            current += (-1, -1);
                        }
                        index += 2;
                    }
                    else if (s[index] == 'w')
                    {
                        ++index;
                        current += (-2, 0);
                    }
                    else if (s[index] == 'n')
                    {
                        if (s[index + 1] == 'e')
                        {
                            current += (1, 1);
                        }
                        else if (s[index + 1] == 'w')
                        {
                            current += (-1, 1);
                        }
                        index += 2;
                    }
                }

                if (_tiles.TryGetValue(current, out bool tile))
                {
                    _tiles[current] = !tile;
                }
                else
                    _tiles[current] = true;
            }
        }

        [Fact]
        public void Part1()
        {
            int answer = _tiles.Values.Count(t => t);
            Assert.Equal(356, answer);
        }

        [Fact]
        public void Part2()
        {
            Dictionary<IntVec2, (bool isBlack, bool isSurrounded)> current = new(_tiles.Count * 2);
            foreach ((IntVec2 coord, bool isBlack) in _tiles)
                current.Add(coord, (isBlack, true));
            foreach (IntVec2 coord in current.Keys.ToList())
                foreach (IntVec2 adj in Adjacent(coord))
                    current.TryAdd(adj, (false, false));

            Dictionary<IntVec2, (bool isBlack, bool isSurrounded)> next = new(current.Count);
            List<IntVec2> toAdd = new(current.Count);

            for (int i = 0; i < 100; i++)
            {
                foreach ((IntVec2 coord, (bool isBlack, bool isSurrounded)) in current)
                {
                    int adjBlack = Adjacent(coord).Count(adj => current.TryGetValue(adj, out (bool isBlack, bool isSurrouned) adjState) && adjState.isBlack);
                    bool isNextBlack = isBlack ? !(adjBlack is 0 or > 2) : adjBlack is 2;
                    next[coord] = (isNextBlack, isNextBlack || isSurrounded);
                    if (isNextBlack && !isSurrounded)
                        toAdd.Add(coord);
                }

                foreach (IntVec2 coord in toAdd)
                    foreach (IntVec2 adj in Adjacent(coord))
                        next.TryAdd(adj, (false, false));
                toAdd.Clear();

                (current, next) = (next, current);
            }

            int answer = current.Values.Count(t => t.isBlack);
            Assert.Equal(3887, answer);
        }

        private IEnumerable<IntVec2> Adjacent(IntVec2 tile)
        {
            yield return new IntVec2(tile.X - 2, tile.Y);       // w
            yield return new IntVec2(tile.X - 1, tile.Y + 1);   // nw
            yield return new IntVec2(tile.X + 1, tile.Y + 1);   // ne
            yield return new IntVec2(tile.X + 2, tile.Y);       // e
            yield return new IntVec2(tile.X + 1, tile.Y - 1);   // se
            yield return new IntVec2(tile.X - 1, tile.Y - 1);   // sw
        }
    }
}
