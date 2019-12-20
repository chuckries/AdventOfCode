using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace AdventOfCode._2019
{
    public class Day18
    {
        class Graph
        {
            public Graph(int count)
            {
                _count = count;
                _graph = new Edge[count, count];
            }

            class Edge
            {
                public readonly int SinkIndex;
                public readonly int Distance;
                public readonly int KeysRequiredMask;

                public Edge(int sinkIndex, int distance, int keysRequiredMask)
                {
                    SinkIndex = sinkIndex;
                    Distance = distance;
                    KeysRequiredMask = keysRequiredMask;
                }
            }

            public void AddEdge(int source, int sink, int distance, int[] keysRequired)
            {
                int mask = 0;
                for (int i = 0; i < keysRequired.Length; i++)
                    mask |= (1 << keysRequired[i]);

                _graph[source, sink] = new Edge(sink, distance, mask);
            }

            private int DynamicHelper(int pos, int keystate, int keycount, int[,] states)
            {
                if (keycount == _count - 1)
                    return 0;

                if (states[pos, keystate] != int.MaxValue)
                    return states[pos, keystate];

                for (int i = 0; i < _count; i++)
                {
                    Edge edge = _graph[pos, i];
                    if (edge == null)
                        continue;

                    if ((keystate & (1 << (i - 1))) > 0)
                        continue;

                    if ((keystate & edge.KeysRequiredMask) != edge.KeysRequiredMask)
                        continue;

                    int distance = edge.Distance + DynamicHelper(i, keystate | (1 << (i - 1)), keycount + 1, states);
                    if (distance < states[pos, keystate])
                        states[pos, keystate] = distance;
                }

                return states[pos, keystate];
            }

            public int MinDistance()
            {
                int[,] states = new int[_count, (1 << (_count - 1)) - 1];
                for (int i = 0; i < states.GetLength(0); i++)
                    for (int j = 0; j < states.GetLength(1); j++)
                        states[i, j] = int.MaxValue;

                return DynamicHelper(0, 0, 0, states);
            }

            int _count;
            Edge[,] _graph;
        }

        char[,] _map;
        IntPoint2 _bounds;
        IntPoint2[] _keys;
        IntPoint2 _entry;
        Graph _graph;

        public Day18()
        {
            string[] input = File.ReadAllLines("Inputs/Day18.txt");
            List<IntPoint2> keys = new List<IntPoint2>(26);
            _map = new char[input[0].Length, input.Length];
            _bounds = new IntPoint2(_map.GetLength(0), _map.GetLength(1));
            for (int j = 0; j < _bounds.Y; j++)
            {
                for (int i = 0; i < _bounds.X; i++)
                {
                    _map[i, j] = input[j][i];
                    if (IsKey(_map[i, j], out int keyIndex))
                    {
                        while (keys.Count <= keyIndex)
                            keys.Add(IntPoint2.Zero);
                        keys[keyIndex] = new IntPoint2(i, j);
                    }
                    else if (_map[i, j] == '@')
                    {
                        _entry = new IntPoint2(i, j);
                    }
                }
            }
            _keys = keys.ToArray();
            _graph = new Graph(_keys.Length + 1);
        }

        [Fact]
        public void Part1()
        {
            for (int i = 0; i < _keys.Length; i++)
                BfsKeys(_keys[i], i + 1);
            BfsKeys(_entry, 0);

            int answer = _graph.MinDistance();
            Assert.Equal(3764, answer);
        }

        private void BfsKeys(IntPoint2 origin, int originIndex)
        {
            (int distance, List<int> keysNeeded)[] distances = new (int distance, List<int> keysNeeded)[_bounds.X * _bounds.Y];
            for (int i = 0; i < distances.Length; i++)
                distances[i].distance = int.MaxValue;
            bool[] visited = new bool[_bounds.X * _bounds.Y];
            Queue<int> toSearch = new Queue<int>();

            int currentIndex = origin.ToIndex(_bounds);
            toSearch.Enqueue(currentIndex);
            distances[currentIndex] = (0, new List<int>());

            do
            {
                currentIndex = toSearch.Dequeue();
                IntPoint2 current = IntPoint2.FromIndex(currentIndex, _bounds);
                int distance = distances[currentIndex].distance;
                int newDistance = distance + 1;
                List<int> newKeys = distances[currentIndex].keysNeeded;

                char c = _map[current.X, current.Y];
                if (IsDoor(c, out int doorIndex))
                {
                    newKeys = newKeys.ToList();
                    newKeys.Add(doorIndex);
                }
                else if (IsKey(c, out int keyIndex) && keyIndex != origin.ToIndex(_bounds))
                {
                    _graph.AddEdge(originIndex, keyIndex + 1, distance, newKeys.ToArray());
                }

                visited[currentIndex] = true;
                foreach (IntPoint2 adjacent in current.Adjacent())
                {
                    int adjacentIndex = adjacent.ToIndex(_bounds);
                    if (visited[adjacentIndex] || toSearch.Contains(adjacentIndex))
                        continue;

                    c = _map[adjacent.X, adjacent.Y];
                    if (c == '#')
                    {
                        visited[adjacentIndex] = true;
                    }
                    else
                    {
                        distances[adjacentIndex] = (newDistance, newKeys);
                        toSearch.Enqueue(adjacentIndex);
                    }
                }
            } while (toSearch.Count > 0);
        }

        private void BfsEntry()
        {
            int[] distances = new int[_bounds.X * _bounds.Y];
            for (int i = 0; i < distances.Length; i++)
                distances[i] = int.MaxValue;
            bool[] visited = new bool[_bounds.X * _bounds.Y];
            Queue<int> toSearch = new Queue<int>();

            int currentIndex = _entry.ToIndex(_bounds);
            toSearch.Enqueue(currentIndex);
            distances[currentIndex] = 0;

            do
            {
                currentIndex = toSearch.Dequeue();
                IntPoint2 current = IntPoint2.FromIndex(currentIndex, _bounds);
                int newDistance = distances[currentIndex] + 1;

                char c = _map[current.X, current.Y];
                if (IsKey(c, out int keyIndex))
                {
                    _graph.AddEdge(0, keyIndex + 1, distances[currentIndex], new int[0]);
                }

                foreach (IntPoint2 adjacent in IntPoint2.FromIndex(currentIndex, _bounds).Adjacent(_bounds))
                {
                    int adjacentIndex = adjacent.ToIndex(_bounds);
                    if (visited[adjacentIndex])
                        continue;

                    c = _map[adjacent.X, adjacent.Y];
                    if (c == '#' || IsDoor(c, out _))
                    {
                        visited[adjacentIndex] = true;
                    }
                    else
                    {
                        distances[adjacentIndex] = newDistance;
                        toSearch.Enqueue(adjacentIndex);
                    }
                }

                visited[currentIndex] = true;
            } while (toSearch.Count > 0);
        }

        private static bool IsKey(char c, out int keyIndex)
        {
            keyIndex = -1;
            if (c >= 'a' && c <= 'z')
            {
                keyIndex = c - 'a';
                return true;
            }
            return false;
        }

        private static bool IsDoor(char c, out int doorIndex)
        {
            doorIndex = -1;
            if (c >= 'A' && c <= 'Z')
            {
                doorIndex = c - 'A';
                return true;
            }
            return false;
        }
    }
}
