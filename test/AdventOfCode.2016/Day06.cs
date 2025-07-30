using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace AdventOfCode._2016
{
    public class Day06
    {
        string[] _input;

        public Day06()
        {
            _input = File.ReadAllLines("Inputs/Day06.txt");
        }

        [Fact]
        public void Part1()
        {
            string answer = Reduce(occurences =>
            {
                char result = default;
                int max = 0;

                foreach (var kvp in occurences)
                {
                    if (kvp.Value > max)
                    {
                        max = kvp.Value;
                        result = kvp.Key;
                    }
                }

                return result;
            });

            Assert.Equal("cyxeoccr", answer);
        }

        [Fact]
        public void Part2()
        {
            string answer = Reduce(occurences =>
            {
                char result = default;
                int min = int.MaxValue;

                foreach (var kvp in occurences)
                {
                    if (kvp.Value < min)
                    {
                        min = kvp.Value;
                        result = kvp.Key;
                    }
                }

                return result;
            });

            Assert.Equal("batwpask", answer);
        }

        private string Reduce(Func<Dictionary<char, int>, char> reduceOccurences)
        {
            int size = _input[0].Length;
            Dictionary<char, int>[] occurences = new Dictionary<char, int>[size];
            for (int i = 0; i < size; i++)
                occurences[i] = new Dictionary<char, int>();

            foreach (string s in _input)
            {
                for (int i = 0; i < size; i++)
                {
                    char c = s[i];
                    if (!occurences[i].ContainsKey(c))
                    {
                        occurences[i][c] = 1;
                    }
                    else
                    {
                        occurences[i][c]++;
                    }
                }
            }

            char[] result = new char[size];
            for (int i = 0; i < size; i++)
            {
                result[i] = reduceOccurences(occurences[i]);
            }

            return new string(result);
        }
    }
}
