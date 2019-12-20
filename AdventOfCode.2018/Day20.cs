using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Xunit;

namespace AdventOfCode._2018
{
    public class Day20
    {
        class RegexMap
        {
            public delegate T Visitor<T>(T current, ReadOnlySpan<char> directions);

            public RegexMap(string pattern)
            {
                _pattern = pattern;
            }

            public IEnumerable<T> Traverse<T>(T origin, Visitor<T> visitor)
            {
                int index = 0;
                T[] currentOptionHeads = { origin };
                HashSet<T> newHeads = new HashSet<T>();
                Stack<T[]> currentOptionsHeadsStack = new Stack<T[]>();
                Stack<HashSet<T>> newHeadsStack = new Stack<HashSet<T>>();

                while (index < _pattern.Length)
                {
                    char c = _pattern[index];

                    if (c == '(')
                    {
                        index++;
                        currentOptionsHeadsStack.Push(currentOptionHeads);
                        currentOptionHeads = (T[])currentOptionHeads.Clone();
                        newHeadsStack.Push(newHeads);
                        newHeads = new HashSet<T>();
                    }
                    else if (c == '|')
                    {
                        index++;
                        foreach (T n in currentOptionHeads)
                            newHeads.Add(n);

                        currentOptionHeads = (T[])currentOptionsHeadsStack.Peek().Clone();
                    }
                    else if (c == ')')
                    {
                        index++;
                        foreach (T n in currentOptionHeads)
                            newHeads.Add(n);

                        currentOptionHeads = newHeads.ToArray();
                        newHeads = newHeadsStack.Pop();
                        _ = currentOptionsHeadsStack.Pop();
                    }
                    else if (c == '$')
                    {
                        return currentOptionHeads;
                    }
                    else if (c == '^')
                    {
                        index++;
                    }
                    else
                    {
                        int tokenIndex = _pattern.IndexOfAny(s_tokens, index);
                        ReadOnlySpan<char> directions = _pattern.AsSpan(index, tokenIndex - index);
                        for (int i = 0; i < currentOptionHeads.Length; i++)
                            currentOptionHeads[i] = visitor(currentOptionHeads[i], directions);
                        index = tokenIndex;
                    }
                }
                throw new InvalidOperationException("reached end without returning?");
            }

            private string _pattern;

            private static char[] s_tokens = new[] { '|', '(', ')', '^', '$' };
        }

        int[] _distances;

        public Day20()
        {
            RegexMap regex = new RegexMap(File.ReadAllText("Inputs/Day20.txt"));
            int nextIndex = 0;
            Dictionary<IntPoint2, int> nodes = new Dictionary<IntPoint2, int>();
            List<HashSet<int>> graph = new List<HashSet<int>>();

            int GetNodeIndex(IntPoint2 p)
            {
                if (!nodes.TryGetValue(p, out int index))
                    nodes.Add(p, index = nextIndex++);
                return index;
            }

            void AddEdge(IntPoint2 source, IntPoint2 sink)
            {
                int sourceIndex = GetNodeIndex(source);
                int sinkIndex = GetNodeIndex(sink);

                int maxIndex = Math.Max(sourceIndex, sinkIndex);
                while (graph.Count <= maxIndex)
                    graph.Add(new HashSet<int>());

                graph[sourceIndex].Add(sinkIndex);
                graph[sinkIndex].Add(sourceIndex);
            }

            IntPoint2 origin = IntPoint2.Zero;
            regex.Traverse(origin, (current, directions) =>
            {
                foreach (char c in directions)
                {
                    IntPoint2 newCoord = current + c switch
                    {
                        'N' => IntPoint2.UnitY,
                        'S' => -IntPoint2.UnitY,
                        'W' => -IntPoint2.UnitX,
                        'E' => IntPoint2.UnitX,
                        _ => throw new InvalidOperationException()
                    };

                    AddEdge(current, newCoord);

                    current = newCoord;
                }
                return current;
            });

            bool[] visited = new bool[nodes.Count];
            _distances = new int[nodes.Count];
            int currentIndex = GetNodeIndex(origin);
            _distances[currentIndex] = 0;
            Queue<int> toVisit = new Queue<int>();
            toVisit.Enqueue(currentIndex);

            do
            {
                currentIndex = toVisit.Dequeue();

                if (visited[currentIndex])
                    continue;

                visited[currentIndex] = true;

                int newDistance = _distances[currentIndex] + 1;
                foreach (int adjacentIndex in graph[currentIndex])
                {
                    if (!visited[adjacentIndex])
                    {
                        _distances[adjacentIndex] = newDistance;
                        toVisit.Enqueue(adjacentIndex);
                    }
                }
            } while (toVisit.Count > 0);
        }

        [Fact]
        public void Part1()
        {
            int answer = _distances.Max();
            Assert.Equal(3739, answer);
        }

        [Fact]
        public void Part2()
        {
            int answer = _distances.Count(d => d >= 1000);
            Assert.Equal(8409, answer);
        }
    }
}
