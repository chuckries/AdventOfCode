﻿using System.Runtime.ExceptionServices;

namespace AdventOfCode._2021
{
    public class Day14
    {
        private readonly int[] _start;
        private readonly List<List<int>> _rules;
        private readonly int _size;

        public Day14()
        {
            string[] lines = File.ReadAllLines("Inputs/Day14.txt");

            int[] ids = new int[26];
            for (int i = 0; i < ids.Length; i++)
                ids[i] = -1;
            int nextId = 0;

            _start = lines[0].Select(c => GetId(c)).ToArray();

            _rules = new(26);
            foreach (string[] tok in lines.Skip(2).Select(l => l.Split(" -> ")))
            {
                SetRule(GetId(tok[0][0]), GetId(tok[0][1]), GetId(tok[1][0]));
            }

            _size = nextId;

            int GetId(char c)
            {
                int idx = c - 'A';
                int id = ids[idx];
                if (id == -1)
                    ids[idx] = id = nextId++;
                return id;
            }

            void SetRule(int x, int y, int z)
            {
                while (x >= _rules.Count)
                    _rules.Add(new(26));

                List<int> list = _rules[x];

                while (y >= list.Count)
                    list.Add(-1);

                list[y] = z;
            }
        }

        [Fact]
        public void Part1()
        {
            long answer = Count(10);
            Assert.Equal(2587, answer);
        }

        [Fact]
        public void Part2()
        {
            long answer = Count(40);
            Assert.Equal(3318837563123, answer);
        }

        public long Count(int iterations)
        {
            long[,] pairs = new long[_size, _size];
            long[] counts = new long[_size];

            for (int i = 0; i < _start.Length - 1; i++)
            {
                counts[_start[i]]++;
                pairs[_start[i], _start[i + 1]]++;
            }
            counts[_start[^1]]++;

            long current;
            int next;
            for (int iteration = 0; iteration < iterations; iteration++)
            {
                long[,] newPairs = new long[_size, _size];
                for (int i = 0; i < _size; i++)
                    for (int j = 0; j < _size; j++)
                    {
                        current = pairs[i, j];
                        if (current != 0)
                        {
                            next = _rules[i][j];
                            counts[next] += current;
                            newPairs[i, next] += current;
                            newPairs[next, j] += current;
                        }
                    }

                pairs = newPairs;
            }

            long max = counts[0];
            long min = counts[0];
            for (int i = 1; i < counts.Length; i++)
            {
                long count = counts[i];
                if (count > max)
                    max = count;
                if (count < min)
                    min = count;
            }

            return max - min;
        }
    }
}
