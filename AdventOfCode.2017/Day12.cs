using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AdventOfCode._2017
{
    public class Day12
    {
        List<List<int>> _graph;

        public Day12()
        {
            string[] lines = File.ReadAllLines("Inputs/Day12.txt");

            _graph = new(lines.Length);
            for (int i = 0; i < lines.Length; i++)
                _graph.Add(new());

            for (int i = 0; i < lines.Length; i++)
                foreach (int sink in lines[i].Split("<->")[1].Split(',', StringSplitOptions.TrimEntries).Select(int.Parse))
                    _graph[i].Add(sink);
        }

        [Fact]
        public void Part1()
        {
            bool[] visited = new bool[_graph.Count];
            int answer = Traverse(0, ref visited);

            Assert.Equal(380, answer);
        }

        [Fact]
        public void Part2()
        {
            bool[] visited = new bool[_graph.Count];
            int count = 0;
            for (int i = 0; i < _graph.Count; i++)
            {
                if (!visited[i])
                {
                    count++;
                    Traverse(i, ref visited);
                }
            }

            Assert.Equal(181, count);
        }

        private int Traverse(int start, ref bool[] visited)
        {
            Queue<int> toSearch = new Queue<int>();

            toSearch.Enqueue(start);
            int count = 0;
            while (toSearch.Count > 0)
            {
                int current = toSearch.Dequeue();

                if (visited[current])
                    continue;

                visited[current] = true;
                count++;

                foreach (int next in _graph[current])
                    if (!visited[next])
                        toSearch.Enqueue(next);
            }

            return count;
        }
    }
}
