using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode._2019
{
    public class Day20
    {
        class Graph
        {
            public void AddEdge(string source, string sink, int weight)
            {
                int sourceIndex = GetNodeIndex(source);
                int sinkIndex = GetNodeIndex(sink);

                while (_adjList.Count <= sourceIndex)
                    _adjList.Add(new List<(int, int)>());

                _adjList[sourceIndex].Add((sinkIndex, weight));
            }

            //public int MinDistance(string source, string sink)
            //{
            //    int sourceIndex = GetNodeIndex(source);
            //    int sinkIndex = GetNodeIndex(sink);

            //    int[] distances = Enumerable.Repeat(int.MaxValue, _adjList.Count).ToArray();
            //    bool[] visited = new bool[_adjList.Count];
            //    PriorityQueue<(int index, int total)> toSearch = new PriorityQueue<(int index, int total)>
            //        (Comparer<(int, int total)>.Create((left, right) =>
            //        {
            //            return left.total - right.total;
            //        }));

            //    distances[sourceIndex] = 0;
            //    toSearch.Enqueue((sourceIndex, 0));

            //    int currentIndex;
            //    do
            //    {
            //        (currentIndex, curr = toSearch.Dequeue();
            //        if (visited[currentIndex])
            //            continue;


            //    }
            //}

            private int GetNodeIndex(string node)
            {
                if (!_nodes.TryGetValue(node, out int index))
                    _nodes.Add(node, index = _nextIndex++);
                return index;
            }

            List<List<(int sinkIndex, int weight)>> _adjList = new List<List<(int, int)>>();
            Dictionary<string, int> _nodes;
            int _nextIndex = 0;
        }
    }
}
