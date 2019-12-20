using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
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
                _graph = new List<List<Edge>>(_count);
                for (int i = 0; i < _count; i++)
                    _graph.Add(new List<Edge>(_count));
            }

            struct Edge
            {
                public readonly int SinkIndex;
                public readonly int SinkKeyFlag;
                public readonly int Distance;
                public readonly int KeysRequiredMask;

                public Edge(int sinkIndex, int distance, int keysRequiredMask)
                {
                    SinkIndex = sinkIndex;
                    SinkKeyFlag = 1 << (sinkIndex - 1);
                    Distance = distance;
                    KeysRequiredMask = keysRequiredMask;
                }
            }

            public void AddEdge(int source, int sink, int distance, int[] keysRequired)
            {
                int mask = 0;
                for (int i = 0; i < keysRequired.Length; i++)
                    mask |= (1 << keysRequired[i]);

                _graph[source].Add(new Edge(sink, distance, mask));
            }

            public int MinDistance()
            {
                Dictionary<(int, int), int> states = new Dictionary<(int, int), int>((1 << (_count - 1)) - 1);
                return DynamicHelper(0, 0, 0, states);
            }

            private int DynamicHelper(int pos, int keystate, int keycount, Dictionary<(int pos, int state), int> states)
            {
                if (keycount == _count - 1)
                    return 0;

                if (states.TryGetValue((pos, keystate), out int cached))
                    return cached;

                int count = _graph[pos].Count;
                for (int i = 0; i < count; i++)
                {
                    Edge edge = _graph[pos][i];

                    if ((keystate & edge.SinkKeyFlag) > 0)
                        continue;

                    if ((keystate & edge.KeysRequiredMask) != edge.KeysRequiredMask)
                        continue;

                    int distance = edge.Distance + DynamicHelper(edge.SinkIndex, keystate | edge.SinkKeyFlag, keycount + 1, states);
                    if (!states.TryGetValue((pos, keystate), out int minDistance) || distance < minDistance)
                        states[(pos, keystate)] = distance;
                }

                return states[(pos, keystate)];
            }

            int _count;
            List<List<Edge>> _graph;
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
                BfsKeys(_keys[i], i + 1, true);
            BfsKeys(_entry, 0, false);

            int answer = _graph.MinDistance();
            Assert.Equal(3764, answer);
        }

        private void BfsKeys(IntPoint2 origin, int originIndex, bool passDoors)
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

                if (visited[currentIndex])
                    continue;
                visited[currentIndex] = true;

                IntPoint2 current = IntPoint2.FromIndex(currentIndex, _bounds);
                int distance = distances[currentIndex].distance;
                int newDistance = distance + 1;
                List<int> newKeys = distances[currentIndex].keysNeeded;

                char c = _map[current.X, current.Y];
                if (IsDoor(c, out int doorIndex))
                {
                    if (passDoors)
                    {
                        newKeys = newKeys.ToList();
                        newKeys.Add(doorIndex);
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (IsKey(c, out int keyIndex) && keyIndex != origin.ToIndex(_bounds))
                {
                    _graph.AddEdge(originIndex, keyIndex + 1, distance, newKeys.ToArray());
                }

                foreach (IntPoint2 adjacent in current.Adjacent())
                {
                    int adjacentIndex = adjacent.ToIndex(_bounds);
                    if (visited[adjacentIndex])
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
