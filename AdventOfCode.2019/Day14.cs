using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace AdventOfCode._2019
{
    public class Day14
    {
        class DependencyGraph
        {
            public void AddEdge(string source, string sink, int sourceValue, int sinkValue)
            {
                int sourceIndex = GetNodeIndex(source);
                int sinkIndex = GetNodeIndex(sink);

                while (_graph.Count <= sourceIndex)
                    _graph.Add(new List<(int sinkIndex, int sourceValue, int sinkValue)>());

                _graph[sourceIndex].Add((sinkIndex, sourceValue, sinkValue));
            }

            public long GetNumberRequired(string source, string sink, long required)
            {
                int sourceIndex = GetNodeIndex(source);
                int sinkIndex = GetNodeIndex(sink);
                long[] totals = new long[_nodes.Count];
                totals[sinkIndex] = required;

                return GetNumberRequiredHelper(sourceIndex, totals);
            }

            private long GetNumberRequiredHelper(int index, long[] totals)
            {
                if (totals[index] != 0)
                    return totals[index];

                long total = 0;
                foreach (var entry in _graph[index])
                {
                    long downstreamTotal = GetNumberRequiredHelper(entry.sinkIndex, totals);
                    long multiplier = (downstreamTotal / entry.sinkValue) + Math.Sign(downstreamTotal % entry.sinkValue);
                    total += entry.sourceValue * multiplier;
                }
                return totals[index] = total;
            }

            private int GetNodeIndex(string nodeName)
            {
                if (!_nodes.TryGetValue(nodeName, out int index))
                    _nodes.Add(nodeName, index = _nextIndex++);
                return index;
            }

            int _nextIndex = 0;
            Dictionary<string, int> _nodes = new Dictionary<string, int>();
            List<List<(int sinkIndex, int sourceValue, int sinkValue)>> _graph = new List<List<(int sinkIndex, int sourceValue, int sinkValue)>>();
        }

        DependencyGraph _graph = new DependencyGraph();

        public Day14()
        {
            foreach (string line in File.ReadAllLines("Inputs/Day14.txt"))
            {
                (string node, int value) ParseNode(string entry)
                {
                    string[] parts = entry.Split();
                    return (parts[1], int.Parse(parts[0]));
                }

                string[] split = line.Split(" => ");
                string sources = split[0];
                string sink = split[1];

                (string sinkName, int sinkValue2) = ParseNode(sink);
                foreach (string source in sources.Split(", "))
                {
                    (string sourceName, int sourceValue2) = ParseNode(source);
                    _graph.AddEdge(sourceName, sinkName, sourceValue2, sinkValue2);
                }
            }
        }

        [Fact]
        public void Part1()
        {
            long answer = _graph.GetNumberRequired("ORE", "FUEL", 1);
            Assert.Equal(443537, answer);
        }

        [Fact]
        public void Part2()
        {
            const string source = "ORE";
            const string sink = "FUEL";
            long orePerFuel = _graph.GetNumberRequired(source, sink, 1);

            const long inputOre = 1_000_000_000_000;

            long lowerBound = (inputOre / orePerFuel) + orePerFuel;
            long upperBound = lowerBound + orePerFuel;

            long answer = 0;
            for (; ; )
            {
                if (lowerBound >= upperBound)
                {
                    answer = lowerBound;
                    break;
                }
                long half = lowerBound + (upperBound - lowerBound + 1) / 2;
                answer = _graph.GetNumberRequired(source, sink, half);
                if (answer <= inputOre)
                    lowerBound = half;
                else
                    upperBound = half - 1;
            }

            Assert.Equal(2910558, answer);
        }
    }
}
