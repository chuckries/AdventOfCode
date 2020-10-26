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
        private readonly ILookup<string, string> _lookup;
        private readonly string _initial;

        public Day19()
        {
            string[] input = File.ReadAllLines("Inputs/Day19.txt");
            _lookup = input[..^2]
                .Select(s => s.Split(' '))
                .ToLookup(t => t[0], t => t[2]);

            _initial = input[^1];
        }

        [Fact]
        public void Part1()
        {
            HashSet<string> answers = new HashSet<string>(ComposeAll(_initial, _lookup));
            int answer = answers.Count;

            Assert.Equal(518, answer);
        }

        [Fact]
        public void Part2()
        {
            Dictionary<string, string> inverseTransforms = File
                .ReadAllLines("Inputs/Day19.txt")
                .SkipLast(2)
                .Select(s => s.Split(' '))
                .ToDictionary(t => t[2], t => t[0]);

            int answer = Decompose(_initial, inverseTransforms, "e");
            Assert.Equal(200, answer);
        }

        private static IEnumerable<string> ComposeAll(string input, ILookup<string, string> lookup)
        {
            foreach (var transform in lookup)
            {
                foreach (string transformed in Compose(input, transform))
                    yield return transformed;
            }
        }

        private static IEnumerable<string> Compose(string input, IGrouping<string, string> transform)
        {
            StringBuilder sb = new StringBuilder(input.Length * 2);
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

        private static IEnumerable<string> DecomposeAll(string input, Dictionary<string, string> transforms)
        {
            StringBuilder sb = new StringBuilder(input.Length);

            foreach ((string key, string value) in transforms)
            {
                int index = 0;
                while (index != -1)
                {
                    index = input.IndexOf(key, index);
                    if (index != -1)
                    {
                        sb.Append(input.AsSpan(0, index));
                        sb.Append(value);
                        index += key.Length;
                        sb.Append(input.AsSpan(index));
                        yield return sb.ToString();
                        sb.Clear();
                    }
                }
            }
        }

        private static int Decompose(string input, Dictionary<string, string> transforms, string target)
        {
            var result = Recurse(input, transforms, target, 0);
            if (!result.found) throw new InvalidOperationException();
            return result.level;
        }

        private static (bool found, int level) Recurse(string input, Dictionary<string, string> transforms, string target, int level)
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
                        var result = Recurse(sb.ToString(), transforms, target, level + 1);
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
