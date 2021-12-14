using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode._2021
{
    public class Day14
    {
        readonly int[] _start;
        Dictionary<IntVec2, int> _rules;

        public Day14()
        {
            string[] lines = File.ReadAllLines("Inputs/Day14.txt");

            _start = lines[0].Select(c => c - 'A').ToArray();
            _rules = lines[2..].Select(l => l.Split(" -> ")).ToDictionary(tok => new IntVec2(tok[0][0] - 'A', tok[0][1] -'A'), tok => tok[1][0] - 'A');
        }

        [Fact]
        public void Part1()
        {
            int answer = Count(10);
            Assert.Equal(2587, answer);
        }

        [Fact]
        public void Part2()
        {
            long answer = CountDynamic(40);
            Assert.Equal(3318837563123, answer);
        }

        private int Count(int depth)
        {
            int[] counts = new int[26];

            foreach (int i in _start)
            {
                counts[i]++;
            }

            for (int i = 0; i < _start.Length - 1; i++)
                Recurse(0, new IntVec2(_start[i], _start[i + 1]), counts);

            void Recurse(int currentDepth, IntVec2 current, int[] counts)
            {
                if (currentDepth == depth)
                    return;

                int next = _rules[current];
                counts[next]++;

                Recurse(currentDepth + 1, new IntVec2(current.X, next), counts);
                Recurse(currentDepth + 1, new IntVec2(next, current.Y), counts);
            }

            return counts.Where(c => c != 0).Max() - counts.Where(c => c != 0).Min();
        }

        private long CountDynamic(int depth)
        {
            Dictionary<IntVec3, long[]> levelCounts = new();

            long[] finalCounts = new long[26];
            for (int i = 0; i < _start.Length; i++)
                finalCounts[_start[i]]++;

            for (int i = 0; i < _start.Length - 1; i++)
            {
                long[] tmpCounts = Recurse(0, new IntVec2(_start[i], _start[i + 1]));
                for (int j = 0; j < 26; j++)
                    finalCounts[j] += tmpCounts[j];
            }

            return finalCounts.Where(c => c != 0).Max() - finalCounts.Where(c => c != 0).Min();

            long[] Recurse(int currentDepth, IntVec2 current)
            {
                if (currentDepth == depth)
                    return new long[26];

                if (levelCounts.TryGetValue((current.X, current.Y, currentDepth), out long[] counts))
                    return counts;

                int next = _rules[current];

                IntVec2 left = new IntVec2(current.X, next);
                IntVec2 right = new IntVec2(next, current.Y);

                long[] countsLeft = Recurse(currentDepth + 1, left);
                long[] countsRight = Recurse(currentDepth + 1, right);

                counts = new long[26];
                counts[next]++;

                for (int i = 0; i < 26; i++)
                {
                    counts[i] += countsLeft[i] + countsRight[i];
                }

                levelCounts[new IntVec3(current.X, current.Y, currentDepth)] = counts;
                return counts;
            }

        }
    }
}
