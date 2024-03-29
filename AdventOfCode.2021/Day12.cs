﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode._2021
{
    public class Day12
    {
        private bool[] _nodes;
        private List<List<int>> _graph;
        private readonly int _start;
        private readonly int _end;

        public Day12()
        {
            string[] lines = File.ReadAllLines("Inputs/Day12.txt");

            _graph = new(lines.Length * 2);
            Dictionary<string, int> nodes = new(lines.Length * 2);

            Func<string, int> getNodeIndex = name =>
            {
                if (!nodes.TryGetValue(name, out int index))
                    nodes[name] = index = nodes.Count;
                return index;
            };

            Func<int, List<int>> getDestList = index =>
            {
                while (index >= _graph.Count)
                    _graph.Add(new());

                return _graph[index];
            };

            foreach (string line in lines)
            {
                string[] tok = line.Split('-');
                int index0 = getNodeIndex(tok[0]);
                int index1 = getNodeIndex(tok[1]);

                getDestList(index0).Add(index1);
                getDestList(index1).Add(index0);
            }

            _nodes = new bool[nodes.Count];
            foreach (var kvp in nodes)
            {
                _nodes[kvp.Value] = char.IsLower(kvp.Key[0]);
                if (kvp.Key == "start")
                    _start = kvp.Value;
                else if (kvp.Key == "end")
                    _end = kvp.Value;
            }
        }

        [Fact]
        public void Part1()
        {
            int answer = CountPaths(false);
            Assert.Equal(3292, answer);
        }

        [Fact]
        public void Part2()
        {
            int answer = CountPaths(true);
            Assert.Equal(89592, answer);
        }

        private int CountPaths(bool allowDouble)
        {
            int count = 0;
            Backtrack(_start, allowDouble, new bool[_nodes.Length], ref count);
            return count;

            void Backtrack(int current, bool allowDouble, bool[] visited, ref int count)
            {
                foreach (int cand in _graph[current])
                {
                    if (cand == _start)
                        continue;
                    else if (cand == _end)
                        count++;
                    else
                    {
                        bool isLower = _nodes[cand];
                        bool hasVisited = visited[cand];
                        bool shouldBacktrack = !isLower || !hasVisited || allowDouble;

                        if (shouldBacktrack)
                        {
                            bool toggleVisited = isLower && !hasVisited;

                            if (toggleVisited)
                                visited[cand] = true;

                            Backtrack(cand, allowDouble && !hasVisited, visited, ref count);

                            if (toggleVisited)
                                visited[cand] = false;
                        }
                    }
                }
            }
        }
    }
}
