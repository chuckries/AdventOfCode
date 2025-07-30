using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using AdventOfCode.Common;

using Xunit;

namespace AdventOfCode._2015
{
    public class Day19
    {
        private readonly string[] _rawTransforms;
        private readonly string _initial;

        public Day19()
        {
            string[] input = File.ReadAllLines("Inputs/Day19.txt");
            _rawTransforms = input[..^2];
            _initial = input[^1];
        }

        [Fact]
        public void Part1()
        {
            ILookup<string, string> lookup = _rawTransforms
                .Select(s => s.Split(' '))
                .ToLookup(t => t[0], t => t[2]);
            HashSet<string> answers = new HashSet<string>(ComposeAll(_initial, lookup));
            int answer = answers.Count;

            Assert.Equal(518, answer);
        }

        [Fact]
        public void Part2()
        {
            // I was able to get the right answer but I don't think I fully solved the problem.
            // The question asks for min number of steps, and my solution reports the number of steps
            // for the very first solution it finds.
            // According to this thread (https://www.reddit.com/r/adventofcode/comments/3xflz8/day_19_solutions/)
            // there is only one viable solution based on the nature of the input

            Dictionary<string, string> inverseTransforms = _rawTransforms
                .Select(s => s.Split(' '))
                .ToDictionary(t => t[2], t => t[0]);

            int answer = Decompose(_initial, inverseTransforms, "e");
            Assert.Equal(200, answer);
        }

        private static IEnumerable<string> ComposeAll(string input, ILookup<string, string> lookup)
        {
            StringBuilder sb = new StringBuilder(input.Length * 2);
            foreach (var transform in lookup)
            {
                string key = transform.Key;
                int index = 0;
                while (index != -1)
                {
                    index = input.IndexOf(key, index);
                    if (index != -1)
                    {
                        int start = index;
                        index += key.Length;
                        foreach (string value in transform)
                        {
                            sb.Append(input.AsSpan(0, start));
                            sb.Append(value);
                            sb.Append(input.AsSpan(index));
                            yield return sb.ToString();
                            sb.Clear();
                        }
                    }
                }
            }

        }

        private static int Decompose(string input, Dictionary<string, string> transforms, string target)
        {
            var result = RecursiveDecompose(input, transforms, target, 0);
            if (!result.found) throw new InvalidOperationException();
            return result.level;
        }

        private static (bool found, int level) RecursiveDecompose(string input, Dictionary<string, string> transforms, string target, int level)
        {
            if (input == target)
            {
                return (true, level);
            }

            StringBuilder sb = new StringBuilder(input.Length);
            foreach (var kvp in transforms)
            {
                int index = 0;
                while (index != -1)
                {
                    index = input.IndexOf(kvp.Key);
                    if (index != -1)
                    {
                        sb.Append(input.AsSpan(0, index));
                        sb.Append(kvp.Value);
                        index += kvp.Key.Length;
                        sb.Append(input.AsSpan(index));
                        var result = RecursiveDecompose(sb.ToString(), transforms, target, level + 1);
                        if (result.found)
                            return result; 
                        sb.Clear();
                    }
                }
            }

            return (false, 0);
        }
    }
}
