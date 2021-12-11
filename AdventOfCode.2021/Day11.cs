using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode._2021
{
    public class Day11
    {
        int[,] _map;
        IntVec2 _bounds;

        public Day11()
        {
            string[] lines = File.ReadAllLines("Inputs/Day11.txt");
            _bounds = new IntVec2(lines[0].Length, lines.Length);
            _map = new int[_bounds.X, _bounds.Y];

            for (int i = 0; i < _bounds.X; i++)
                for (int j = 0; j < _bounds.Y; j++)
                    _map[i, j] = lines[j][i] - '0';
        }

        [Fact]
        public void Part1()
        {
            Queue<IntVec2> toFlash = new();
            List<IntVec2> flashed = new();
            int flashes = 0;

            for (int iterations = 0; iterations < 100; iterations++)
            {
                for (int i = 0; i < _bounds.X; i++)
                    for (int j = 0; j < _bounds.Y; j++)
                    {
                        _map[i, j]++;
                        if (_map[i, j] == 10)
                            toFlash.Enqueue(new IntVec2(i, j));
                    }

                while (toFlash.Count > 0)
                {
                    IntVec2 current = toFlash.Dequeue();
                    flashed.Add(current);

                    foreach (IntVec2 adj in current.Surrounding(_bounds))
                    {
                        _map[adj.X, adj.Y]++;
                        if (_map[adj.X, adj.Y] == 10)
                            toFlash.Enqueue(adj);
                    }
                }

                flashes += flashed.Count;
                foreach (IntVec2 toZero in flashed)
                    _map[toZero.X, toZero.Y] = 0;

                flashed.Clear();
            }

            Assert.Equal(1591, flashes);
        }

        [Fact]
        public void Part2()
        {
            Queue<IntVec2> toFlash = new();
            List<IntVec2> flashed = new();

            int iteration = 0;
            while (true)
            {
                iteration++;
                for (int i = 0; i < _bounds.X; i++)
                    for (int j = 0; j < _bounds.Y; j++)
                    {
                        _map[i, j]++;
                        if (_map[i, j] == 10)
                            toFlash.Enqueue(new IntVec2(i, j));
                    }

                while (toFlash.Count > 0)
                {
                    IntVec2 current = toFlash.Dequeue();
                    flashed.Add(current);

                    foreach (IntVec2 adj in current.Surrounding(_bounds))
                    {
                        _map[adj.X, adj.Y]++;
                        if (_map[adj.X, adj.Y] == 10)
                            toFlash.Enqueue(adj);
                    }
                }

                if (flashed.Count == _bounds.X * _bounds.Y)
                    break;

                foreach (IntVec2 toZero in flashed)
                    _map[toZero.X, toZero.Y] = 0;

                flashed.Clear();
            }

            Assert.Equal(0, iteration);
        }
    }
}
