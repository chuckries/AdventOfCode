using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace AdventOfCode._2015
{
    public class Day09
    {
        private class Graph
        {
            private Dictionary<string, int> _nodeIndices = new Dictionary<string, int>();
            private List<List<int>> _edges = new List<List<int>>();

            public Graph(string [] input)
            {
                foreach (string s in input)
                {
                    Match match = s_Regex.Match(s);

                    int sourceIndex = GetNodeIndex(match.Groups["source"].Value);
                    int sinkIndex = GetNodeIndex(match.Groups["sink"].Value);
                    int dist = int.Parse(match.Groups["dist"].Value);

                    AddEdge(sourceIndex, sinkIndex, dist);
                    AddEdge(sinkIndex, sourceIndex, dist);
                }
            }

            public int MinPathAllNodes()
            {
                return _nodeIndices.Values.Select(i => Backtrack(i).Min()).Min();
            }

            public int MaxPathAllNodes()
            {
                return _nodeIndices.Values.Select(i => Backtrack(i).Max()).Max();
            }

            private readonly struct BackTrackNode
            {
                public readonly int Index;
                public readonly bool[] Set;
                public readonly int Total;

                public BackTrackNode(int index, bool[] set, int total)
                {
                    Index = index;
                    Set = set;
                    Total = total;
                }
            }

            private IEnumerable<int> Backtrack(int sourceIndex)
            {
                bool[] initialSet = new bool[_nodeIndices.Count];
                initialSet[sourceIndex] = true;

                Stack<BackTrackNode> stack = new Stack<BackTrackNode>();
                stack.Push(new BackTrackNode(sourceIndex, initialSet, 0));

                while (stack.Count > 0)
                {
                    BackTrackNode current = stack.Pop();
                    if (current.Set.All(b => b))
                    {
                        yield return current.Total;
                    }
                    else
                    {
                        for (int i = 0; i < current.Set.Length; i++)
                        {
                            int dist;
                            if (!current.Set[i] && (dist = _edges[current.Index][i]) != 1)
                            {
                                bool[] newSet = (bool[])current.Set.Clone();
                                newSet[i] = true;
                                stack.Push(new BackTrackNode(i, newSet, current.Total + dist));
                            }
                        }
                    }
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

            private Regex s_Regex = new Regex(
                @"^(?'source'[a-zA-Z]+) to (?'sink'[a-zA-z]+) = (?'dist'\d+)$",
                RegexOptions.Compiled);
        }

        Graph _graph;

        public Day09()
        {
            _graph = new Graph(File.ReadAllLines("Inputs/Day09.txt"));
        }

        [Fact]
        public void Part1()
        {
            int answer = _graph.MinPathAllNodes();

            Assert.Equal(251, answer);
        }

        [Fact]
        public void Part2()
        {
            int answer = _graph.MaxPathAllNodes();

            Assert.Equal(898, answer);
        }
    }
}
