using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using AdventOfCode.Common;

using Xunit;

namespace AdventOfCode._2015
{
    public class Day13
    {
        private class Graph
        {
            private Dictionary<string, int> _nodeIndices;
            private List<List<int>> _edges;

            public Graph(string[] input)
            {
                _nodeIndices = new Dictionary<string, int>();
                _edges = new List<List<int>>();
                BuildGraph(input);
            }

            public int Max()
            {
                return _nodeIndices.Values.FullPermutations().Select(o => Evaluate(o.ToArray())).Max();
            }

            public void AddMe()
            {
                int myIndex = GetNodeIndex("me");
                for (int i = 0; i < myIndex; i++)
                {
                    AddEdge(myIndex, i, 0);
                    AddEdge(i, myIndex, 0);
                }
            }

            private int Evaluate(int[] order)
            {
                int total = 0;
                for (int i = 0; i < order.Length; i++)
                {
                    total += (_edges[order[i]][order[Mod(i - 1)]] + _edges[order[i]][order[Mod(i + 1)]]);
                }

                return total;

                int Mod(int index)
                {
                    return (index % order.Length + order.Length) % order.Length;
                }
            }

            private void BuildGraph(string[] input)
            {
                foreach (string s in input)
                {
                    Match match = s_Regex.Match(s);

                    int sourceIndex = GetNodeIndex(match.Groups["source"].Value);
                    int sinkIndex = GetNodeIndex(match.Groups["sink"].Value);
                    int value = int.Parse(match.Groups["val"].Value);
                    if (match.Groups["sign"].Value == "lose")
                        value = -value;

                    AddEdge(sourceIndex, sinkIndex, value);
                }
            }

            private void AddEdge(int sourceIndex, int sinkIndex, int value)
            {
                if (sourceIndex >= _edges.Count)
                    for (int i = _edges.Count; i <= sourceIndex; i++)
                        _edges.Add(new List<int>());

                List<int> inner = _edges[sourceIndex];

                if (sinkIndex >= inner.Count)
                    for (int i = inner.Count; i <= sinkIndex; i++)
                        inner.Add(-1);

                inner[sinkIndex] = value;
            }

            private int GetNodeIndex(string name)
            {
                if (!_nodeIndices.TryGetValue(name, out int index))
                {
                    index = _nodeIndices.Count;
                    _nodeIndices[name] = index;
                }

                return index;
            }

            private static Regex s_Regex = new Regex(
                @"^(?'source'[a-zA-Z]+) would (?'sign'gain|lose) (?'val'\d+) happiness units by sitting next to (?'sink'[a-zA-Z]+)\.$",
                RegexOptions.Compiled);
        }

        Graph _graph;

        public Day13()
        {
            _graph = new Graph(File.ReadAllLines("Inputs/Day13.txt"));
        }

        [Fact]
        public void Part1()
        {
            int answer = _graph.Max();
            Assert.Equal(664, answer);
        }

        [Fact]
        public void Part2()
        {
            _graph.AddMe();
            int answer = _graph.Max();
            Assert.Equal(640, answer);
        }
    }
}
