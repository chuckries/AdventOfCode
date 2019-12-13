using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace AdventOfCode._2018
{
    public class Day18
    {
        enum Cell : int
        {
            Open = '.',
            Tree = '|',
            Lumber = '#'
        }

        IntPoint2 _bounds;
        Cell[][,] _map;
        Cell[,] _current;
        Cell[,] _next;

        public Day18()
        {
            string[] input = File.ReadAllLines("Inputs/Day18.txt").ToArray();
            _bounds = new IntPoint2(input[0].Length, input.Length);
            _map = new Cell[][,] { new Cell[_bounds.X, _bounds.Y], new Cell[_bounds.X, _bounds.Y] };
            _current = _map[0];
            _next = _map[1];

            int j = 0;
            foreach (string s in File.ReadAllLines("Inputs/Day18.txt"))
            {
                int i = 0;
                foreach (char c in s)
                {
                    _current[j, i++] = (Cell)c;
                }
                j++;
            }
        }

        [Fact]
        public void Part1()
        {
            for (int i = 0; i < 10; i++)
                Tick();

            int lumber = 0;
            int wood = 0;
            for (int i = 0; i < _bounds.X; i++)
            {
                for (int j = 0; j < _bounds.Y; j++)
                {
                    if (_current[i, j] == Cell.Lumber) lumber++;
                    else if (_current[i, j] == Cell.Tree) wood++;
                }
            }

            int answer = lumber * wood;
            Assert.Equal(620624, answer);
        }

        [Fact]
        public void Part2()
        {
            List<string> states = new List<string>(500);
            StringBuilder sb = new StringBuilder(60 * 60);

            int periodStart = 0;
            int periodLength = 0;
            for (; ;)
            {
                for (int i = 0; i < _bounds.X; i++)
                {
                    for (int j = 0; j < _bounds.Y; j++)
                    {
                        sb.Append((char)_current[i, j]);
                    }
                }

                string current = sb.ToString();
                int index = states.IndexOf(current);
                if (index == -1)
                {
                    states.Add(current);
                }
                else
                {
                    periodStart = index;
                    periodLength = states.Count - periodStart;
                    break;
                }

                Tick();
                sb.Clear();
            }

            const int target = 1000000000;

            int periodIndex = (target - periodStart) % periodLength;
            string correctState = states[periodStart + periodIndex];

            int lumber = 0;
            int wood = 0;
            foreach (Cell c in correctState)
            {
                if (c == Cell.Lumber) lumber++;
                else if (c == Cell.Tree) wood++;
            }

            int answer = lumber * wood;
            Assert.Equal(169234, answer);
        }

        private void Tick()
        {
            for (int x = 0; x < _bounds.X; x++)
            {
                for (int y = 0; y < _bounds.Y; y++)
                {
                    int adjacentTrees = 0;
                    int adjacentLumber = 0;

                    for (int u = x - 1; u <= x + 1; u++)
                    {
                        if (u < 0 || u >= _bounds.X)
                            continue;

                        for (int v = y - 1; v <= y + 1; v++)
                        {
                            if (v < 0 || v >= _bounds.Y || (u == x && v ==y))
                                continue;

                            if (_current[u, v] == Cell.Tree) adjacentTrees++;
                            else if (_current[u, v] == Cell.Lumber) adjacentLumber++;
                        }
                    }

                    Cell current = _current[x, y];
                    Cell next;

                    if (current == Cell.Open && adjacentTrees >= 3)
                        next = Cell.Tree;
                    else if (current == Cell.Tree && adjacentLumber >= 3)
                        next = Cell.Lumber;
                    else if (current == Cell.Lumber && !(adjacentLumber >= 1 && adjacentTrees >= 1))
                        next = Cell.Open;
                    else
                        next = current;

                    _next[x, y] = next;
                }
            }

            Cell[,] temp = _current;
            _current = _next;
            _next = temp;
        }
    }
}
